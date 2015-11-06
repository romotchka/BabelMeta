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

using System.Collections.Generic;

namespace BabelMetaClassifier.Model
{
    public interface IColumnIndex
    {
        /// <summary>
        /// Column index or sub-index.
        /// </summary>
        /// <returns></returns>
        int Index { get; set; }

        /// <summary>
        /// Column parent if any.
        /// </summary>
        /// <returns></returns>
        IColumnIndex Parent { get; set; }

        /// <summary>
        /// 0 if the column index has no container.
        /// Otherwise, 1 + container's depth.
        /// </summary>
        /// <returns></returns>
        int Depth { get; }

        /// <summary>
        /// Object-wise copy of parent data set's split patterns, since dynamic split occurrences are tailored to each column.
        /// </summary>
        List<SplitPattern> ColumnSplitPatterns { get; set; }

        /// <summary>
        /// Determines whether the current index has the given value in its chain of ancestors.
        /// </summary>
        /// <param name="ancestor"></param>
        /// <returns></returns>
        bool OwnsAsAncestor(IColumnIndex ancestor);

        /// <summary>
        /// Makes an object-wise copy of the list of split patterns from parent data set.
        /// </summary>
        /// <param name="splitPatterns"></param>
        void CopySplitPatterns(List<SplitPattern> splitPatterns);
    }
}
