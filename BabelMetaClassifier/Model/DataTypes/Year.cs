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

namespace BabelMetaClassifier.Model.DataTypes
{
    public class Year : GenericDataType
    {
        /// <summary>
        /// Nullable int for Year. Null value stands for Year not provided/not applicable.
        /// </summary>
        public Year()
        {
            NativeType = typeof(int?);
        }

        private int _minLikelyYear = DateTime.Now.Year - 500;

        /// <summary>
        /// Minimum of the time window year inside which likelihood is 1.0.
        /// Taken for usual dates from middle ages to nowadays.
        /// </summary>
        public int MinLikelyYear 
        {
            get
            {
                return _minLikelyYear;
                
            }
            set
            {
                if (value < MinPossibleYear)
                {
                    MinPossibleYear = value;
                }
                if (value > MaxLikelyYear)
                {
                    MaxLikelyYear = value;
                }
                _minLikelyYear = value;
            }
        }

        private int _maxLikelyYear = DateTime.Now.Year + 5;

        /// <summary>
        /// Maximum of the time window year inside which likelihood is 1.0.
        /// Taken for usual dates from nowadays to short-term future.
        /// </summary>
        public int MaxLikelyYear
        {
            get
            {
                return _maxLikelyYear;
            }
            set
            {
                if (value < MinLikelyYear)
                {
                    MinLikelyYear = value;
                }
                if (value > MaxPossibleYear)
                {
                    MaxPossibleYear = value;
                }
                _maxLikelyYear = value;
            }
        }

        private int _minPossibleYear = DateTime.Now.Year - 2000;

        /// <summary>
        /// Minimum of possible year value.
        /// Taken for usual dates from common era to nowadays.
        /// </summary>
        public int MinPossibleYear
        {
            get
            {
                return _minPossibleYear;
            }
            set
            {
                if (value > MinLikelyYear)
                {
                    MinLikelyYear = value;
                }
                _minPossibleYear = value;
            }
        }

        private int _maxPossibleYear = DateTime.Now.Year + 20;

        /// <summary>
        /// Maximum of possible year value.
        /// Taken for usual dates from nowadays to mid-term future.
        /// </summary>
        public int MaxPossibleYear
        {
            get
            {
                return _maxPossibleYear;

            }
            set
            {
                if (value < MaxLikelyYear)
                {
                    MaxLikelyYear = value;
                }
                _maxPossibleYear = value;
            }
        }

        public override double TypeLikelihoodFor(String s)
        {
            // Allow null value as half valid (can be actually anything).
            if (String.IsNullOrEmpty(s))
            {
                return 0.5;
            }

            int n;
            try
            {
                n = Convert.ToInt32(s.Trim());
            }

            catch (FormatException)
            {
                return 0.0;
            }

            catch (OverflowException)
            {
                return 0.0;
            }

            if (n <= _minPossibleYear)
            {
                return 0.0;
            }

            if (n > _minPossibleYear && n < _minLikelyYear)
            {
                // Quarter ellipse curve.
                return 1.0 - Math.Sqrt(
                    ((double)((n - _minPossibleYear) * (n - _minPossibleYear)))
                    / 
                    ((double)((_minLikelyYear - _minPossibleYear) * (_minLikelyYear - _minPossibleYear)))
                    );
            }

            if (n >= _minLikelyYear && n <= _maxLikelyYear)
            {
                return 1.0;
            }

            if (n > _maxLikelyYear && n < _maxPossibleYear)
            {
                // Quarter ellipse curve.
                return 1.0 - Math.Sqrt(
                    ((double)((n - _maxLikelyYear) * (n - _maxLikelyYear)))
                    /
                    ((double)((_maxPossibleYear - _maxLikelyYear) * (_maxPossibleYear - _maxLikelyYear)))
                    );
            }

            // n >= _maxPossibleYear
            return 0.0;
        }
    }
}
