﻿/*
 * Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using MetadataConverter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataConverter.Helpers
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
    }
}