/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using BabelMeta.AppConfig;
using BabelMeta.Helpers;
using BabelMeta.Model;
using BabelMeta.Model.Config;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;

namespace BabelMeta.Modules.Import
{
    /// <summary>
    /// This default Input format consists in a Excel 2003 XML export of worksheets:
    /// lang, role, tag, artist, work, isrc, featuring, album, asset
    /// </summary>
    public class DefaultCatalogReader : ICatalogReader
    {
        /// <summary>
        /// Excel format main object
        /// </summary>
        private Excel.Application _excelApplication;
        private Workbook _excelDocument;
        private Dictionary<String, Int32> _excelLastRow; // Worksheet-wise
        private Dictionary<String, Int32> _excelLastColumn; // Worksheet-wise

        /// <summary>
        /// Xml format main object
        /// </summary>
        private XmlDocument _xmlDocument;

        /// <summary>
        /// 0: Excel XML 2003
        /// 1: Excel Workbook .xlsx
        /// </summary>
        private String _formatType;

        private MainFormViewModel _mainFormViewModel;

        private const int _initPayload = 7; // Conventional value reflecting the init payload - feel free to put any reasonable value

        /// <summary>
        /// Legacy convention for tonalities
        /// </summary>
        private readonly Dictionary<String, Key> _shortenedKeys =
            new Dictionary<String, Key>()
            {
                {"Ab", Key.AFlatMajor},
                {"A", Key.AMajor},
                {"a", Key.AMinor},
                {"Bb", Key.BFlatMajor},
                {"bb", Key.BFlatMinor},
                {"B", Key.BMajor},
                {"b", Key.BMinor},
                {"C", Key.CMajor},
                {"c", Key.CMinor},
                {"c#", Key.CSharpMinor},
                {"Db", Key.DFlatMajor},
                {"D", Key.DMajor},
                {"d", Key.DMinor},
                {"d#", Key.DSharpMinor},
                {"Eb", Key.EFlatMajor},
                {"eb", Key.EFlatMinor},
                {"E", Key.EMajor},
                {"e", Key.EMinor},
                {"F", Key.FMajor},
                {"f", Key.FMinor},
                {"F#", Key.FSharpMajor},
                {"f#", Key.FSharpMinor},
                {"Gb", Key.GFlatMajor},
                {"G", Key.GMajor},
                {"g", Key.GMinor},
                {"g#", Key.GSharpMinor},
            };

        /// <summary>
        /// Worksheet nodes
        /// </summary>
        private Dictionary<String, object> _worksheets;

        /// <summary>
        /// Column indexes by worksheets (permits flexibility in the worksheet structure)
        /// </summary>
        private Dictionary<String, Dictionary<String, Int32>> _worksheetColumns;

        private List<object> _settings;
        private List<object> _langs;
        private List<object> _tags;
        private List<object> _roles;
        private List<object> _qualities;
        private List<object> _artists;
        private List<object> _works;
        private List<object> _isrcs;
        private List<object> _albums;
        private List<object> _assets;

        private static DefaultCatalogReader _instance;

        private DefaultCatalogReader()
        {
            _worksheets = new Dictionary<String, object>();
            _worksheetColumns = new Dictionary<String, Dictionary<String, Int32>>();
        }

        public static DefaultCatalogReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DefaultCatalogReader();

                }
                return _instance;
            }
        }

        public ReturnCodes Parse(OpenFileDialog ofd, String formatType, MainFormViewModel viewModel = null)
        {
            if (ofd == null)
            {
                return ReturnCodes.ModulesImportDefaultParseEmptyStream;
            }

            if (String.IsNullOrEmpty(formatType))
            {
                return ReturnCodes.ModulesImportDefaultParseUnknownFormat;
            }

            formatType = formatType.ToLower();
            
            if (formatType.CompareTo("xml") == 0 || formatType.CompareTo("excel") == 0)
            {
                _formatType = formatType;
            }
            else 
            {
                return ReturnCodes.ModulesImportDefaultParseUnknownFormat;
            }

            switch (_formatType)
            {
                case "excel":
                    _excelLastRow = new Dictionary<String, Int32>();
                    _excelLastColumn = new Dictionary<String, Int32>();
                    if (String.IsNullOrEmpty(ofd.FileName))
                    {
                        return ReturnCodes.ModulesImportDefaultParseEmptyStream;
                    }
                    _excelApplication = new Excel.Application();
                    _excelDocument = _excelApplication.Workbooks.Open(ofd.FileName);
                    if (_excelDocument == null)
                    {
                        return ReturnCodes.ModulesImportDefaultParseEmptyStream;
                    }
                    break;
                case "xml":
                    Stream s = ofd.OpenFile();
                    if (s == null)
                    {
                        return ReturnCodes.ModulesImportDefaultParseEmptyStream;
                    }
                    _xmlDocument = new XmlDocument();
                    _xmlDocument.Load(s);
                    break;
            }

            if (viewModel != null)
            {
                _mainFormViewModel = viewModel;
                _mainFormViewModel.InputProgressBarValue = 0;
                _mainFormViewModel.InputProgressBarMax = _initPayload + 1; // initial payload and 'symbolic' last bit of payload before last return 
            }

            if (!IsValidWorkbook())
            {
                return ReturnCodes.ModulesImportDefaultParseInvalidWorkbook;
            }

            if (_mainFormViewModel != null)
            {
                if (_langs != null) _mainFormViewModel.InputProgressBarMax += _langs.Count;
                if (_tags != null) _mainFormViewModel.InputProgressBarMax += _tags.Count;
                if (_roles != null) _mainFormViewModel.InputProgressBarMax += _roles.Count;
                if (_qualities != null) _mainFormViewModel.InputProgressBarMax += _qualities.Count;
                if (_artists != null) _mainFormViewModel.InputProgressBarMax += _artists.Count;
                if (_works != null) _mainFormViewModel.InputProgressBarMax += _works.Count;
                if (_isrcs != null) _mainFormViewModel.InputProgressBarMax += _isrcs.Count;
                if (_albums != null) _mainFormViewModel.InputProgressBarMax += _albums.Count;
                if (_assets != null) _mainFormViewModel.InputProgressBarMax += _assets.Count;

                // Init phase 'completed'
                _mainFormViewModel.InputProgressBarValue = _initPayload;

                // Langs were already parsed
                if (_langs != null) _mainFormViewModel.InputProgressBarValue += _langs.Count;

            }

            ParseTags();
            ParseRoles();
            ParseQualities();
            ParseArtists();
            ParseWorks();
            ParseIsrcs();
            ParseAlbums();
            ParseAssets();

            // Finalization
            FinalizeAlbums();
            FinalizeIsrcs();

            CatalogContext.Instance.Initialized = true;
            _mainFormViewModel.InputProgressBarValue = _mainFormViewModel.InputProgressBarMax;
            return ReturnCodes.Ok;
        }

        private Dictionary<Int32,object> GetHeader(String worksheetName)
        {
            //if (String.IsNullOrEmpty(worksheetName))
            //{
            //    return null;
            //}

            Dictionary<Int32, object> map = null;
            switch (_formatType)
            {
                case "excel":
                    map = CellMapByRow(1, worksheetName);
                    break;
                case "xml":
                    if ((XmlNode)(_worksheets[worksheetName]) == null)
                    { 
                        return null; 
                    }
                    map = CellMapByRow(((XmlNode)(_worksheets[worksheetName])).ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
                    break;
            }
            return map;
        }

        /// <summary>
        /// Checks that Workbook embarks expected Worksheets
        /// </summary>
        /// <returns></returns>
        private bool IsValidWorkbook()
        {
            //if (String.IsNullOrEmpty(_formatType))
            //{
            //    return false;
            //}

            if  (
                    (_formatType.CompareTo("xml") == 0 && (_xmlDocument == null || _xmlDocument.DocumentElement == null))
                    || (_formatType.CompareTo("excel") == 0 && (_excelDocument == null))
                )
            {
                return false;
            }

            // Check worksheets
            if (!ExistsWorksheet("SETTINGS")) return false;
            if (!ExistsWorksheet("lang")) return false;
            if (!ExistsWorksheet("tag")) return false;
            if (!ExistsWorksheet("role")) return false;
            if (!ExistsWorksheet("quality")) return false;
            if (!ExistsWorksheet("artist")) return false;
            if (!ExistsWorksheet("work")) return false;
            if (!ExistsWorksheet("isrc")) return false;
            if (!ExistsWorksheet("album")) return false;
            if (!ExistsWorksheet("asset")) return false;

            // Check columns and identify their indexes
            Dictionary<Int32, object> map = new Dictionary<Int32,object>(); // TODO remove call to constructor, map is a pointer
            int i;


            // Strict order so as to enable referential integrity check

            // SETTINGS
            map = GetHeader("SETTINGS");
            if (!ExistsCellValueInRow("parameter", map, "SETTINGS")) return false;
            if (!ExistsCellValueInRow("value", map, "SETTINGS")) return false;
            _settings = WorksheetActiveRows("SETTINGS");
            ParseSettings(); // must be parsed first

            // lang
            map = GetHeader("lang");
            if (!ExistsCellValueInRow("local_db", map, "lang")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "lang")) return false;
            if (!ExistsCellValueInRow("long_name", map, "lang")) return false;
            if (!ExistsCellValueInRow("short_name", map, "lang")) return false;
            if (!ExistsCellValueInRow("default", map, "lang")) return false;
            _langs = WorksheetActiveRows("lang");
            ParseLangs(); // must be parsed second, because columns lang-dependent

            // tag
            map = GetHeader("tag");
            if (!ExistsCellValueInRow("local_db", map, "tag")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "tag")) return false;
            if (!ExistsCellValueInRow("tag_name", map, "tag")) return false;
            _tags = WorksheetActiveRows("tag");

            // role
            map = GetHeader("role");
            if (!ExistsCellValueInRow("local_db", map, "role")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "role")) return false;
            if (!ExistsCellValueInRow("role_name", map, "role")) return false;
            _roles = WorksheetActiveRows("role");

            // quality
            map = GetHeader("quality");
            if (!ExistsCellValueInRow("local_db", map, "quality")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "quality")) return false;
            if (!ExistsCellValueInRow("quality_name", map, "quality")) return false;
            _qualities = WorksheetActiveRows("quality");

            // artist
            map = GetHeader("artist");
            if (!ExistsCellValueInRow("local_db", map, "artist")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "artist")) return false;
            if (!ExistsCellValueInRow("id", map, "artist")) return false;
            if (!ExistsCellValueInRow("birth", map, "artist")) return false;
            if (!ExistsCellValueInRow("death", map, "artist")) return false;
            foreach (Lang lang in CatalogContext.Instance.Langs)
            {
                if (!ExistsCellValueInRow("lastname_" + lang.ShortName, map, "artist")) return false;
                if (!ExistsCellValueInRow("firstname_" + lang.ShortName, map, "artist")) return false;
            }
            _artists = WorksheetActiveRows("artist");

            // work
            map = GetHeader("work");
            if (!ExistsCellValueInRow("local_db", map, "work")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "work")) return false;
            if (!ExistsCellValueInRow("id", map, "work")) return false;
            if (!ExistsCellValueInRow("id_parent", map, "work")) return false;
            if (!ExistsCellValueInRow("catalog_number", map, "work")) return false;
            if (!ExistsCellValueInRow("key", map, "work")) return false;
            if (!ExistsCellValueInRow("year", map, "work")) return false;
            i = 1;
            while (
                        ExistsCellValueInRow("id_contributor" + i, map, "work")
                        && ExistsCellValueInRow("role_contributor" + i, map, "work")
                    )
            {
                i++;
            }
            if (i == 1)
            {
                return false;
            }
            if (!ExistsCellValueInRow("movement_number", map, "work")) return false;
            foreach (Lang lang in CatalogContext.Instance.Langs)
            {
                if (!ExistsCellValueInRow("title_" + lang.ShortName, map, "work")) return false;
                if (!ExistsCellValueInRow("movement_title_" + lang.ShortName, map, "work")) return false;
            }
            _works = WorksheetActiveRows("work");

            // isrc
            map = GetHeader("isrc");
            if (!ExistsCellValueInRow("local_db", map, "isrc")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "isrc")) return false;
            if (!ExistsCellValueInRow("isrc_id", map, "isrc")) return false;
            if (!ExistsCellValueInRow("work_id", map, "isrc")) return false;
            i = 1;
            while (
                    ExistsCellValueInRow("id_contributor" + i, map, "isrc")
                    && ExistsCellValueInRow("role_contributor" + i, map, "isrc")
                    && ExistsCellValueInRow("quality_contributor" + i, map, "isrc")
                    )
            {
                i++;
            }
            if (i == 1)
            {
                return false;
            }
            if (!ExistsCellValueInRow("c_name", map, "isrc")) return false;
            if (!ExistsCellValueInRow("c_year", map, "isrc")) return false;
            if (!ExistsCellValueInRow("p_name", map, "isrc")) return false;
            if (!ExistsCellValueInRow("p_year", map, "isrc")) return false;
            if (!ExistsCellValueInRow("recording_location", map, "isrc")) return false;
            if (!ExistsCellValueInRow("recording_year", map, "isrc")) return false;
            if (!ExistsCellValueInRow("available_separately", map, "isrc")) return false;
            if (!ExistsCellValueInRow("catalog_tier", map, "isrc")) return false;
            _isrcs = WorksheetActiveRows("isrc");

            // album
            map = GetHeader("album");
            if (!ExistsCellValueInRow("local_db", map, "album")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "album")) return false;
            if (!ExistsCellValueInRow("album_id", map, "album")) return false;
            if (!ExistsCellValueInRow("c_name", map, "album")) return false;
            if (!ExistsCellValueInRow("c_year", map, "album")) return false;
            if (!ExistsCellValueInRow("p_name", map, "album")) return false;
            if (!ExistsCellValueInRow("p_year", map, "album")) return false;
            if (!ExistsCellValueInRow("recording_location", map, "album")) return false;
            if (!ExistsCellValueInRow("recording_year", map, "album")) return false;
            if (!ExistsCellValueInRow("catalog_tier", map, "album")) return false;
            if (!ExistsCellValueInRow("consumer_release_date", map, "album")) return false;
            if (!ExistsCellValueInRow("label", map, "album")) return false;
            if (!ExistsCellValueInRow("reference", map, "album")) return false;
            if (!ExistsCellValueInRow("ean", map, "album")) return false;
            if (!ExistsCellValueInRow("genre", map, "album")) return false;
            if (!ExistsCellValueInRow("subgenre", map, "album")) return false;
            foreach (Lang lang in CatalogContext.Instance.Langs)
            {
                if (!ExistsCellValueInRow("title_" + lang.ShortName, map, "album")) return false;
            }
            if (!ExistsCellValueInRow("primary_artist", map, "album")) return false;
            if (!ExistsCellValueInRow("recording_location", map, "album")) return false;
            if (!ExistsCellValueInRow("recording_year", map, "album")) return false;
            if (!ExistsCellValueInRow("redeliver", map, "album")) return false;
            _albums = WorksheetActiveRows("album");

            // asset
            map = GetHeader("asset");
            if (!ExistsCellValueInRow("local_db", map, "asset")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "asset")) return false;
            if (!ExistsCellValueInRow("album_id", map, "asset")) return false;
            if (!ExistsCellValueInRow("volume_index", map, "asset")) return false;
            if (!ExistsCellValueInRow("track_index", map, "asset")) return false;
            if (!ExistsCellValueInRow("isrc_id", map, "asset")) return false;
            _assets = WorksheetActiveRows("asset");

            return true;
        }

        /// <summary>
        /// Returns a dictionary of cell objects (Key = column)
        /// </summary>
        /// <param name="row">
        ///     For Excel, represents the row number
        ///     For XML, represents the XmlNode row object
        /// </param>
        /// <param name="worksheetName">
        ///     Used only in Excel format.
        /// </param>
        /// <returns></returns>
        private Dictionary<Int32, object> CellMapByRow(object row, String worksheetName = "")
        {
            //if (String.IsNullOrEmpty(_formatType))
            //{
            //    return null;
            //}

            if (row == null)
            {
                return null;
            }

            Dictionary<Int32, object> map = new Dictionary<Int32, object>();
            Int32 index = 1;

            switch (_formatType)
            {
                case "excel":
                    //if (String.IsNullOrEmpty(worksheetName) || !_worksheets.ContainsKey(worksheetName))
                    //{
                    //    return null;
                    //}
                    Int32 rowIndex = (Int32)row;
                    if (rowIndex < 1)
                    {
                        return null;
                    }
                    for (; index <= _excelLastColumn[worksheetName]; index++)
                    {
                        Range cell = ((_Worksheet)_worksheets[worksheetName]).Cells[rowIndex, index];
                        if (cell != null && cell.Value != null && !String.IsNullOrEmpty(cell.Value.ToString()))
                        {
                            map[index] = cell;
                        }
                    }
                    break;

                case "xml":
                    foreach (XmlNode cell in ((XmlNode)row).ChildNodes.Cast<XmlNode>().Where(n => n.Name.CompareTo(OfficeXml.WorksheetCell) == 0))
                    {
                        // Cell index has priority over deduced index
                        if (cell.Attributes[OfficeXml.CellIndex] != null)
                        {
                            Int32 tryIndex;
                            if (int.TryParse(cell.Attributes[OfficeXml.CellIndex].InnerText, out tryIndex))
                            {
                                index = tryIndex;
                            }
                        }
                        map[index] = cell;
                        index++;
                    }
                    break;
            }

            return map;
        }

        /// <summary>
        /// Returns a List of Rows marked as 'active' in AT LEAST one column. Possible configurations are:
        /// localActive == true, partnerActive == false:    column local_db is filtered according to rows marked 'active'
        /// localActive == false, partnerActive == true:    column partner_db is filtered according to rows marked 'active'
        /// localActive == true, partnerActive == true:     both columns are filtered according to rows marked 'active' (equivalent to AND clause)
        /// localActive == false, partnerActive == false:   at least one of the two columns has a row marked 'active' (equivalent to OR clause)
        /// </summary>
        /// <param name="worksheetName"></param>
        /// <param name="localActive"></param>
        /// <param name="partnerActive"></param>
        /// <returns></returns>
        private List<object> WorksheetActiveRows(String worksheetName, bool localActive = false, bool partnerActive = false)
        {
            //if (String.IsNullOrEmpty(worksheetName))
            //{
            //    return null;
            //}

            if (!(_worksheets.ContainsKey(worksheetName)) ||  _worksheets[worksheetName] == null)
            {
                return null;
            }

            if (_excelLastRow == null || !(_excelLastRow.ContainsKey(worksheetName)) || _excelLastColumn == null || !(_excelLastColumn.ContainsKey(worksheetName)))
            {
                return null;
            }

            List<object> rows = null; 
            List<object> filteredRows = new List<object>();

            switch (_formatType)
            {
                case "excel":
                    rows = Enumerable.Range(1, _excelLastRow[worksheetName]).Cast<object>().ToList();
                    break;
                case "xml":
                    rows = ((XmlNode)_worksheets[worksheetName]).ChildNodes.Cast<object>()
                    .Where(r => ((XmlNode)r).Name.CompareTo(OfficeXml.WorksheetRow) == 0)
                    .ToList();
                    break;
            }

            // SETTINGS: return all lines
            if (worksheetName.CompareTo("SETTINGS") == 0)
            {
                if (rows != null) rows.RemoveAt(0); // Header line
                return rows;
            }

            // Any other tab
            foreach (object row in rows)
            {
                Dictionary<Int32, object> map = null;
                map = CellMapByRow(row, worksheetName);

                // Any worksheet other than SETTINGS...
                bool local = false;
                bool partner = false;

                switch (_formatType)
                {
                    case "excel":
                        local = map.ContainsKey(_worksheetColumns[worksheetName]["local_db"]) && ((Range)map[_worksheetColumns[worksheetName]["local_db"]]).Value.ToString().ToLower().CompareTo("active") == 0;
                        partner = map.ContainsKey(_worksheetColumns[worksheetName]["partner_db"]) && ((Range)map[_worksheetColumns[worksheetName]["partner_db"]]).Value.ToString().ToLower().CompareTo("active") == 0;
                        break;

                    case "xml":
                        local = map.ContainsKey(_worksheetColumns[worksheetName]["local_db"]) && ((XmlNode)map[_worksheetColumns[worksheetName]["local_db"]]).InnerText.ToLower().CompareTo("active") == 0;
                        partner = map.ContainsKey(_worksheetColumns[worksheetName]["partner_db"]) && ((XmlNode)map[_worksheetColumns[worksheetName]["partner_db"]]).InnerText.ToLower().CompareTo("active") == 0;
                        break;
                }

                if (
                        (localActive && partnerActive && local && partner)
                        || (localActive && !partnerActive && local)
                        || (!localActive && partnerActive && partner)
                        || (!localActive && !partnerActive && (local || partner))
                    )
                {
                    filteredRows.Add(row);
                }
            }


            return filteredRows;
        }

        /// <summary>
        /// Tests occurrence of a cell value in a given row.
        /// </summary>
        /// <param name="cellValue"></param>
        /// <param name="map"></param>
        /// <param name="worksheetName">If provided, the value sarched is considered a header name and is recorded along with column index</param>
        /// <returns></returns>
        private bool ExistsCellValueInRow(String cellValue, Dictionary<Int32, object> map, String worksheetName = "")
        {
            if (String.IsNullOrEmpty(cellValue) || map == null || map.Count == 0)
            {
                return false;
            }

            KeyValuePair<Int32, object>? element = null;
            bool exists = false;

            switch (_formatType)
            {
                case "excel":
                    element = map.FirstOrDefault(e => ((Range)e.Value).Value.ToString().Trim().CompareTo(cellValue) == 0);
                    break;

                case "xml":
                    element = map.FirstOrDefault(e => ((XmlNode)e.Value).InnerText.Trim().CompareTo(cellValue) == 0);
                    break;
            }

            exists = (element != null) && ((KeyValuePair<Int32, object>)element).Value != null;
            if (exists && !String.IsNullOrEmpty(worksheetName) && _worksheetColumns[worksheetName] != null)
            {
                Int32 index = ((KeyValuePair<Int32, object>)element).Key;
                _worksheetColumns[worksheetName][cellValue] = index;
            }

            return exists;
        }

        /// <summary>
        /// Tests worksheet existence and records its table main sub-node if found
        /// </summary>
        /// <param name="worksheetName"></param>
        /// <returns></returns>
        private bool ExistsWorksheet(String worksheetName)
        {
            //if (String.IsNullOrEmpty(worksheetName))
            //{
            //    return false;
            //}

            if (_worksheets.ContainsKey(worksheetName))
            {
                return true;
            }

            object table = null;

            switch (_formatType)
            {
                case "excel":
                    Sheets sheets = _excelDocument.Worksheets;
                    int sheetsCount = sheets.Count;
                    int index = 1;
                    int maxSheets = 64; // Security
                    while (index <= sheetsCount && index <= maxSheets)
                    {
                        _Worksheet excelWorksheet = (_Worksheet)sheets.get_Item(index);
                        if (excelWorksheet != null)
                        {
                            if (!String.IsNullOrEmpty(excelWorksheet.Name) && excelWorksheet.Name.CompareTo(worksheetName) == 0)
                            {
                                table = excelWorksheet;
                                var rngLast = excelWorksheet.get_Range("A1").SpecialCells(XlCellType.xlCellTypeLastCell);
                                _excelLastRow[worksheetName] = rngLast.Row;
                                _excelLastColumn[worksheetName] = rngLast.Column;
                                break;
                            }
                        }
                        index++;
                    }
                    break;

                case "xml":
                    XmlNode xmlWorksheet = _xmlDocument.DocumentElement.ChildNodes.Cast<XmlNode>()
                        .FirstOrDefault(
                            n =>
                            n.Name == OfficeXml.Worksheet
                            && n.Attributes[OfficeXml.WorksheetName] != null
                            && n.Attributes[OfficeXml.WorksheetName].InnerText.CompareTo(worksheetName) == 0
                        );

                    if (xmlWorksheet == null)
                    {
                        return false;
                    }
                    table = xmlWorksheet.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == OfficeXml.WorksheetTable);
                    break;
            }

            bool exists = table != null;

            if (exists)
            {
                // Add table element of the worksheet
                _worksheets[worksheetName] = table;
                _worksheetColumns[worksheetName] = new Dictionary<String, Int32>();
            }

            return exists;
        }

        /// <summary>
        /// Retrieves cell value according to the different implementations. Heart of the multi-format support.
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="columnIndex"></param>
        /// <param name="trim"></param>
        /// <param name="defaultValue">so as to adapt return type, if a number e.g. is expected</param>
        /// <returns></returns>
        private String CellContentWizard(Dictionary<Int32, object> cells, Int32 columnIndex, String defaultValue = "", bool trim = true)
        {
            // _formatType nullity is not tested for the sake of performance
            if (cells == null || cells.Count == 0 || columnIndex <= 0)
            {
                return defaultValue;
            }

            object o = (cells.Keys.ToList().Contains(columnIndex))
                ? cells.FirstOrDefault(c => c.Key == columnIndex).Value
                : null;

            if (o == null)
            {
                return defaultValue;
            }

            String v;
            switch (_formatType)
            {
                case "excel":
                    v = (trim) ? ((Range)o).Value.ToString().Trim() : ((Range)o).Value.ToString();
                    return (String.IsNullOrEmpty(v)) ? defaultValue : v;

                case "xml":
                    v = (trim) ? ((XmlNode)(o)).InnerText.Trim() : ((XmlNode)(o)).InnerText;
                    return (String.IsNullOrEmpty(v)) ? defaultValue : v;

                default:
                    return defaultValue;
            }
        }

        /// <summary>
        /// Settings parser
        /// </summary>
        private void ParseSettings()
        {
            if (_settings == null)
            {
                return;
            }

            foreach (object row in _settings)
            {
                Dictionary<Int32, object> cells = CellMapByRow(row, "SETTINGS");
                if (cells != null && cells.Count > 0)
                {
                    String parameter = CellContentWizard(cells, _worksheetColumns["SETTINGS"]["parameter"]);
                    String value = CellContentWizard(cells, _worksheetColumns["SETTINGS"]["value"]);

                    if (!String.IsNullOrEmpty(parameter))
                    {
                        if (CatalogContext.Instance.Settings == null)
                        {
                            CatalogContext.Instance.Settings = new CatalogSettings();
                        }
                        CatalogSettings settings = CatalogContext.Instance.Settings;

                        switch (parameter)
                        {
                            case "SUPPLIER": settings.SupplierDefault = value; break;
                            case "LABEL_NAME": settings.LabelDefault = value; break;
                            case "ALBUM_COPYRIGHT_C_OWNER": settings.COwnerDefault = (String.IsNullOrEmpty(value)) ? settings.LabelDefault : value; break;
                            case "ALBUM_COPYRIGHT_P_OWNER": settings.POwnerDefault = (String.IsNullOrEmpty(value)) ? settings.LabelDefault : value; break;
                            case "ALBUM_CATALOG_TIER":
                                if (!String.IsNullOrEmpty(value))
                                {
                                    switch (value.ToLower())
                                    {
                                        case "back": settings.CatalogTierDefault = CatalogTier.Back; break;
                                        case "budget": settings.CatalogTierDefault = CatalogTier.Budget; break;
                                        case "front": settings.CatalogTierDefault = CatalogTier.Front; break;
                                        case "mid": settings.CatalogTierDefault = CatalogTier.Mid; break;
                                        case "premium": settings.CatalogTierDefault = CatalogTier.Premium; break;
                                    }
                                }
                                else
                                {
                                    settings.CatalogTierDefault = CatalogTier.Front;
                                }
                                break;
                            case "ALBUM_MAIN_GENRE": settings.MainGenreDefault = value; break;
                            case "ALBUM_FORMAT":
                                if (!String.IsNullOrEmpty(value))
                                {
                                    switch (value.ToLower())
                                    {
                                        case "album": settings.FormatDefault = ProductFormat.Album; break;
                                        case "boxset": settings.FormatDefault = ProductFormat.BoxSet; break;
                                        case "ep": settings.FormatDefault = ProductFormat.EP; break;
                                        case "single": settings.FormatDefault = ProductFormat.Single; break;
                                    }
                                }
                                else
                                {
                                    settings.FormatDefault = ProductFormat.Album;
                                }
                                break;
                            case "ASSET_AVAILABLE_SEPARATELY": settings.AvailableSeparatelyDefault = (String.IsNullOrEmpty(value) || value.Trim().ToLower().CompareTo("no") != 0); break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Langs parser
        /// Since other worksheet rows count is unknown at this stage, do not update progress bar inside the function
        /// </summary>
        private void ParseLangs()
        {
            if (_langs == null)
            {
                return;
            }

            foreach (object row in _langs)
            {
                Dictionary<Int32, object> cells = CellMapByRow(row, "lang");
                if (cells != null && cells.Count > 0)
                {
                    Lang lang = new Lang
                    {
                        LongName = CellContentWizard(cells, _worksheetColumns["lang"]["long_name"]),
                        ShortName = CellContentWizard(cells, _worksheetColumns["lang"]["short_name"]),
                        IsDefault = CellContentWizard(cells, _worksheetColumns["lang"]["default"]).ToLower().CompareTo("yes") == 0,
                    };

                    // Short name is mandatory
                    if (!String.IsNullOrEmpty(lang.ShortName))
                    {
                        CatalogContext.Instance.Langs.Add(lang);
                    }
                }
            }
        }

        /// <summary>
        /// Tags parser
        /// </summary>
        private void ParseTags()
        {
            if (_tags == null)
            {
                return;
            }

            foreach (object row in _tags)
            {
                Dictionary<Int32, object> cells = CellMapByRow(row, "tag");
                if (cells != null && cells.Count > 0)
                {
                    Tag tag = new Tag
                    {
                        Name = CellContentWizard(cells, _worksheetColumns["tag"]["tag_name"]),
                    };

                    if (!String.IsNullOrEmpty(tag.Name))
                    {
                        CatalogContext.Instance.Tags.Add(tag);
                    }
                }

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

        /// <summary>
        /// Roles parser
        /// </summary>
        private void ParseRoles()
        {
            if (_roles == null)
            {
                return;
            }

            foreach (object row in _roles)
            {
                Dictionary<Int32, object> cells = CellMapByRow(row, "role");
                if (cells != null && cells.Count > 0)
                {
                    Role role = new Role
                    {
                        Name = CellContentWizard(cells, _worksheetColumns["role"]["role_name"]).ToLower(),
                    };

                    // Attempt to retrieve a qualified name (standardized)
                    switch (role.Name)
                    {
                        case "arranger": role.Reference = Role.QualifiedName.Arranger; break;
                        case "composer": role.Reference = Role.QualifiedName.Composer; break;
                        case "conductor": role.Reference = Role.QualifiedName.Conductor; break;
                        case "engineer": role.Reference = Role.QualifiedName.Engineer; break;
                        case "ensemble": role.Reference = Role.QualifiedName.Ensemble; break;
                        case "performer": role.Reference = Role.QualifiedName.Performer; break;
                    }

                    if (role.Reference != null)
                    {
                        CatalogContext.Instance.Roles.Add(role);
                    }
                }

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

        private void ParseQualities()
        {
            if (_qualities == null)
            {
                return;
            }

            foreach (object row in _qualities)
            {
                Dictionary<Int32, object> cells = CellMapByRow(row, "quality");
                if (cells != null && cells.Count > 0)
                {
                    Quality quality = new Quality
                    {
                        Name = CellContentWizard(cells, _worksheetColumns["quality"]["quality_name"]),
                    };

                    if (!String.IsNullOrEmpty(quality.Name))
                    {
                        CatalogContext.Instance.Qualities.Add(quality);
                    }
                }

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

        private void ParseArtists()
        {
            if (_artists == null)
            {
                return;
            }

            // TODO replace Convert.ToInt by int.TryParse...
            foreach (object row in _artists)
            {
                Dictionary<Int32, object> cells = CellMapByRow(row, "artist");
                if (cells != null && cells.Count > 0)
                {
                    Artist artist = new Artist
                    {
                        Id = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns["artist"]["id"], "0")),
                        Birth = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["artist"]["birth"], "0")),
                        Death = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["artist"]["death"], "0")),
                        LastName = new Dictionary<Lang, String>
                        {
                            {CatalogContext.Instance.DefaultLang, CellContentWizard(cells, _worksheetColumns["artist"]["lastname_" + CatalogContext.Instance.DefaultLang.ShortName])},
                        },
                        FirstName = new Dictionary<Lang, String>
                        {
                            {CatalogContext.Instance.DefaultLang, CellContentWizard(cells, _worksheetColumns["artist"]["firstname_" + CatalogContext.Instance.DefaultLang.ShortName])},
                        },
                    };

                    if (artist == null || artist.Id <= 0 || String.IsNullOrEmpty(artist.LastName[CatalogContext.Instance.DefaultLang]))
                    {
                        if (_mainFormViewModel != null)
                        {
                            _mainFormViewModel.InputProgressBarValue++;
                        }
                        continue;
                    }

                    if (artist.Birth <= 0) { artist.Birth = null; }
                    if (artist.Death <= 0) { artist.Death = null; }

                    // Object may have several lang-dependent field sets
                    List<Lang> otherLangs = CatalogContext.Instance.Langs.Where(l => l != CatalogContext.Instance.DefaultLang).ToList();
                    foreach (Lang lang in otherLangs)
                    {
                        String last = CellContentWizard(cells, _worksheetColumns["artist"]["lastname_" + lang.ShortName]);
                        String first = CellContentWizard(cells, _worksheetColumns["artist"]["firstname_" + lang.ShortName]);

                        if (String.IsNullOrEmpty(last))
                        {
                            last = artist.LastName[CatalogContext.Instance.DefaultLang];
                        }
                        if (String.IsNullOrEmpty(first))
                        {
                            first = artist.FirstName[CatalogContext.Instance.DefaultLang];
                        }

                        artist.LastName[lang] = last;
                        artist.FirstName[lang] = first;
                    }

                    CatalogContext.Instance.Artists.Add(artist);
                }

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

        /// <summary>
        /// Works parser
        /// </summary>
        private void ParseWorks()
        {
            if (_works == null)
            {
                return;
            }

            // TODO replace Convert.ToInt by int.TryParse...
            foreach (object row in _works)
            {
                Dictionary<Int32, object> cells = CellMapByRow(row, "work");
                if (cells != null && cells.Count > 0)
                {

                    String shortKey = CellContentWizard(cells, _worksheetColumns["work"]["key"]);
                    Work work = new Work
                    {
                        Id = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns["work"]["id"], "0")),
                        Parent = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns["work"]["id_parent"], "0")),
                        ClassicalCatalog = CellContentWizard(cells, _worksheetColumns["work"]["catalog_number"]),
                        Tonality = (_shortenedKeys.ContainsKey(shortKey)) ? _shortenedKeys[shortKey] : (Key?)null,
                        Year = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["work"]["year"], "0")),
                        MovementNumber = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["work"]["movement_number"], "0")),
                        Title = new Dictionary<Lang, String>
                        {
                            {CatalogContext.Instance.DefaultLang, CellContentWizard(cells, _worksheetColumns["work"]["title_" + CatalogContext.Instance.DefaultLang.ShortName])},  
                        },
                        MovementTitle = new Dictionary<Lang, String>
                        {
                            {CatalogContext.Instance.DefaultLang, CellContentWizard(cells, _worksheetColumns["work"]["movement_title_" + CatalogContext.Instance.DefaultLang.ShortName])},  
                        },
                        Contributors = new Dictionary<Int32, Role>(),
                    };

                    if (work == null || work.Id <= 0)
                    {
                        if (_mainFormViewModel != null)
                        {
                            _mainFormViewModel.InputProgressBarValue++;
                        }
                        continue;
                    }

                    if (work.Parent <= 0) { work.Parent = null; }
                    if (work.Year <= 0) { work.Year = null; }
                    if (work.MovementNumber <= 0) { work.MovementNumber = null; }

                    // Object may have several lang-dependent field sets
                    List<Lang> otherLangs = CatalogContext.Instance.Langs.Where(l => l != CatalogContext.Instance.DefaultLang).ToList();
                    foreach (Lang lang in otherLangs)
                    {
                        String title = CellContentWizard(cells, _worksheetColumns["work"]["title_" + lang.ShortName]);
                        String movementTitle = CellContentWizard(cells, _worksheetColumns["work"]["movement_title_" + lang.ShortName]);

                        work.Title[lang] = (String.IsNullOrEmpty(title)) ? work.Title[CatalogContext.Instance.DefaultLang] : title;
                        work.MovementTitle[lang] = (String.IsNullOrEmpty(movementTitle)) ? work.MovementTitle[CatalogContext.Instance.DefaultLang] : movementTitle;
                    }

                    int i = 1;
                    // TODO replace Convert.ToInt by int.TryParse...
                    while (
                                (_worksheetColumns["work"].ContainsKey("id_contributor" + i))
                                && (_worksheetColumns["work"].ContainsKey("role_contributor" + i))
                            )
                    {
                        Int32 idContributor = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns["work"]["id_contributor" + i], "0"));
                        String roleName = CellContentWizard(cells, _worksheetColumns["work"]["role_contributor" + i]).ToLower();
                        Role roleContributor = CatalogContext.Instance.Roles.FirstOrDefault(r => r.Name.CompareTo(roleName) == 0);

                        if (idContributor > 0 && roleContributor != null)
                        {
                            work.Contributors[idContributor] = roleContributor;
                        }
                        i++;
                    }

                    // If at least one language set (default or not) and one contributor is available, save the entry 
                    if (work.Contributors.Count > 0 && work.Title.Count > 0)
                    {
                        CatalogContext.Instance.Works.Add(work);
                    }
                }

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

        /// <summary>
        /// Isrcs parser
        /// </summary>
        private void ParseIsrcs()
        {
            if (_isrcs == null)
            {
                return;
            }

            // TODO replace Convert.ToInt by int.TryParse...
            foreach (object row in _isrcs)
            {
                Dictionary<Int32, object> cells = CellMapByRow(row, "isrc");
                if (cells != null && cells.Count > 0)
                {
                    Isrc isrc = new Isrc
                    {
                        Id = CellContentWizard(cells, _worksheetColumns["isrc"]["isrc_id"]),
                        Work = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns["isrc"]["work_id"], "0")),
                        CName = CellContentWizard(cells, _worksheetColumns["isrc"]["c_name"]),
                        CYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["isrc"]["c_year"], "0")),
                        PName = CellContentWizard(cells, _worksheetColumns["isrc"]["p_name"]),
                        PYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["isrc"]["p_year"], "0")),
                        RecordingLocation = CellContentWizard(cells, _worksheetColumns["isrc"]["recording_location"]),
                        RecordingYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["isrc"]["recording_year"], "0")),
                        AvailableSeparately = (CellContentWizard(cells, _worksheetColumns["isrc"]["available_separately"]).ToLower().CompareTo("no") == 0)
                            ? false
                            : CatalogContext.Instance.Settings.AvailableSeparatelyDefault,
                        Contributors = new Dictionary<Int32, Dictionary<Role, Quality>>(),
                    };

                    if (isrc == null || String.IsNullOrEmpty(isrc.Id) || isrc.Id.Length < 12 || isrc.Id.Length > 15 || isrc.Work <= 0)
                    {
                        if (_mainFormViewModel != null)
                        {
                            _mainFormViewModel.InputProgressBarValue++;
                        }
                        continue;
                    }

                    if (String.IsNullOrEmpty(isrc.CName))
                    {
                        isrc.CName = CatalogContext.Instance.Settings.LabelDefault;
                    }
                    if (isrc.CYear < 1900 || isrc.CYear > 2100)
                    {
                        isrc.CYear = null;
                    }
                    if (String.IsNullOrEmpty(isrc.PName))
                    {
                        isrc.PName = CatalogContext.Instance.Settings.LabelDefault;
                    }
                    if (isrc.PYear < 1900 || isrc.PYear > 2100)
                    {
                        isrc.PYear = null;
                    }
                    if (isrc.RecordingYear < 1900 || isrc.RecordingYear > 2100)
                    {
                        isrc.RecordingYear = null;
                    }

                    int i = 1;
                    // TODO replace Convert.ToInt by int.TryParse...
                    while (
                                (_worksheetColumns["isrc"].ContainsKey("id_contributor" + i))
                                && (_worksheetColumns["isrc"].ContainsKey("role_contributor" + i))
                                && (_worksheetColumns["isrc"].ContainsKey("quality_contributor" + i))
                            )
                    {
                        Int32 idContributor = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns["isrc"]["id_contributor" + i], "0"));
                        String roleContributorName = CellContentWizard(cells, _worksheetColumns["isrc"]["role_contributor" + i]).ToLower();
                        Role roleContributor = CatalogContext.Instance.Roles.FirstOrDefault(c => c.Name.CompareTo(roleContributorName) == 0);
                        String qualityContributorName = CellContentWizard(cells, _worksheetColumns["isrc"]["quality_contributor" + i]);
                        Quality qualityContributor = CatalogContext.Instance.Qualities.FirstOrDefault(c => c.Name.CompareTo(qualityContributorName) == 0);

                        // Contributor valid
                        if (idContributor > 0 && roleContributor != null && qualityContributor != null)
                        {
                            if (!isrc.Contributors.ContainsKey(idContributor))
                            {
                                isrc.Contributors[idContributor] = new Dictionary<Role, Quality>();
                            }
                            isrc.Contributors[idContributor][roleContributor] = qualityContributor;
                        }
                        i++;
                    }

                    String tier = CellContentWizard(cells, _worksheetColumns["isrc"]["catalog_tier"]).ToLower();
                    if (!String.IsNullOrEmpty(tier))
                    {
                        switch (tier)
                        {
                            case "back": isrc.Tier = CatalogTier.Back; break;
                            case "budget": isrc.Tier = CatalogTier.Budget; break;
                            case "free": isrc.Tier = CatalogTier.Free; break;
                            case "front": isrc.Tier = CatalogTier.Front; break;
                            case "mid": isrc.Tier = CatalogTier.Mid; break;
                            case "premium": isrc.Tier = CatalogTier.Premium; break;
                        }
                    }

                    // If at least isrc has one contributor, record entry
                    if (isrc.Contributors.Count > 0)
                    {
                        CatalogContext.Instance.Isrcs.Add(isrc);
                    }
                }

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

        /// <summary>
        /// Albums parser
        /// </summary>
        private void ParseAlbums()
        {
            if (_albums == null)
            {
                return;
            }

            // TODO replace Convert.ToInt by int.TryParse...
            foreach (object row in _albums)
            {
                Dictionary<Int32, object> cells = CellMapByRow(row, "album");
                if (cells != null && cells.Count > 0)
                {
                    {
                        String dateString = CellContentWizard(cells, _worksheetColumns["album"]["consumer_release_date"]);
                        String genreTag = CellContentWizard(cells, _worksheetColumns["album"]["genre"]);
                        String subgenreTag = CellContentWizard(cells, _worksheetColumns["album"]["subgenre"]);
                        Album album = new Album
                        {
                            Id = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns["album"]["album_id"], "0")),
                            CName = CellContentWizard(cells, _worksheetColumns["album"]["c_name"]),
                            CYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["album"]["c_year"], "0")),
                            PName = CellContentWizard(cells, _worksheetColumns["album"]["p_name"]),
                            PYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["album"]["p_year"], "0")),
                            RecordingLocation = CellContentWizard(cells, _worksheetColumns["album"]["recording_location"]),
                            RecordingYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["album"]["recording_year"], "0")),
                            Owner = CellContentWizard(cells, _worksheetColumns["album"]["label"]),
                            CatalogReference = CellContentWizard(cells, _worksheetColumns["album"]["reference"]),
                            Ean = Convert.ToInt64(CellContentWizard(cells, _worksheetColumns["album"]["ean"], "0")),
                            ConsumerReleaseDate = CellContentWizard(cells, _worksheetColumns["album"]["consumer_release_date"]).ToDateTime(),
                            Title = new Dictionary<Lang, String>
                        {
                            {CatalogContext.Instance.DefaultLang, CellContentWizard(cells, _worksheetColumns["album"]["title_" + CatalogContext.Instance.DefaultLang.ShortName])},
                        },
                            Genre = (CatalogContext.Instance.Tags.Exists(t => t.Name.CompareTo(genreTag) == 0))
                                ? CatalogContext.Instance.Tags.FirstOrDefault(t => t.Name.CompareTo(genreTag) == 0)
                                : null,
                            Subgenre = (!String.IsNullOrEmpty(subgenreTag) && CatalogContext.Instance.Tags.Exists(t => t.Name.CompareTo(subgenreTag) == 0))
                                ? CatalogContext.Instance.Tags.FirstOrDefault(t => t.Name.CompareTo(subgenreTag) == 0)
                                : null,
                            PrimaryArtistId = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns["album"]["primary_artist"], "0")),
                            Redeliver = false,
                        };

                        if (album == null || album.Id <= 0)
                        {
                            if (_mainFormViewModel != null)
                            {
                                _mainFormViewModel.InputProgressBarValue++;
                            }
                            continue;
                        }

                        if (String.IsNullOrEmpty(album.CName))
                        {
                            album.CName = CatalogContext.Instance.Settings.LabelDefault;
                        }
                        if (album.CYear < 1900 || album.CYear > 2100)
                        {
                            album.CYear = null;
                        }
                        if (String.IsNullOrEmpty(album.PName))
                        {
                            album.PName = CatalogContext.Instance.Settings.LabelDefault;
                        }
                        if (album.PYear < 1900 || album.PYear > 2100)
                        {
                            album.PYear = null;
                        }
                        if (album.RecordingYear < 1900 || album.RecordingYear > 2100)
                        {
                            album.RecordingYear = null;
                        }

                        if (String.IsNullOrEmpty(album.Owner))
                        {
                            album.Owner = CatalogContext.Instance.Settings.LabelDefault;
                        }

                        String tier = CellContentWizard(cells, _worksheetColumns["album"]["catalog_tier"]).ToLower();
                        if (!String.IsNullOrEmpty(tier))
                        {
                            switch (tier)
                            {
                                case "back": album.Tier = CatalogTier.Back; break;
                                case "budget": album.Tier = CatalogTier.Budget; break;
                                case "front": album.Tier = CatalogTier.Front; break;
                                case "mid": album.Tier = CatalogTier.Mid; break;
                                case "premium": album.Tier = CatalogTier.Premium; break;
                            }
                        }
                        else
                        {
                            album.Tier = CatalogContext.Instance.Settings.CatalogTierDefault;
                        }

                        if (album.Ean != null && album.Ean < 0) { album.Ean = null; }


                        // Object may have several lang-dependent field sets
                        List<Lang> otherLangs = CatalogContext.Instance.Langs.Where(l => l != CatalogContext.Instance.DefaultLang).ToList();
                        foreach (Lang lang in otherLangs)
                        {
                            String title = CellContentWizard(cells, _worksheetColumns["album"]["title_" + lang.ShortName]);
                            album.Title[lang] = (!String.IsNullOrEmpty(title)) ? album.Title[CatalogContext.Instance.DefaultLang] : title;
                        }

                        CatalogContext.Instance.Albums.Add(album);
                    }
                }

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

        /// <summary>
        /// Assets parser
        /// </summary>
        private void ParseAssets()
        {
            if (_assets == null)
            {
                return;
            }

            // TODO replace Convert.ToInt by int.TryParse...
            foreach (object row in _assets)
            {
                Dictionary<Int32, object> cells = CellMapByRow(row, "asset");
                if (cells != null && cells.Count > 0)
                {
                    Int32 albumId = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns["asset"]["album_id"], "0"));
                    Int16 volumeIndex = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["asset"]["volume_index"], "0"));
                    Int16 trackIndex = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns["asset"]["track_index"], "0"));
                    String isrcId = String.Empty;

                    if (!CatalogContext.Instance.Albums.Exists(a => a.Id == albumId))
                    {
                        continue;
                    }

                    Album album = CatalogContext.Instance.Albums.FirstOrDefault(a => a.Id == albumId);
                    if (album == null)
                    {
                        if (_mainFormViewModel != null)
                        {
                            _mainFormViewModel.InputProgressBarValue++;
                        }
                        continue;
                    }

                    if (album.Assets == null)
                    {
                        album.Assets = new Dictionary<Int16, Dictionary<Int16, String>>();
                    }

                    if (volumeIndex <= 0)
                    {
                        if (_mainFormViewModel != null)
                        {
                            _mainFormViewModel.InputProgressBarValue++;
                        }
                        continue;
                    }

                    if (!album.Assets.ContainsKey(volumeIndex))
                    {
                        album.Assets[volumeIndex] = new Dictionary<Int16, String>();
                    }

                    if (trackIndex <= 0 || trackIndex > 99)
                    {
                        if (_mainFormViewModel != null)
                        {
                            _mainFormViewModel.InputProgressBarValue++;
                        }
                        continue;
                    }

                    // Id is mandatory and has a minimal standardized length
                    isrcId = CellContentWizard(cells, _worksheetColumns["asset"]["isrc_id"]);
                    if (String.IsNullOrEmpty(isrcId) || isrcId.Length < 12 || isrcId.Length > 15)
                    {
                        if (_mainFormViewModel != null)
                        {
                            _mainFormViewModel.InputProgressBarValue++;
                        }
                        continue;
                    }

                    // Conditions ok to add asset to album
                    album.Assets[volumeIndex][trackIndex] = isrcId;
                }

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

        // Complete fields that rely on posterior data, e.g. on Assets
        private void FinalizeAlbums()
        {
            if (CatalogContext.Instance.Albums == null)
            {
                return;
            }

            foreach (Album album in CatalogContext.Instance.Albums)
            {
                // Primary Artist field automatic computation
                Dictionary<Int32, int> artistFrequencies = new Dictionary<Int32, int>(); // Keys are primary artists, Values are occurrences in tracks
                if (album.PrimaryArtistId == null)
                {
                    foreach (KeyValuePair<Int16, Dictionary<Int16, String>> volume in album.Assets)
                    {
                        foreach (KeyValuePair<Int16, String> track in volume.Value)
                        {
                            Isrc isrc = CatalogContext.Instance.Isrcs.FirstOrDefault(e => e.Id.CompareTo(track.Value) == 0);
                            if (isrc == null || String.IsNullOrEmpty(isrc.Id))
                            {
                                continue;
                            }
                            foreach (KeyValuePair<Int32, Dictionary<Role, Quality>> contributor in isrc.Contributors)
                            {
                                foreach (KeyValuePair<Role, Quality> roleQuality in contributor.Value)
                                {
                                    if (roleQuality.Key.Reference == Role.QualifiedName.Performer && roleQuality.Value.Name.ToLower().CompareTo("primary") == 0)
                                    {
                                        // Found a Primary Performer occurrence!
                                        if (artistFrequencies.ContainsKey(contributor.Key))
                                        {
                                            artistFrequencies[contributor.Key]++;
                                        }
                                        else
                                        {
                                            artistFrequencies[contributor.Key] = 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Now select the most frequent primary performer
                    int max = artistFrequencies.Values.ToList().Max();
                    album.PrimaryArtistId = artistFrequencies.Keys.ToList().FirstOrDefault(e => artistFrequencies[e] == max);
                }

                // Total discs
                album.TotalDiscs = album.Assets.Keys.ToList().Max();

                // C & P Copyrights
                if (String.IsNullOrEmpty(album.CName))
                {
                    album.CName = album.PName;
                }
                if (String.IsNullOrEmpty(album.PName))
                {
                    album.PName = album.CName;
                }
                if (album.CYear == null)
                {
                    album.CYear = album.PYear;
                }
                if (album.PYear == null)
                {
                    album.PYear = album.CYear;
                }
            }
        }

        /// <summary>
        /// Complete such fields as Catalog Tier (default value), after albums parsing
        /// </summary>
        private void FinalizeIsrcs()
        {
            if (CatalogContext.Instance.Isrcs == null || CatalogContext.Instance.Albums == null)
            {
                return;
            }

            // This update may raise undesired affectations if the isrc is present in more than one album (compilations, etc.) 
            foreach (Isrc isrc in CatalogContext.Instance.Isrcs)
            {
                if (isrc.Tier == null)
                {
                    isrc.Tier = (CatalogContext.Instance.Albums.FirstOrDefault(a => a.Assets.Values.ToList().Exists(v => v.Values.ToList().Contains(isrc.Id)))).Tier;
                }
            }
        }
    }
}
