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

namespace BabelMetaClassifier.Model
{
    public interface IDataSet
    {
        List<IRowIndex> RowIndexes { get; set; }
        List<IColumnIndex> ColumnIndexes { get; set; }

        /// <summary>
        /// Ordered list of splits that will be successively applied on data content.
        /// In case 
        /// </summary>
        List<SplitPattern> SplitPatterns { get; set; }

        List<ICell> Cells { get; set; }

        List<GenericDataType> DataTypes { get; set; }

        List<GenericDataClass> DataClasses { get; set; }

        /// <summary>
        /// Likelihood for colum index to be of a certain data type.
        /// </summary>
        Dictionary<IColumnIndex, Dictionary<GenericDataType, double?>> ColumnTypeLikelihood { get; set; }

        /// <summary>
        /// Likelihood for colum index to be of a certain data class.
        /// </summary>
        Dictionary<IColumnIndex, Dictionary<GenericDataClass, double?>> ColumnClassLikelihood { get; set; }

        /// <summary>
        /// Adds a row of data and generate corresponding cells.
        /// </summary>
        /// <param name="row"></param>
        void AddRow(List<String> row);

        /// <summary>
        /// For split patterns with dynamic split occurrences, parses data to determine each right number of occurrences.
        /// </summary>
        void InitializeSplitPatterns();

        /// <summary>
        /// Initialize the likelihood that a column index be of given type.
        /// </summary>
        void InitializeColumnTypeLikelihood(IColumnIndex columnIndex = null, GenericDataType dataType = null);

        /// <summary>
        /// Initialize the likelihood that a column index be of given type.
        /// </summary>
        void InitializeColumnClassLikelihood(IColumnIndex columnIndex = null, GenericDataClass dataClass = null);

        /// <summary>
        /// Generates the child column indexes for the given column index (or all root column indexes), according to the column split pattern corresponding to its depth.
        /// </summary>
        /// <param name="columnIndex"></param>
        void ApplySplitPattern(IColumnIndex columnIndex = null);
    }
}
