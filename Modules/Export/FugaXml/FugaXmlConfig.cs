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

namespace BabelMeta.Modules.Export.FugaXml
{
    public static class FugaXmlConfig
    {
        /// <summary>
        /// Removes or not the '-' characters in an ISRC string.
        /// </summary>
        public static readonly bool RemoveIsrcDashes = true;

        /// <summary>
        /// Default Fuga value for recording location.
        /// </summary>
        public static readonly Dictionary<String, String> DefaultRecordingLocationLabel = new Dictionary<string, string>
        {
            {"en", "Unknown"},
            {"fr", "Inconnu"},
        };

        public static String ToFugaFormattedIsrc(this String isrc)
        {
            if (String.IsNullOrEmpty(isrc))
            {
                return String.Empty;
            }
            return (RemoveIsrcDashes)
                ? isrc.Replace("-", "")
                : isrc;
        }
    }
}
