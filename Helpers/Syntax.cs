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
        public static String HierarchicalSeparator(Lang lang)
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
        /// <param name="dateString"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this String dateString)
        {
            string[] words = dateString.Split(new char[] { '-' });

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

    }
}
