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

using System;

namespace BabelMeta.Services.DbDriver
{
    public class DbDriverConfig
    {
        public String DbEngineType { get; set; }

        public String DbServerName { get; set; }

        public String DbDatabaseName { get; set; }

        /// <summary>
        /// WARNING! the user privileges may require to be granted reading access in the information schema.
        /// </summary>
        public String DbDatabaseUser { get; set; }

        public String DbDatabasePassword { get; set; }

        public enum DbEngineTypeEnum 
        {
            Cassandra,
            MongoDb,
            MySql,
            Oracle,
            PostgreSql,
            SqlServer,
            Unknown,
        }

        public DbEngineTypeEnum DbEngineTypeEnumValue
        {
            get
            {
                if (String.IsNullOrEmpty(DbEngineType))
                {
                    return DbEngineTypeEnum.Unknown;
                }

                switch (DbEngineType.Trim().ToLower())
                {
                    case "cassandra": return DbEngineTypeEnum.Cassandra;
                    case "mongodb": return DbEngineTypeEnum.MongoDb;
                    case "mysql": return DbEngineTypeEnum.MySql;
                    case "oracle": return DbEngineTypeEnum.Oracle;
                    case "postegresql": return  DbEngineTypeEnum.PostgreSql;
                    case "sqlserver": return DbEngineTypeEnum.SqlServer;
                    default: return DbEngineTypeEnum.Unknown;
                }
            }
        }
    }
}
