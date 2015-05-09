/*
 * Metadata Converter
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
    public class Album
    {
        public Int32 Id { get; set; }

        public Int64? Ean { get; set; }

        /// <summary>
        /// The field name is singular because it represents the same entry in different languages available
        /// </summary>
        public Dictionary<Lang, String> Title { get; set; }

        public Tag Genre { get; set; }

        public Tag Subgenre { get; set; }

        public String Owner { get; set; }

        public String CatalogReference { get; set; }

        public Int16 CopyrightPYear { get; set; }

        public String CopyrightPLabel { get; set; }

        public Int16 CopyrightCYear { get; set; }
        
        public String CopyrightCLabel { get; set; }

        /// <summary>
        /// Nested Dictionaries correspond to Volume index & Track index.
        /// The String value is an Isrc id.
        /// </summary>
        public Dictionary<Int16, Dictionary<Int16, String>> Assets { get; set; }
    }
}
