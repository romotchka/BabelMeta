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
    public class Isrc
    {
        public String Id { get; set; }

        public Int32 Work { get; set; }

        /// <summary>
        /// Isrc Contributor is e.g. the Performer or Musical Director or Sound Engineer
        /// </summary>
        public Dictionary<Int32, Dictionary<Role, Quality>> Contributors { get; set; }
    }
}
