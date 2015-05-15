/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelMeta.Model
{
    public class Work
    {
        public Int32 Id { get; set; }

        public Int32? Parent { get; set; }

        public Int16? MovementNumber { get; set; }

        /// <summary>
        /// The field name is singular because it represents the same entry in different languages available
        /// </summary>
        public Dictionary<Lang, String> Title { get; set; }

        /// <summary>
        /// The field name is singular because it represents the same entry in different languages available
        /// </summary>
        public Dictionary<Lang, String> MovementTitle { get; set; }

        /// <summary>
        /// Work Contributor is e.g. the Composer or Arranger
        /// </summary>
        public Dictionary<Int32, Role> Contributors { get; set; }

        public String ClassicalCatalog { get; set; }

        public Key? Tonality { get; set; }

        public Int16? Year { get; set; }
    }
}
