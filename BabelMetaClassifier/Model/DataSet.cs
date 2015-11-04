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
using System.Linq;

namespace BabelMetaClassifier.Model
{
    public class DataSet : IDataSet
    {
        private List<IRowIndex> _rowIndexes;
        private List<IColumnIndex> _columnIndexes;

        public DataSet()
        {
            _dataInsert = new object();

            _rowIndexes = new List<IRowIndex>();
            _columnIndexes = new List<IColumnIndex>();
            _cells = new List<ICell>();
        }

        private readonly object _dataInsert;

        /// <summary>
        /// Ordered list of splits that will be successively applied on data content.
        /// In case 
        /// </summary>
        public List<SplitPattern> SplitPatterns { get; set; }

        private List<ICell> _cells;

        public void AddRow(List<String> row)
        {
            if (row == null || row.Count == 0)
            {
                return;
            }
            lock (_dataInsert)
            {
                var newRowIndexValue = _rowIndexes.Count == 0
                    ? 0
                    : _rowIndexes
                    .Where(i => i.GetParent() == null)
                    .Max(i => i.GetIndex()) + 1;
                var newRowIndex = new RowIndex(newRowIndexValue);
                _rowIndexes.Add(newRowIndex);

                var currentMaxColumnIndexValue = _columnIndexes.Count == 0
                    ? -1
                    : _columnIndexes
                    .Where(i => i.GetParent() == null)
                    .Max(i => i.GetIndex());

                // Generate new column indexes if needed.
                for (var n = currentMaxColumnIndexValue + 1; n < row.Count; n++)
                {
                    var columnIndex = new ColumnIndex(n);
                    _columnIndexes.Add(columnIndex);
                }

                // Finally, populate cells.
                for (var n = 0; n < row.Count; n++)
                {
                    var columnIndex = _columnIndexes
                        .Where(i => i.GetParent() == null)
                        .FirstOrDefault(i => i.GetIndex() == n);
                    
                    // By construction, should never happen.
                    if (columnIndex == null)
                    {
                        continue;
                    }

                    var cell = new Cell(row[n], newRowIndex, columnIndex);
                    _cells.Add(cell);
                }
            }
        }
    }
}
