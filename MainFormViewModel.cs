/*
 * Classical Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataConverter
{
    public class MainFormViewModel : INotifyPropertyChanged
    {
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

        public bool CheckedPictoVisibility { get; set; }

        public bool WarningPictoVisibility { get; set; }


        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> for the specified property.
        /// </summary>
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
