/*
 * Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataConverter.Modules
{
    public static class OfficeXml
    {
        public const string Worksheet = "ss:Worksheet";
        public const string WorksheetName = "ss:Name";
        public const string WorksheetTable = "Table";
        public const string WorksheetRow = "Row";
        public const string WorksheetCell = "Cell";

        public const string CellType = "ss:Type";
        public const string CellTypeString = "String";
        public const string CellIndex = "ss:Index";
    }
}
