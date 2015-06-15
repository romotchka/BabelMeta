using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelMeta.Modules.Import
{
    public class SmartCatalogReader : ICatalogReader
    {
        Task<AppConfig.ReturnCodes> ICatalogReader.Parse(System.Windows.Forms.OpenFileDialog ofd, string formatType, MainFormViewModel viewModel)
        {
            throw new NotImplementedException();
        }
    }
}
