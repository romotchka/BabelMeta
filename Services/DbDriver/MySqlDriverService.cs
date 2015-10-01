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

using System.Globalization;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using BabelMeta.Helpers;
using Microsoft.Office.Interop.Excel;
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

        private static DbDriverConfig _config;

        /// <summary>
        /// The connection to the database.
        /// </summary>
        private MySqlConnection _connection;

        /// <summary>
        /// The connection to the information schema (used for deserialization tests).
        /// </summary>
        private MySqlConnection _informationSchemaConnection;

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
                Debug.WriteLine("MySqlDriverService.Initialize, MySql version: " + _connection.ServerVersion);
                _connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySqlDriverService.Initialize, " + ex);
            }
            finally
            {
                if (_connection != null)
                {
                    _connection.Close();
                }
            }

            var infoConnectionString = String.Format(
                "server={0};userid={1};password={2};database={3}"
                , config.DbServerName
                , config.DbDatabaseUser
                , config.DbDatabasePassword
                , "information_schema");

            try
            {
                _informationSchemaConnection = new MySqlConnection(infoConnectionString);
                _informationSchemaConnection.Open();
                _informationSchemaConnection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySqlDriverService.Initialize, " + ex);
            }
            finally
            {
                if (_informationSchemaConnection != null)
                {
                    _informationSchemaConnection.Close();
                }
            }

            _config = config;
            IsInitialized = true;
        }

        /// <summary>
        /// Retrieves the relevant properties from a T type, eligible for serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static List<PropertyInfo> TableProperties<T>()
        {
            var t = typeof(T);
            return t.GetProperties()
                .Where(p => p.GetCustomAttribute<DbField>() == null || !p.GetCustomAttribute<DbField>().Ignore)
                .ToList();
        }

        /// <summary>
        /// Builds a unique field name for the index, different from those of all properties. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static String PrimaryKeyFieldName<T>(int suffix = 0)
        {
            var t = typeof(T);
            var tableProperties = TableProperties<T>();
            var candidateFieldName =
                t.Name + "_id"
                + ((suffix == 0) ? String.Empty : suffix.ToString(CultureInfo.InvariantCulture));
            return tableProperties.Select(p => p.ToMySqlFieldName()).Contains(candidateFieldName) 
                ? PrimaryKeyFieldName<T>(suffix + 1) 
                : candidateFieldName;
        }

        private static String DefaultTableName<T>()
        {
            var t = typeof(T);
            return t.Name.Replace(".", "").ToLowerInvariant();
        }

        /// <summary>
        /// Creates a table for type T and adds an internal primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="optionalExplicitTitle"></param>
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
                    .Select(p => "`" + p.ToMySqlFieldName() + "` " + p.ToMySqlType())
                    .ToList();
                // Add to the list an internal id, so as to authorize external edition.
                var primaryKeyFieldName = PrimaryKeyFieldName<T>();
                fieldDeclarationsList.Add("`" + primaryKeyFieldName + "` " +  "int(11) unsigned NOT NULL AUTO_INCREMENT");
                fieldDeclarationsList.Add("PRIMARY KEY (`" + primaryKeyFieldName + "`)");
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
                Debug.WriteLine("MySqlDriverService.InitializeTable, exception " + ex);
            } 
            finally 
            {
                if (_connection != null) 
                {
                    _connection.Close();
                }
            }
        }

        public bool IsValidTable<T>(String optionalExplicitTitle = "")
        {
            if (_connection == null)
            {
                return false;
            }

            var properties = TableProperties<T>();
            if (!(properties.Count > 0))
            {
                return true;
            }

            try
            {
                _informationSchemaConnection.Open();

                MySqlCommand cmd;

                // Drop table.
                var tableName = String.IsNullOrEmpty(optionalExplicitTitle)
                    ? DefaultTableName<T>()
                    : optionalExplicitTitle;

                var retrieveColumnInfoCommandText = 
                    " SELECT column_name, column_type, is_nullable" +
                    " FROM columns" +
                    " WHERE table_schema = '" + _config.DbDatabaseName + "'" +
                    " AND table_name = '" + tableName + "'";
                cmd = new MySqlCommand
                {
                    Connection = _informationSchemaConnection,
                    CommandText = retrieveColumnInfoCommandText,
                };
                var reader = cmd.ExecuteReader();
                if (reader == null)
                {
                    return false;
                }

                // The column order is not predictable.
                var columnNames = new List<String>
                {
                    reader.GetName(0),
                    reader.GetName(1),
                    reader.GetName(2),
                };

                var columnNameIndex =
                    Enumerable.Range(0, 3)
                        .FirstOrDefault(
                            i => String.Compare(
                                columnNames[i].Trim().ToLower()
                                , "column_name"
                                , StringComparison.Ordinal) == 0);
                if (!(columnNameIndex >= 0 && columnNameIndex < 3))
                {
                    return false;
                }

                var columnTypeIndex =
                    Enumerable.Range(0, 3)
                        .FirstOrDefault(
                            i => String.Compare(
                                columnNames[i].Trim().ToLower()
                                , "column_type"
                                , StringComparison.Ordinal) == 0);
                if (!(columnTypeIndex >= 0 && columnTypeIndex < 3))
                {
                    return false;
                }

                var isNullableIndex =
                    Enumerable.Range(0, 3)
                        .FirstOrDefault(
                            i => String.Compare(
                                columnNames[i].Trim().ToLower()
                                , "is_nullable"
                                , StringComparison.Ordinal) == 0);
                if (!(isNullableIndex >= 0 && isNullableIndex < 3))
                {
                    return false;
                }

                var columnInfos = new List<ColumnInfo>();
                while (reader.Read())
                {
                    columnInfos.Add(new ColumnInfo
                    {
                        ColumnName = reader.GetString(columnNameIndex),
                        ColumnType = reader.GetString(columnTypeIndex),
                        IsNullable = reader.GetString(isNullableIndex),
                    });
                }

                // Check that any T property is present in the table in the expected type.
                foreach (var property in properties)
                {
                    if  (!columnInfos.Exists(c => 
                            String.Compare(c.ColumnName, property.ToMySqlFieldName(), StringComparison.Ordinal) == 0
                            && property.ToMySqlType().ToLower().Contains(c.ColumnType.Trim().ToLower())
                            &&  (
                                    (
                                        String.Compare(c.IsNullable.Trim().ToLower(), "yes", StringComparison.Ordinal) == 0
                                        && !property.ToMySqlType().ToLower().Contains("not null")
                                    )
                                    ||
                                    (
                                        String.Compare(c.IsNullable.Trim().ToLower(), "no", StringComparison.Ordinal) == 0
                                        && property.ToMySqlType().ToLower().Contains("not null")
                                    )
                                )
                            )
                        )
                    {
                        Debug.WriteLine("MySqlDriverService.IsValidTable, wrong property definition, " 
                                    + tableName 
                                    + "." + property.Name);
                        return false;
                    }
                }
                Debug.WriteLine("MySqlDriverService.IsValidTable, ok, " + tableName);
                return true;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySqlDriverService.IsValidTable, exception " + ex);
            }
            finally
            {
                if (_informationSchemaConnection != null)
                {
                    _informationSchemaConnection.Close();
                }
            }

            return false;
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
                            Debug.WriteLine("MySqlDriverService.InsertMany, unexpected null property value " + property.Name);
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

    public class ColumnInfo
    {
        public String ColumnName { get; set; }

        public String ColumnType { get; set; }

        public String IsNullable { get; set; }
    }
}
