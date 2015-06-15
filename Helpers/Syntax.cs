/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using BabelMeta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelMeta.Helpers
{
    public static class Syntax
    {
        public static string HierarchicalSeparator(Lang lang)
        {
            switch (lang.ShortName)
            {
                case "en": return ": ";
                case "fr": return " : ";
                default: return ": ";
            }
        }

        /// <summary>
        /// Expected format is yyyy-mm-dd[...]
        /// Returns DateTime extracted, or Now if an error occurs
        /// </summary>
        /// <param name="datestring"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string datestring)
        {
            string[] words = datestring.Split(new char[] { '-' });

            if (words != null && words.ToList().Count >= 3)
            {
                var year = Convert.ToInt32(words[0]);
                var month = Convert.ToInt32(words[1]);
                var day = Convert.ToInt32(words[2]);

                if (year > 0 && month > 0 && month <= 12 && day > 0 && day <= 31)
                {
                    try
                    {
                        DateTime dateTime = new DateTime(year, month, day);
                        return dateTime;
                    }
                    catch (Exception)
                    {
                        return DateTime.Now;
                    }
                }
            }
            return DateTime.Now;
        }

        public static string GetFileNameFromFullPath(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            var pathElements = path.Split('\\');
            return pathElements[pathElements.Length - 1];
        }

        /// <summary>
        /// Replaces the header by the expected Fuga header
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string WithFugaXmlHeader(this string stream)
        {
            if (string.IsNullOrEmpty(stream))
            {
                return string.Empty;
            }

            // Ensure no double space prevents pattern recognition
            string modifiedStream = stream.Replace("  ", " ");

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
    }
}
