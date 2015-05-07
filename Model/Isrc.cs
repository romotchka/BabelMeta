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

        public Work Work { get; set; }

        /// Isrc Contributor is e.g. the Performer or Musical Director or Sound Engineer
        public Dictionary<Artist, Role> Contributors { get; set; }
    }
}
