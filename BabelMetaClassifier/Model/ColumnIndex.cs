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
    public class ColumnIndex : IColumnIndex
    {
        public ColumnIndex(int index, IDataSet mainContainer = null, IColumnIndex parent = null)
        {
            Index = index;
            Parent = parent;
            MainContainer = mainContainer;
        }

        #region Properties
        public int Index { get; set; }

        public IColumnIndex Parent { get; set; }

        public IDataSet MainContainer { get; set; }

        public int Depth
        {
            get
            {
                return Parent == null
                    ? 0
                    : 1 + Parent.Depth;
            }
        }
        #endregion

        #region Methods
        public List<SplitPattern> ColumnSplitPatterns { get; set; }

        public bool OwnsAsAncestor(IColumnIndex ancestor)
        {
            if (Parent == ancestor)
            {
                return true;
            }
            return Parent != null && Parent.OwnsAsAncestor(ancestor);
        }

        public void CopySplitPatterns(List<SplitPattern> splitPatterns)
        {
            if (splitPatterns == null)
            {
                return;
            }
            
            ColumnSplitPatterns = new List<SplitPattern>();
            splitPatterns.ForEach(p => ColumnSplitPatterns.Add(new SplitPattern(
                this, 
                p.SplitOccurrences)
            {
                DynamicSplitOccurrences = p.DynamicSplitOccurrences,
                Separator = p.Separator,
                SplitDisambiguationStrategyWhenTooFewValue = p.SplitDisambiguationStrategyWhenTooFewValue,
                SplitDisambiguationStrategyWhenTooManyValue = p.SplitDisambiguationStrategyWhenTooManyValue,
            }));
        }
        #endregion
    }
}
