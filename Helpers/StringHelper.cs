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

using BabelMeta.Model;
using System;
using System.Linq;
using BabelMeta.Modules;

namespace BabelMeta.Helpers
{
    /// <summary>
    /// Syntax provides with String-related extension methods and functions
    /// </summary>
    public static class StringHelper
    {
        public static MainFormViewModel ViewModel { get; set; }

        #region Typography
        /// <summary>
        /// Language-dependent separator between an entity and a sub-entity.
        /// </summary>
        public static String HierarchicalSeparator(Lang lang)
        {
            switch (lang.ShortName)
            {
                case "en": return ": ";
                case "fr": return " : ";
                default: return ": ";
            }
        }

        public static String ToCurlySimpleQuotes(this String s)
        {
            return String.IsNullOrEmpty(s) 
                ? String.Empty 
                : s.Replace('\'', '’');
        }

        /// <summary>
        /// This replacement implies an even number of double quotes so as to perform consistent action.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String ToCurlyDoubleQuotes(this String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.Empty;
            }
            var chars = s.ToCharArray().ToList();
            var testEven = chars.Count(c => c == '"') % 2 == 0;
            if (!testEven)
            {
                return s;
            }
            var evenness = true;
            foreach (var i in Enumerable.Range(0,chars.Count).Where(k => chars[k] == '"'))
            {
                chars[i] = evenness
                    ? '“'
                    : '”';
                evenness = !evenness;
            }
            return s;
        }

        public static String DoubleSpacesRecursiveRemoval(this String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.Empty;
            }
            return s.Contains("  ")
                ? s.Replace("  ", " ").DoubleSpacesRecursiveRemoval()
                : s;
        }

        /// <summary>
        /// Apply the different typographic rules active according to main view model.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String Typo(this String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.Empty;
            }
            if (ViewModel == null)
            {
                return s;
            }
            var o = s;
            if (ViewModel.CurlySimpleQuotesActive) o = o.ToCurlySimpleQuotes();
            if (ViewModel.CurlyDoubleQuotesActive) o = o.ToCurlyDoubleQuotes();
            if (ViewModel.DoubleSpacesRemovalActive) o = o.DoubleSpacesRecursiveRemoval();
            return o;
        }
        #endregion

        /// <summary>
        /// Expected format is yyyy-mm-dd[...]
        /// Returns DateTime extracted, or Now if an error occurs
        /// </summary>
        public static DateTime ToDateTime(this String dateString)
        {
            var words = dateString.Split(new[] { '-' });

            if (words.ToList().Count >= 3)
            {
                var year = Convert.ToInt32(words[0]);
                var month = Convert.ToInt32(words[1]);
                var day = Convert.ToInt32(words[2]);

                if (year <= 0 || month <= 0 || month > 12 || day <= 0 || day > 31)
                {
                    return DateTime.Now;
                }
                try
                {
                    var dateTime = new DateTime(year, month, day);
                    return dateTime;
                }
                catch (Exception)
                {
                    return DateTime.Now;
                }
            }
            return DateTime.Now;
        }

        /// <summary>
        /// Removes path from file full name and returns filename only.
        /// </summary>
        /// <param name="path"></param>
        public static String GetFileNameFromFullPath(this String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return String.Empty;
            }
            var pathElements = path.Split('\\');
            return pathElements[pathElements.Length - 1];
        }

        /// <summary>
        /// Replaces the default header by the expected Fuga header (not properly provided by Xsd2Code-generated ingestion.designer.cs).
        /// </summary>
        public static String WithFugaXmlHeader(this String stream)
        {
            if (String.IsNullOrEmpty(stream))
            {
                return String.Empty;
            }

            // Ensure no double space prevents pattern recognition
            var modifiedStream = stream.Replace("  ", " ");

            modifiedStream = modifiedStream.Replace(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                ,"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n");

            modifiedStream = modifiedStream.Replace(
                "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\""
                ,"xsi:noNamespaceSchemaLocation=\"http://fugamusic.com/docs/ingestion/ingestion.xsd\"");

            modifiedStream = modifiedStream.Replace(
                "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\""
                , "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");

            // For readability only
            modifiedStream = modifiedStream.Replace(
                "><"
                , ">\r\n<");

            return modifiedStream;
        }

        /// <summary>
        /// Converts a String into a file format supported in import/export modules.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static FileFormatType ToFileFormatType(this String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return FileFormatType.Unknown;
            }
            s = s.ToLower();
            switch (s)
            {
                case "excel workbook": return FileFormatType.ExcelWorkbook;
                case "excel xml 2003": return FileFormatType.ExcelXml2003;
                default: return FileFormatType.Unknown;
            }
        }
    }
}
