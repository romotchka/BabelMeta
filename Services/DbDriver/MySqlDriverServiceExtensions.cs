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
using System.Reflection;

namespace BabelMeta.Services.DbDriver
{
    public static class MySqlDriverServiceExtensions
    {
        public static String ToMySqlType(this PropertyInfo p)
        {
            if (p == null)
            {
                return String.Empty;
            }
            var t = p.PropertyType;
            if (t == typeof (bool))
            {
                return "tinyint(1) NOT NULL";
            }
            if (t == typeof(bool?))
            {
                return "tinyint(1) DEFAULT NULL";
            }
            if (t == typeof(DateTime))
            {
                return "datetime NOT NULL";
            }
            if (t == typeof(DateTime?))
            {
                return "datetime DEFAULT NULL";
            }
            if (t == typeof(int))
            {
                return "int(11) NOT NULL";
            }
            if (t == typeof(int?))
            {
                return "int(11) DEFAULT NULL";
            }
            if (t == typeof(long))
            {
                return "bigint(20) NOT NULL";
            }
            if (t == typeof(long?))
            {
                return "bigint(20) DEFAULT NULL";
            }
            if (t == typeof(short))
            {
                return "smallint(6) NOT NULL";
            }
            if (t == typeof(short?))
            {
                return "smallint(6) DEFAULT NULL";
            }

            // String fields or Object fields (serialized) seek a MaxSize attribute.
            var dbFieldAttribute = p.GetCustomAttribute<DbField>();
            return  (
                        dbFieldAttribute != null
                        && dbFieldAttribute.MaxSize > 0
                    )
                ? String.Format("varchar({0}) DEFAULT NULL", dbFieldAttribute.MaxSize)
                : "text DEFAULT NULL";
        }

        public static String ToMySqlFieldName(this PropertyInfo p)
        {
            return p == null ? String.Empty : p.Name.ToLower();
        }

        public static String Placeholder(this PropertyInfo p)
        {
            return p == null ? String.Empty : "@" + p.Name;
        }
    }
}
