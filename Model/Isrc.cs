/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using BabelMeta.Model.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelMeta.Model
{
    public class Isrc
    {
        public String Id { get; set; }

        public Int32 Work { get; set; }

        /// <summary>
        /// Isrc Contributor is e.g. the Performer or Musical Director or Sound Engineer
        /// </summary>
        public Dictionary<Int32, Dictionary<Role, Quality>> Contributors { get; set; }

        public String CName { get; set; }

        public Int16? CYear { get; set; }

        public String PName { get; set; }

        public Int16? PYear { get; set; }

        public String RecordingLocation { get; set; }

        public Int16? RecordingYear { get; set; }

        public bool AvailableSeparately { get; set; }

        public CatalogTier Tier { get; set; }
    }
}
