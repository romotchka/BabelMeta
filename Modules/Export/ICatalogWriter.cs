/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using BabelMeta.AppConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelMeta.Modules.Export
{
    public interface ICatalogWriter
    {
        /// <summary>
        /// The Generate method generates a set of output folders and files from CatalogContext.Albums
        /// </summary>
        ReturnCodes Generate(String folder, MainFormViewModel viewModel = null);
    }
}
