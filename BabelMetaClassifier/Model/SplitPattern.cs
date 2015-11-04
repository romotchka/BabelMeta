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

namespace BabelMetaClassifier.Model
{
    public class SplitPattern
    {
        private bool _initialized = false;

        /// <summary>
        /// Determines when an external process established, heuristically or deterministically, the right property values. 
        /// </summary>
        public bool Initialized
        {
            get { return _initialized; }
            set { _initialized = value; }
        }

        private String _separator = " ";

        /// <summary>
        /// The separator string pattern for the split.
        /// </summary>
        public String Separator
        {
            get { return _separator; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }
                _separator = value;
            }
        }

        private int _splitOccurrences = 1;

        /// <summary>
        /// n>0: Will perform a maximum of n splits, resulting in n+1 elements.
        /// n=0: Will perform as many splits as possible.
        /// </summary>
        public int SplitOccurrences
        {
            get { return _splitOccurrences; }
            set
            {
                if (value < 0)
                {
                    return;
                }
                _splitOccurrences = value;

                // Strategies are pointless for the 'open' split.
                if (value != 0)
                {
                    return;
                }
                SplitDisambiguationStrategyWhenTooManyValue = SplitDisambiguationStrategyWhenTooMany.Idle;
                SplitDisambiguationStrategyWhenTooFewValue = SplitDisambiguationStrategyWhenTooFew.Idle;
            }
        }

        private SplitDisambiguationStrategyWhenTooMany _splitDisambiguationStrategyWhenTooManyValue = SplitDisambiguationStrategyWhenTooMany.Idle;

        /// <summary>
        /// What should occur when a particular string contains more elements than expected, according to the pattern seeked.
        /// </summary>
        public SplitDisambiguationStrategyWhenTooMany SplitDisambiguationStrategyWhenTooManyValue
        {
            get { return _splitDisambiguationStrategyWhenTooManyValue; }
            set { _splitDisambiguationStrategyWhenTooManyValue = value; }
        }

        private SplitDisambiguationStrategyWhenTooFew _splitDisambiguationStrategyWhenTooFewValue = SplitDisambiguationStrategyWhenTooFew.Idle;

        /// <summary>
        /// What should occur when a particular string contains less elements than expected, according to the pattern seeked.
        /// </summary>
        public SplitDisambiguationStrategyWhenTooFew SplitDisambiguationStrategyWhenTooFewValue
        {
            get { return _splitDisambiguationStrategyWhenTooFewValue; }
            set { _splitDisambiguationStrategyWhenTooFewValue = value; }
        }
    }
    
    public enum SplitDisambiguationStrategyWhenTooMany
    {
        Idle,
        ConcatenateRightElements,
        ConcatenateLeftElements,
        GenerateAll,
    }

    public enum SplitDisambiguationStrategyWhenTooFew
    {
        Idle,
        EmptyStringOnRightElements,
        EmptyStringOnLeftElements,
        Discard,
    }
}
