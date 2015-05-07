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
    public class Work
    {
        public int Id { get; set; }

        public Work Parent { get; set; }

        public int? MovementNumber { get; set; }

        /// <summary>
        /// The field name is singular because it represents the same entry in different languages available
        /// </summary>
        public Dictionary<Lang, String> Title { get; set; }

        /// <summary>
        /// Work Contributor is e.g. the Composer or Arranger
        /// </summary>
        public Dictionary<Artist, Role> Contributors { get; set; }

        public String ClassicalCatalog { get; set; }

        public Key? Key { get; set; }

        public int? Year { get; set; }
    }
}
