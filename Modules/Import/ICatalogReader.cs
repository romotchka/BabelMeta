/*
 * Classical Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataConverter.Modules.Import
{
    public interface ICatalogReader
    {
        /// <summary>
        /// The Parse method refreshes the CatalogContext according to the Input data
        /// </summary>
        void Parse(Stream s, MainFormViewModel viewModel = null);
    }
}
