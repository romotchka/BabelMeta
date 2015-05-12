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

namespace MetadataConverter.Model.Config
{
    /// <summary>
    /// Default values
    /// </summary>
    public class CatalogSettings
    {
        public String SupplierDefault { get; set; }

        public String LabelDefault { get; set; }

        public String COwnerDefault { get; set; }

        public String POwnerDefault { get; set; }

        public CatalogTier CatalogTierDefault { get; set; }

        public String MainGenreDefault { get; set; }

        public ProductFormat FormatDefault { get; set; }

        public bool AvailableSeparatelyDefault { get; set; }

        // TODO: Complete with default behaviours
    }
}
