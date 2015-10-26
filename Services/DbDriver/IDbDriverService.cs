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
using System.Collections.Generic;

namespace BabelMeta.Services.DbDriver
{
    /// <summary>
    /// Interface defining the basic CRUD operations for a database.
    /// </summary>
    public interface IDbDriverService
    {
        /// <summary>
        /// Drops and creates the table with appropriate fields, before serialization (InsertMany).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="optionalExplicitTitle"></param>
        void InitializeTable<T>(String optionalExplicitTitle = "");

        /// <summary>
        /// Checks whether a table expected to host T-type records is valid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="optionalExplicitTitle"></param>
        /// <returns></returns>
        bool IsValidTable<T>(String optionalExplicitTitle = "");

        /// <summary>
        /// Inserts the T-type records in an initialized table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entries"></param>
        /// <param name="optionalExplicitTitle"></param>
        void InsertMany<T>(List<T> entries, String optionalExplicitTitle = "");

        /// <summary>
        /// Dumps all T-type records from a valid table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> SelectAll<T>();
    }
}
