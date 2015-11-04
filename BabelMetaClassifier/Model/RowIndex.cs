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

namespace BabelMetaClassifier.Model
{
    public class RowIndex : IRowIndex
    {
        private double _weight = 1.0;

        /// <summary>
        /// Relative weight of the row compared to other rows.
        /// E.g. in the case where the row creation resulted from ambiguous split occurrences. 
        /// </summary>
        public double Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        private int _index;

        private IRowIndex _parent;

        public RowIndex(int index, IRowIndex parent = null)
        {
            _index = index;
            _parent = parent;

        }

        public int GetIndex()
        {
            return _index;
        }

        public IRowIndex GetParent()
        {
            return _parent;
        }

        public int GetDepth()
        {
            return _parent == null
                ? 0
                : 1 + _parent.GetDepth();
        }
    }
}
