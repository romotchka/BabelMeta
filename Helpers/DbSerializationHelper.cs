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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace BabelMeta.Helpers
{
    public static class DbSerializationHelper
    {
        /// <summary>
        /// Basic types which do not need dedicated (de)serialization.
        /// </summary>
        private static readonly Dictionary<Type, String> _basicTypes = new Dictionary<Type, String>
        {
            {typeof(bool), "bool"},           
            {typeof(bool?), "bool"},            
            {typeof(DateTime), "datetime"},
            {typeof(DateTime?), "datetime"},
            {typeof(int), "integer"},
            {typeof(int?), "integer"},    
            {typeof(long), "long"},   
            {typeof(long?), "long"},    
            {typeof(short), "short"},   
            {typeof(short?), "short"},    
            {typeof(String), "string"},   
        };

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
        };

        /// <summary>
        /// Serialization method, performing a serialization for complex types only and letting other objects 'as is'.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static object DbSerialize(this object o)
        {
            if (o == null)
            {
                return null;
            }

            var typeO = o.GetType();

            var isBasicO = _basicTypes != null && _basicTypes.Keys.Contains(typeO);

            // For basic types, the input object is immediately returned.
            if (isBasicO)
            {
                return o;
            }

            // Enum case
            if (typeO.IsEnum)
            {
                return o.ToString();
            }

            try
            {
                var s = JsonConvert.SerializeObject(o, Formatting.None, _jsonSerializerSettings);
                return s;
            }
            catch (Exception ex)
            {
                Debug.Write("DbSerializationHelper.DbSerialize, " + ex);
                throw new DbSerializationException();
            }
        }

        /// <summary>
        /// Deerialization method, performing a deserialization for complex types only and letting other objects 'as is'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static object DbDeserialize<T>(this object o) 
        {
            if (o == null)
            {
                return null;
            }

            var typeT = typeof(T);
            var typeO = o.GetType();

            var isBasicT = _basicTypes != null && _basicTypes.Keys.Contains(typeT);
            var isBasicO = _basicTypes != null && _basicTypes.Keys.Contains(typeO);

            // For basic types, the input object is immediately returned if both types match ('nullable-insensitive' comparison).
            if  (
                    isBasicT && isBasicO
                    &&
                    (
                        typeO == typeT
                        || typeO.IsSubclassOf(typeT)
                        || String.Compare(_basicTypes[typeT], _basicTypes[typeO], StringComparison.Ordinal) == 0
                    )
                )
            {
                return o is T ? (T)o : default(T);
            }

            // Any enum or complex type is supposed to have been serialized into a String.
            var s = o as String;
            if (s == null)
            {
                throw new DbSerializationException();
            }

            // Enum case
            if (typeT.IsEnum)
            {
                try
                {
                    var dS = Enum.Parse(typeT, s);
                    return dS;
                }
                catch (Exception ex)
                {
                    Debug.Write("DbSerializationHelper.DbDeserialize, " + ex);
                    throw new DbSerializationException();
                }
            }

            // Complex object case
            try
            {
                var dS = JsonConvert.DeserializeObject<T>(s, _jsonSerializerSettings);
                return dS;
            }
            catch (Exception ex)
            {
                Debug.Write("DbSerializationHelper.DbDeserialize, " + ex);
                throw new DbSerializationException();
            }
        }
    }

    public class DbSerializationException : Exception
    {
        
    }
}
