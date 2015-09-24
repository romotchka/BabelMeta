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

using System.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BabelMeta.Helpers
{
    public static class DbSerializationHelper
    {
        /// <summary>
        /// Basic types which do not need dedicated (de)serialization
        /// </summary>
        public static readonly Dictionary<Type, String> BasicTypes = new Dictionary<Type, String>
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

        public static object DbSerialize(this object o)
        {
            var typeO = o.GetType();

            var isBasicO = BasicTypes != null && BasicTypes.Keys.Contains(typeO);

            // For basic types, the input object is immediately returned.
            if (isBasicO)
            {
                return o;
            }

            try
            {
                var s = JsonConvert.SerializeObject(o);
                return s;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw new DbSerializationException();
            }
        }

        public static object DbDeserialize<T>(this object o) where T : class
        {
            var typeT = typeof(T);
            var typeO = o.GetType();

            var isBasicT = BasicTypes != null && BasicTypes.Keys.Contains(typeT);
            var isBasicO = BasicTypes != null && BasicTypes.Keys.Contains(typeO);

            // For basic types, the input object is immediately returned if both types match ('nullable-insensitive' comparison).
            if  (
                    isBasicT && isBasicO
                    &&
                    (
                        typeO == typeT
                        || typeO.IsSubclassOf(typeT)
                        || String.Compare(BasicTypes[typeT], BasicTypes[typeO], StringComparison.Ordinal) == 0
                    )
                )
            {
                return o as T;
            }

            // Any complex type is supposed to have been serialized into a String.
            var s = o as String;
            if (s == null)
            {
                throw new DbSerializationException();
            }

            try
            {
                var t = JsonConvert.DeserializeObject<T>(s);
                return t;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw new DbSerializationException();
            }
        }
    }

    public class DbSerializationException : Exception
    {
        
    }
}
