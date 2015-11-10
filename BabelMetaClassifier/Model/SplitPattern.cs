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

namespace BabelMetaClassifier.Model
{
    public class SplitPattern
    {
        /// <summary>
        /// Determines when an external process established, heuristically or deterministically, the right property values. 
        /// </summary>
        public bool Initialized { get; set; }

        public SplitPattern()
        {
            Initialized = false;
        }

        private bool _dynamicSplitOccurrences = true;

        /// <summary>
        /// If true, SplitOccurrences will be set heuristically from parent DataSet parsing.
        /// </summary>
        public bool DynamicSplitOccurrences
        {
            get { return _dynamicSplitOccurrences; }
            set { _dynamicSplitOccurrences = value; }
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
        /// n >= 0 -> Will perform a maximum of n splits (resulting in n+1 strings).
        /// For n=0, the action leaves the string 'as is'.
        /// </summary>
        public int SplitOccurrences
        {
            get { return _splitOccurrences; }
            set
            {
                Initialized = true;
                if (value < 0)
                {
                    return;
                }
                _splitOccurrences = value;
            }
        }

        private StringSplitOptions _patternSplitOptions = StringSplitOptions.RemoveEmptyEntries;

        /// <summary>
        /// The parameters for the native Split method. Should be carefully setup depending on the split pattern.
        /// E.g. for a " " (blank) separator, removing double spaces is likely to be a common strategy.
        /// E.g. for a "-" (dash) separator, it is conversely more appropriate to leave each dash, even if it generates empty strings.
        /// </summary>
        public StringSplitOptions PatternSplitOptions
        {
            get { return _patternSplitOptions; }
            set { _patternSplitOptions = value; }
        }

        private SplitDisambiguationStrategyWhenTooMany _splitDisambiguationStrategyWhenTooManyValue = SplitDisambiguationStrategyWhenTooMany.GenerateAll;

        /// <summary>
        /// What should occur when a particular string contains more elements than expected, according to the pattern seeked.
        /// </summary>
        public SplitDisambiguationStrategyWhenTooMany SplitDisambiguationStrategyWhenTooManyValue
        {
            get { return _splitDisambiguationStrategyWhenTooManyValue; }
            set { _splitDisambiguationStrategyWhenTooManyValue = value; }
        }

        private SplitDisambiguationStrategyWhenTooFew _splitDisambiguationStrategyWhenTooFewValue = SplitDisambiguationStrategyWhenTooFew.GenerateAll;

        /// <summary>
        /// What should occur when a particular string contains less elements than expected, according to the pattern seeked.
        /// </summary>
        public SplitDisambiguationStrategyWhenTooFew SplitDisambiguationStrategyWhenTooFewValue
        {
            get { return _splitDisambiguationStrategyWhenTooFewValue; }
            set { _splitDisambiguationStrategyWhenTooFewValue = value; }
        }

        private SplitDisambiguationStrategyWhenMultipleMaxCardinalities _splitDisambiguationStrategyWhenMultipleMaxCardinalitiesValue
            = SplitDisambiguationStrategyWhenMultipleMaxCardinalities.KeepGreatest;

        /// <summary>
        /// This strategy parameter is used exclusively in a dynamic context, when the max of split cardinalities is reached for several subgroups of cells.
        /// KeepGreatest will lead to keep the greatest cardinality value.
        /// KeepSmallest will lead to keep the smallest cardinality value.
        /// Example for the following column:
        /// 
        /// "bla bla bla"
        /// "toc toc toc toc toc"
        /// "bing bang bong"
        /// "there are more words in this one huh?"
        /// "the end of the example"
        /// 
        /// 2 splits appear 2 times
        /// 4 splits appear 2 times
        /// 7 splits appear 1 time
        /// 
        /// If KeepGreatest -> 4
        /// If KeepSmallest -> 2
        /// </summary>
        public SplitDisambiguationStrategyWhenMultipleMaxCardinalities SplitDisambiguationStrategyWhenMultipleMaxCardinalitiesValue
        {
            get { return _splitDisambiguationStrategyWhenMultipleMaxCardinalitiesValue; }
            set { _splitDisambiguationStrategyWhenMultipleMaxCardinalitiesValue = value; }
        }
    }
    
    public enum SplitDisambiguationStrategyWhenTooMany
    {
        ConcatenateRightElements,
        ConcatenateLeftElements,
        GenerateAll,
    }

    public enum SplitDisambiguationStrategyWhenTooFew
    {
        EmptyStringOnRightElements,
        EmptyStringOnLeftElements,
        GenerateAll,
    }

    public enum SplitDisambiguationStrategyWhenMultipleMaxCardinalities
    {
        KeepGreatest,
        KeepSmallest,
    }
}
