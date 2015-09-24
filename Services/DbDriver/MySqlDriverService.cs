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

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BabelMeta.Services.DbDriver
{
    public class MySqlDriverService : IDbDriverService
    {
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
                    || String.IsNullOrEmpty(config.DbDatabasePassword)
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
                Debug.WriteLine("MySql version: {0}", _connection.ServerVersion);

                // Initialize tables
                // TODO

                IsInitialized = true;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Error: {0}", ex.ToString());
            }
            finally
            {
                if (_connection != null)
                {
                    _connection.Close();
                }
            }
        }

        public void InsertMany<T>(IEnumerable<T> entries)
        {
            if (_connection == null)
            {
                return;
            }
            throw new NotImplementedException();
        }

        public IEnumerable<T> SelectAll<T>()
        {
            if (_connection == null)
            {
                return null;
            }
            throw new NotImplementedException();
        }
    }
}
