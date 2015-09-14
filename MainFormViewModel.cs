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
using System.ComponentModel;
using System.Windows.Threading;

namespace BabelMeta
{
    /// <summary>
    /// Application main window view model
    /// </summary>
    public class MainFormViewModel : INotifyPropertyChanged
    {
        #region ProgressBars
        private int _inputProgressBarMax = 100;

        /// <summary>
        /// Number of function calls *to come* at the beginning of a long-run operation using the InputProgressBar 
        /// </summary>
        public int InputProgressBarMax 
        {
            get { return _inputProgressBarMax; }
            set
            {
                if (_inputProgressBarMax == value)
                {
                    return;
                }
                _inputProgressBarMax = value;
                RaisePropertyChanged("InputProgressBarMax");
            }
        }

        private int _inputProgressBarValue = 0;

        /// <summary>
        /// Input progress bar value
        /// </summary>
        public int InputProgressBarValue 
        {
            get { return _inputProgressBarValue; }
            set
            {
                if (_inputProgressBarValue == value)
                {
                    return;
                }
                _inputProgressBarValue = value;
                RaisePropertyChanged("InputProgressBarValue");
                if (_inputProgressBarValue == 0)
                {
                    WarningPictoVisibility = false;
                    CheckedPictoVisibility = false;
                }
                else
                if (_inputProgressBarValue == _inputProgressBarMax)
                {
                    WarningPictoVisibility = false;
                    CheckedPictoVisibility = true;
                }
            }
        }

        private int _outputProgressBarMax = 100;

        /// <summary>
        /// Number of function calls *to come* at the beginning of a long-run operation using the OutputProgressBar 
        /// </summary>
        public int OutputProgressBarMax 
        {
            get { return _outputProgressBarMax; }
            set
            {
                if (_outputProgressBarMax == value)
                {
                    return;
                }
                _outputProgressBarMax = value;
                RaisePropertyChanged("OutputProgressBarMax");
            }
        }

        private int _outputProgressBarValue = 0;

        /// <summary>
        /// Output progress bar value
        /// </summary>
        public int OutputProgressBarValue 
        {
            get { return _outputProgressBarValue; }
            set
            {
                if (_outputProgressBarValue == value)
                {
                    return;
                }
                _outputProgressBarValue = value;
                RaisePropertyChanged("OutputProgressBarValue");
            }
        }
        #endregion

        private bool _checkedPictoVisibility = false;

        # region Pictos
        public bool CheckedPictoVisibility 
        {
            get { return _checkedPictoVisibility; } 
            set
            {
                if (_checkedPictoVisibility == value)
                {
                    return;
                }
                _checkedPictoVisibility = value;
                RaisePropertyChanged("CheckedPictoVisibility");
            }
        }

        private bool _warningPictoVisibility = false;

        public bool WarningPictoVisibility
        {
            get { return _warningPictoVisibility; }
            set
            {
                if (_warningPictoVisibility == value)
                {
                    return;
                }
                _warningPictoVisibility = value;
                RaisePropertyChanged("WarningPictoVisibility");
            }
        }
        #endregion

        #region Filters
        private bool _filterArtistChecked = false;

        /// <summary>
        /// State of the filter for artists used in no album
        /// </summary>
        public bool FilterArtistChecked
        {
            get { return _filterArtistChecked; }
            set
            {
                if (_filterArtistChecked == value)
                {
                    return;
                }
                _filterArtistChecked = value;
                RaisePropertyChanged("FilterArtistChecked");
            }
        }

        private bool _filterWorkChecked = false;

        /// <summary>
        /// State of the filter for works used in no album
        /// </summary>
        public bool FilterWorkChecked
        {
            get { return _filterWorkChecked; }
            set
            {
                if (_filterWorkChecked == value)
                {
                    return;
                }
                _filterWorkChecked = value;
                RaisePropertyChanged("FilterWorkChecked");
            }
        }
        #endregion

        #region Typography
        public bool CurlySimpleQuotesActive { get; set; }

        public bool CurlyDoubleQuotesActive { get; set; }

        public bool DoubleSpacesRemovalActive { get; set; }
        #endregion

        private String _notification = String.Empty;

        /// <summary>
        /// Notification string to be published on the main form dedicated are. The property changed event must be always fired (identic notifications can be displayed 'in a row').
        /// </summary>
        public String Notification
        {
            get { return _notification; }
            set
            {
                _notification = value;
                RaisePropertyChanged("Notification");
            }
        }

        public Dispatcher MainFormDispatcher { get; set; }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> for the specified property.
        /// </summary>
        protected void RaisePropertyChanged(String propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
