/*
 * Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using MetadataConverter.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataConverter.Modules.Export
{
    public interface ICatalogWriter
    {
        /// <summary>
        /// The Generate method generates a set of output folders and files from CatalogContext.Albums
        /// </summary>
        ReturnCodes Generate(String folder, MainFormViewModel viewModel = null);
    }
}
