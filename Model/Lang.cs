/*
 * Classical Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataConverter.Model
{
    public class Lang
    {
        public String LongName { get; set; }

        public String ShortName { get; set; }

        public bool IsDefault { get; set; }
    }
}
