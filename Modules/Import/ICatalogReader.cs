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
using System.Windows.Forms;

namespace BabelMeta.Modules.Import
{
    public interface ICatalogReader
    {
        /// <summary>
        /// The Parse method refreshes the CatalogContext according to the Input data
        /// </summary>
        ReturnCodes Parse(OpenFileDialog ofd, string formatType, MainFormViewModel viewModel = null);
    }
}
