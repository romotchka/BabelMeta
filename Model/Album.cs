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
    public class Album
    {
        public int Id { get; set; }

        public long Ean { get; set; }

        /// <summary>
        /// The field name is singular because it represents the same entry in different languages available
        /// </summary>
        public Dictionary<Lang, String> Title { get; set; }

        public List<Tag> Genres { get; set; }

        public List<Tag> SubGenres { get; set; }

        public String Owner { get; set; }

        public String CatalogReference { get; set; }

        public int CopyrightPYear { get; set; }

        public String CopyrightPLabel { get; set; }

        public int CopyrightCYear { get; set; }

        public String CopyrightCLabel { get; set; }

        /// <summary>
        /// Nested Dictionaries correspond to Volume index & Track index
        /// </summary>
        public Dictionary<int, Dictionary<int, Isrc>> Assets { get; set; }
    }
}
