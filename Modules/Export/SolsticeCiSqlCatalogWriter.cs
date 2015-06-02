/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using BabelMeta.AppConfig;
using BabelMeta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelMeta.Modules.Export
{
    public class SolsticeCiSqlCatalogWriter : ICatalogWriter
    {
        MainFormViewModel _viewModel = null;

        private string _sqlOutput;

        public AppConfig.ReturnCodes Generate(object context, MainFormViewModel viewModel = null)
        {
            _viewModel = viewModel;

            if (!CatalogContext.Instance.Initialized)
            {
                return ReturnCodes.ModulesExportCatalogContextNotInitialized;
            }

            _sqlOutput = string.Empty;

            GenerateLangs();

            throw new NotImplementedException();
        }

        private void GenerateLangs()
        {
            _sqlOutput += "# Langs \n\r";
            foreach (var lang in CatalogContext.Instance.Langs)
            {

            }
        }
    }
}
