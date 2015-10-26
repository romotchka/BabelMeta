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
using System.Runtime.Serialization;
using BabelMeta.Services.DbDriver;

namespace BabelMeta.Model
{
    /// <summary>
    /// Lang represents a spoken language present in metadata.
    /// </summary>
    [Serializable]
    public class Lang : ISerializable
    {
        public Lang()
        {
            LongName = String.Empty;
            ShortName = String.Empty;
            IsDefault = false;
        }

        public Lang(SerializationInfo info, StreamingContext context)
        {
            LongName = (String)info.GetValue("BabelMeta.Model.Lang.LongName", typeof(String));
            ShortName = (String)info.GetValue("BabelMeta.Model.Lang.ShortName", typeof(String));
            IsDefault = (bool)info.GetValue("BabelMeta.Model.Lang.IsDefault", typeof(bool));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Lang.LongName", LongName);
            info.AddValue("BabelMeta.Model.Lang.ShortName", ShortName);
            info.AddValue("BabelMeta.Model.Lang.IsDefault", IsDefault);
        }

        /// <summary>
        /// Full language name, informative.
        /// </summary>
        [DbField(MaxSize = 128)]
        public String LongName { get; set; }

        /// <summary>
        /// ISO 639-1 2-character language abbreviation, used as unique Id.
        /// </summary>
        [DbField(MaxSize = 4)]
        public String ShortName { get; set; }

        /// <summary>
        /// Determines whether that language, application-wise, is the default one (e.g. for output data generation).
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
