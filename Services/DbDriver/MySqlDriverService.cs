/*
 *  Babel Meta - babelmeta.com
 * 
 *  The present software is licensed according to the MIT licence.
 *  Copyright (c) 2015 Romain Carbou (romain@babelmeta.com)
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE. 
 */

using System.Linq;
using System.Reflection;
using BabelMeta.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BabelMeta.Services.DbDriver
{
    public class MySqlDriverService : IDbDriverService
    {
        private const String _tableCreationParametersText = "ENGINE=INNODB DEFAULT CHARSET=utf8";
        private static MySqlDriverService _instance;

        private MySqlDriverService()
        {
            IsInitialized = false;
        }

        public static MySqlDriverService Instance
        {
            get 
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = new MySqlDriverService();

                return _instance;
            }
        }

        private MySqlConnection _connection;

        public bool IsInitialized { get; private set; }

        public void Initialize(DbDriverConfig config)
        {
            IsInitialized = false;

            if  (
                    config == null
                    || config.DbEngineTypeEnumValue != DbDriverConfig.DbEngineTypeEnum.MySql
                    || String.IsNullOrEmpty(config.DbServerName)
                    || String.IsNullOrEmpty(config.DbDatabaseUser)
                    || String.IsNullOrEmpty(config.DbDatabaseName)
                )
            {
                return;
            }

            var connectionString = String.Format(
                "server={0};userid={1};password={2};database={3}"
                , config.DbServerName
                , config.DbDatabaseUser
                , config.DbDatabasePassword
                , config.DbDatabaseName);

            try
            {
                _connection = new MySqlConnection(connectionString);
                _connection.Open();
                Debug.WriteLine(String.Format("MySql version: {0}", _connection.ServerVersion));
                _connection.Close();
                IsInitialized = true;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Error: {0}", ex);
            }
            finally
            {
                if (_connection != null)
                {
                    _connection.Close();
                }
            }
        }

        private static List<PropertyInfo> TableProperties<T>()
        {
            var t = typeof(T);
            return t.GetProperties()
                .Where(p => p.GetCustomAttribute<DbField>() == null || !p.GetCustomAttribute<DbField>().Ignore)
                .ToList();
        }

        private static String DefaultTableName<T>()
        {
            var t = typeof(T);
            return t.Name.Replace(".", "").ToLowerInvariant();
        }

        public void InitializeTable<T>(String optionalExplicitTitle = "")
        {
            if (_connection == null)
            {
                return;
            }

            var properties = TableProperties<T>();
            if (!(properties.Count > 0))
            {
                return;
            }

            try 
            {
                _connection.Open();

                MySqlCommand cmd;

                // Drop table.
                var tableName = String.IsNullOrEmpty(optionalExplicitTitle)
                    ? DefaultTableName<T>()
                    : optionalExplicitTitle;
                var dropTableCommandText = "DROP TABLE IF EXISTS " + tableName;
                cmd = new MySqlCommand
                {
                    Connection = _connection, 
                    CommandText = dropTableCommandText,
                };
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                // Create table.
                var createTableCommandText = "CREATE TABLE IF NOT EXISTS `" + tableName + "` (";
                var fieldDeclarationsList = properties
                    .Select(p => "`" + p.Name.ToLower() + "` " + p.ToMySqlType())
                    .ToList();
                createTableCommandText += String.Join(",", fieldDeclarationsList);
                createTableCommandText += ") " + _tableCreationParametersText;
                cmd = new MySqlCommand
                {
                    Connection = _connection,
                    CommandText = createTableCommandText,
                };
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            } 
            catch (MySqlException ex) 
            {
                Debug.Write("MySqlDriverService.InitializeTable, exception " + ex);
            } 
            finally 
            {
                if (_connection != null) 
                {
                    _connection.Close();
                }
            }
        }

        public void InsertMany<T>(List<T> entries, String optionalExplicitTitle = "")
        {
            if  (
                    _connection == null 
                    || entries == null 
                    || !(entries.ToList().Count > 0)
                )
            {
                return;
            }

            var tableName = String.IsNullOrEmpty(optionalExplicitTitle)
                ? DefaultTableName<T>()
                : optionalExplicitTitle;

            var properties = TableProperties<T>();
            if (!(properties.Count > 0))
            {
                return;
            }
            var joinPropertyNames = String.Join(",", properties.Select(p => "`" + p.Name.ToLower() + "`"));
            var joinPlaceholders = String.Join(",", properties.Select(p => p.Placeholder()));

            try
            {
                _connection.Open();
                MySqlCommand cmd;

                foreach (var entry in entries)
                {
                    var insertEntryText = "INSERT INTO `" + tableName + "` (";
                    insertEntryText += joinPropertyNames;
                    insertEntryText += ") VALUES (";
                    insertEntryText += joinPlaceholders;
                    insertEntryText += ")";

                    cmd = new MySqlCommand();
                    cmd.Connection = _connection;
                    cmd.CommandText = insertEntryText;
                    cmd.Prepare();

                    // Substitute field placeholders one by one. 
                    var cancellationToken = false;
                    foreach (var property in properties)
                    {
                        var entryPropertyValue = property.GetValue(entry, null);
                        if  (
                                entryPropertyValue == null 
                                && property.ToMySqlType().ToLower().Contains("not null")
                            )
                        {
                            Debug.WriteLine("MySqlDriverService.InsertMany, unexpected null property value: " + property.Name);
                            cancellationToken = true;
                            break;
                        }
                        cmd.Parameters.AddWithValue(
                            property.Placeholder(), 
                            entryPropertyValue != null 
                                ? entryPropertyValue.DbSerialize() 
                                : null
                            );
                    }
                    if (cancellationToken)
                    {
                        continue;
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: {0}", ex);
            }
            finally
            {
                if (_connection != null)
                {
                    _connection.Close();
                }
            }
        }

        public List<T> SelectAll<T>()
        {
            if (_connection == null)
            {
                return null;
            }
            throw new NotImplementedException();
        }
    }
}
