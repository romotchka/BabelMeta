﻿/*
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
using System.Diagnostics;
using System.Linq;
using BabelMetaClassifier.Helpers;

namespace BabelMetaClassifier.Model
{
    public class DataSet : IDataSet
    {
        public List<IRowIndex> RowIndexes { get; set; }
        public List<IColumnIndex> ColumnIndexes { get; set; }

        public List<SplitPattern> SplitPatterns { get; set; }

        public List<ICell> Cells { get; set; }

        public List<GenericDataType> DataTypes { get; set; }

        public List<GenericDataClass> DataClasses { get; set; }

        public Dictionary<IColumnIndex, Dictionary<GenericDataType, double?>> ColumnTypeLikelihood { get; set; }

        public Dictionary<IColumnIndex, Dictionary<GenericDataClass, double?>> ColumnClassLikelihood { get; set; }

        private readonly object _dataSetLock;

        public DataSet()
        {
            _dataSetLock = new object(); // Lock

            RowIndexes = new List<IRowIndex>();
            ColumnIndexes = new List<IColumnIndex>();
            SplitPatterns = new List<SplitPattern>();
            Cells = new List<ICell>();
            DataTypes = new List<GenericDataType>();
            DataClasses = new List<GenericDataClass>();
            ColumnTypeLikelihood = new Dictionary<IColumnIndex, Dictionary<GenericDataType, double?>>();
            ColumnClassLikelihood = new Dictionary<IColumnIndex, Dictionary<GenericDataClass, double?>>();
        }

        public void AddRow(List<String> row)
        {
            if (row == null || row.Count == 0)
            {
                return;
            }
            lock (_dataSetLock)
            {
                var newRowIndexValue = RowIndexes.Count == 0
                    ? 0
                    : RowIndexes
                    .Where(i => i.Parent == null)
                    .Max(i => i.Index) + 1;
                var newRowIndex = new RowIndex(newRowIndexValue);
                RowIndexes.Add(newRowIndex);
                Debug.WriteLine("DataSet.AddRow, adding row index.");

                var currentMaxColumnIndexValue = ColumnIndexes.Count == 0
                    ? -1
                    : ColumnIndexes
                    .Where(i => i.Parent == null)
                    .Max(i => i.Index);

                // Generate new column indexes if needed.
                for (var n = currentMaxColumnIndexValue + 1; n < row.Count; n++)
                {
                    var columnIndex = new ColumnIndex(n);
                    ColumnIndexes.Add(columnIndex);
                    Debug.WriteLine("DataSet.AddRow, adding column index.");
                }

                // Finally, populate cells.
                for (var n = 0; n < row.Count; n++)
                {
                    var columnIndex = ColumnIndexes
                        .Where(i => i.Parent == null)
                        .FirstOrDefault(i => i.Index == n);
                    
                    // By construction, should never happen.
                    if (columnIndex == null)
                    {
                        continue;
                    }

                    var cell = new Cell(newRowIndex, columnIndex, row[n]);
                    Cells.Add(cell);
                    Debug.WriteLine("DataSet.AddRow, adding cell.");
                }
            }
        }

        public void InitializeColumnTypeLikelihood(IColumnIndex columnIndex = null, GenericDataType dataType = null)
        {
            // Proceed with all columns, if null index.
            if (columnIndex == null)
            {
                ColumnIndexes.ForEach(i => InitializeColumnTypeLikelihood(i));
                Debug.WriteLine("DataSet.InitializeColumnTypeLikelihood, proceed all columns.");
                return;
            }
            // If unknown column or if no matching cell, return now, rather than later.
            if  (
                    !ColumnIndexes.Contains(columnIndex)
                    || !Cells.Exists(c => c.CellColumnIndex == columnIndex)
                )
            {
                return;
            }
            // Proceed with all types for the given column, if null type.
            if (dataType == null)
            {
                DataTypes.ForEach(t => InitializeColumnTypeLikelihood(columnIndex, t));
                Debug.WriteLine("DataSet.InitializeColumnTypeLikelihood, proceed all types.");
                return;
            }
            // If unknown type, return.
            if (!DataTypes.Contains(dataType))
            {
                return;
            }
            lock (_dataSetLock)
            {
                // Pay attention to strict implementation of Weight nullity for parent indexes.
                var rowsCardinality = RowIndexes.Sum(i => i.Weight);
                if (rowsCardinality < 1.0)
                {
                    Debug.WriteLine("DataSet.InitializeColumnTypeLikelihood, insufficient cardinality.");
                    return;
                }

                // Prepare likelihood entry in the nested dictionary.
                if (!ColumnTypeLikelihood.ContainsKey(columnIndex))
                {
                    ColumnTypeLikelihood.Add(
                        columnIndex,
                        new Dictionary<GenericDataType, double?>()
                        );
                    Debug.WriteLine("DataSet.InitializeColumnTypeLikelihood, new dictionary column entry.");
                }
                Dictionary<GenericDataType, double?> columnLikelihoodValues;
                if (!ColumnTypeLikelihood.TryGetValue(columnIndex, out columnLikelihoodValues))
                {
                    return; // Impossible.
                }
                if (!columnLikelihoodValues.ContainsKey(dataType))
                {
                    columnLikelihoodValues.Add(dataType, null);
                    Debug.WriteLine("DataSet.InitializeColumnTypeLikelihood, new dictionary data type entry.");
                }

                // Compute likelihood.
                var likelihood = Cells
                    .Where(c =>
                        c.CellRowIndex != null && c.CellRowIndex.Weight > 0.0
                        && c.CellColumnIndex == columnIndex
                        && c.Value != null // Must not filter non-null empty strings.
                        )
                    .Sum(c =>
                        c.CellRowIndex.Weight
                        * c.Value.LikelihoodToBeType(dataType)
                        )
                        /
                        rowsCardinality;
                columnLikelihoodValues[dataType] = likelihood;
                Debug.WriteLine("DataSet.InitializeColumnTypeLikelihood, likelihood for current column/type is " + likelihood);
            }
        }

        public void InitializeColumnClassLikelihood(IColumnIndex columnIndex = null, GenericDataClass dataClass = null)
        {

        }
    }
}
