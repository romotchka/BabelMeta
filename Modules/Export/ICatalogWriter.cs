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
        ReturnCodes Generate(object context, MainFormViewModel viewModel = null);
    }
}
