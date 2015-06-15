/*
 * Babel Meta - babelmeta.com
 * Romain Carbou (romain@babelmeta.com)
 * This is licensed software.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelMeta.AppConfig
{
    public enum ReturnCodes
    {
        Ok,

        ModulesImportDefaultParseEmptyStream,
        ModulesImportDefaultParseUnknownFormat,
        ModulesImportDefaultParseInvalidWorkbook,

        ModulesExportCatalogContextNotInitialized,

        ModulesExportFugaXmlGenerateNullFolderName,

    }
}
