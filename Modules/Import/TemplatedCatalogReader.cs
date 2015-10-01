/*
 *  Babel Meta - babelmeta.com
 * 
 *  The present software is licensed according to the MIT licence.
 *  Copyright (c) 2015 Romain Carbou (romain@babelmeta.com)
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE. 
 */

using BabelMeta.AppConfig;
using BabelMeta.Helpers;
using BabelMeta.Model;
using BabelMeta.Model.Config;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml;
using Action = System.Action;
using Excel = Microsoft.Office.Interop.Excel;

namespace BabelMeta.Modules.Import
{
    /// <summary>
    /// This default Input format consists in a pre-formatted table of several worksheets, with limited column flexibility.
    /// Formats currently supported are Excel Workbook or Excel 2003 XML (more efficient for *small* files).
    /// Mandatory worksheets: lang, role, tag, artist, work, asset, featuring, album, track.
    /// Mandatory columns *MUST NOT* be removed or renamed but *MAY* have any position in their worksheet. 
    /// Additional columns with non-ambiguous names *MAY* be added everywhere in any worksheet.
    /// </summary>
    public class TemplatedCatalogReader : ICatalogReader
    {
        /// <summary>
        /// Excel format main object
        /// </summary>
        private Excel.Application _excelApplication;
        private Workbook _excelDocument;
        private Dictionary<String, int> _excelLastRow; // Worksheet-wise
        private Dictionary<String, int> _excelLastColumn; // Worksheet-wise

        /// <summary>
        /// Xml format main object
        /// </summary>
        private XmlDocument _xmlDocument;

        /// <summary>
        /// 0: Excel XML 2003
        /// 1: Excel Workbook .xlsx
        /// </summary>
        private FileFormatType _fileFormatType;

        private MainFormViewModel _mainFormViewModel;

        private const String SettingsWorksheetName = "SETTINGS";
        private const String LangWorksheetName = "lang";
        private const String TagWorksheetName = "tag";
        private const String RoleWorksheetName = "role";
        private const String QualityWorksheetName = "quality";
        private const String ArtistWorksheetName = "artist";
        private const String WorkWorksheetName = "work";
        private const String AssetWorksheetName = "asset";
        private const String AlbumWorksheetName = "album";
        private const String TrackWorksheetName = "track";

        private const String LocalDbFieldName = "local_db";
        private const String PartnerDbFieldName = "partner_db";
        private const String IsrcIdFieldName = "isrc_id";
        private const String WorkIdFieldName = "work_id";

        private const int InitPayload = 7; // Conventional value reflecting the init payload - feel free to put any reasonable value

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
        private readonly Dictionary<String, object> _worksheets;

        /// <summary>
        /// Column indexes by worksheets (permits flexibility in the worksheet structure)
        /// </summary>
        private readonly Dictionary<String, Dictionary<String, int>> _worksheetColumns;

        private List<object> _settings;
        private List<object> _langs;
        private List<object> _tags;
        private List<object> _roles;
        private List<object> _qualities;
        private List<object> _artists;
        private List<object> _works;
        private List<object> _assets;
        private List<object> _albums;
        private List<object> _tracks;

        private static TemplatedCatalogReader _instance;

        private TemplatedCatalogReader()
        {
            _worksheets = new Dictionary<String, object>();
            _worksheetColumns = new Dictionary<String, Dictionary<String, int>>();
        }

        /// <summary>
        /// Singleton pattern representing the reader service instance.
        /// </summary>
        public static TemplatedCatalogReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TemplatedCatalogReader();

                }
                return _instance;
            }
        }

        /// <summary>
        /// Main parsing method implementing ICatalogReader interface.
        /// </summary>
        /// <param name="ofd"></param>
        /// <param name="formatType"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<ReturnCode> Parse(OpenFileDialog ofd, FileFormatType formatType, MainFormViewModel viewModel = null)
        {
            if (ofd == null)
            {
                return ReturnCode.ModulesImportDefaultParseEmptyStream;
            }

            if (formatType == FileFormatType.ExcelWorkbook || formatType == FileFormatType.ExcelXml2003)
            {
                _fileFormatType = formatType;
            }
            else 
            {
                return ReturnCode.ModulesImportDefaultParseUnknownFormat;
            }

            switch (_fileFormatType)
            {
                case FileFormatType.ExcelWorkbook:
                    _excelLastRow = new Dictionary<String, int>();
                    _excelLastColumn = new Dictionary<String, int>();
                    if (String.IsNullOrEmpty(ofd.FileName))
                    {
                        return ReturnCode.ModulesImportDefaultParseEmptyStream;
                    }
                    _excelApplication = new Excel.Application();
                    _excelDocument = _excelApplication.Workbooks.Open(ofd.FileName);
                    if (_excelDocument == null)
                    {
                        return ReturnCode.ModulesImportDefaultParseEmptyStream;
                    }
                    break;
                case FileFormatType.ExcelXml2003:
                    try
                    {
                        Stream s = ofd.OpenFile();
                        _xmlDocument = new XmlDocument();
                        _xmlDocument.Load(s);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, "TemplatedCatalogReader.Parse, case=FileFormatType.ExcelXml2003, exception=" + ex.Message);
                        Notify("The XML parsing failed.");
                        return ReturnCode.ModulesImportDefaultParseEmptyStream;
                    }
            }

            if (viewModel != null)
            {
                _mainFormViewModel = viewModel;
                _mainFormViewModel.InputProgressBarValue = 0;
                _mainFormViewModel.InputProgressBarMax = InitPayload + 1; // initial payload and 'symbolic' last bit of payload before last return 
            }

            if (!(await IsValidWorkbook()))
            {
                Notify("Invalid Workbook.");
                return ReturnCode.ModulesImportDefaultParseInvalidWorkbook;
            }

            if (_mainFormViewModel != null)
            {
                if (_langs != null) _mainFormViewModel.InputProgressBarMax += _langs.Count;
                if (_tags != null) _mainFormViewModel.InputProgressBarMax += _tags.Count;
                if (_roles != null) _mainFormViewModel.InputProgressBarMax += _roles.Count;
                if (_qualities != null) _mainFormViewModel.InputProgressBarMax += _qualities.Count;
                if (_artists != null) _mainFormViewModel.InputProgressBarMax += _artists.Count;
                if (_works != null) _mainFormViewModel.InputProgressBarMax += _works.Count;
                if (_assets != null) _mainFormViewModel.InputProgressBarMax += _assets.Count;
                if (_albums != null) _mainFormViewModel.InputProgressBarMax += _albums.Count;
                if (_tracks != null) _mainFormViewModel.InputProgressBarMax += _tracks.Count;

                // Init phase 'completed'
                _mainFormViewModel.InputProgressBarValue = InitPayload;

                // Langs were already parsed
                if (_langs != null) _mainFormViewModel.InputProgressBarValue += _langs.Count;

            }

            await ParseTags();
            await ParseRoles();
            await ParseQualities();
            await ParseArtists();
            await ParseWorks();
            await ParseAssets();
            await ParseAlbums();
            await ParseTracks();

            Notify("Finalization...");
            FinalizeAlbums();
            FinalizeAssets();

            if (_mainFormViewModel != null)
            {
                if (_mainFormViewModel.FilterArtistChecked)
                {
                    CatalogContext.Instance.FilterUnusedArtists();
                }
                else if (_mainFormViewModel.FilterWorkChecked)
                {
                    CatalogContext.Instance.FilterUnusedWorks();
                }
                _mainFormViewModel.InputProgressBarValue = _mainFormViewModel.InputProgressBarMax;
            }

            CatalogContext.Instance.Initialized = true;
            Notify("Parsing completed.");
            return ReturnCode.Ok;
        }

        private Dictionary<int,object> GetHeader(String worksheetName)
        {
            Dictionary<int, object> map = null;
            switch (_fileFormatType)
            {
                case FileFormatType.ExcelWorkbook:
                    map = CellMapByRow(1, worksheetName);
                    break;
                case FileFormatType.ExcelXml2003:
                    if (_worksheets[worksheetName] == null)
                    { 
                        return null; 
                    }
                    map = CellMapByRow(((XmlNode)(_worksheets[worksheetName])).ChildNodes.Cast<XmlNode>().FirstOrDefault(n => String.Compare(n.Name, OfficeXml.WorksheetRow, StringComparison.Ordinal) == 0));
                    break;
            }
            return map;
        }

        /// <summary>
        /// Checks that Workbook embarks expected Worksheets
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsValidWorkbook()
        {
            if  (
                    (
                        _fileFormatType == FileFormatType.ExcelXml2003 
                        && (_xmlDocument == null || _xmlDocument.DocumentElement == null)
                    )
                    || 
                    (_fileFormatType == FileFormatType.ExcelWorkbook && _excelDocument == null)
                )
            {
                return false;
            }

            // Check worksheets
            if (!ExistsWorksheet(SettingsWorksheetName)) return false;
            if (!ExistsWorksheet(LangWorksheetName)) return false;
            if (!ExistsWorksheet(TagWorksheetName)) return false;
            if (!ExistsWorksheet(RoleWorksheetName)) return false;
            if (!ExistsWorksheet(QualityWorksheetName)) return false;
            if (!ExistsWorksheet(ArtistWorksheetName)) return false;
            if (!ExistsWorksheet(WorkWorksheetName)) return false;
            if (!ExistsWorksheet(AssetWorksheetName)) return false;
            if (!ExistsWorksheet(AlbumWorksheetName)) return false;
            if (!ExistsWorksheet(TrackWorksheetName)) return false;

            // Check columns and identify their indexes
            // Don't change the order in the following section so as to enable referential integrity check.

            // SETTINGS
            var map = GetHeader(SettingsWorksheetName);
            if (!ExistsCellValueInRow("parameter", map, SettingsWorksheetName)) return false;
            if (!ExistsCellValueInRow("value", map, SettingsWorksheetName)) return false;
            await Task.Run(() =>
            {
                _settings = WorksheetActiveRows(SettingsWorksheetName);
            });
            await ParseSettings(); // *must* be parsed first!

            // lang
            map = GetHeader(LangWorksheetName);
            if (!ExistsCellValueInRow(LocalDbFieldName, map, LangWorksheetName)) return false;
            if (!ExistsCellValueInRow(PartnerDbFieldName, map, LangWorksheetName)) return false;
            if (!ExistsCellValueInRow("long_name", map, LangWorksheetName)) return false;
            if (!ExistsCellValueInRow("short_name", map, LangWorksheetName)) return false;
            if (!ExistsCellValueInRow("default", map, LangWorksheetName)) return false;
            await Task.Run(() =>
            {
                _langs = WorksheetActiveRows(LangWorksheetName);
            });
            await ParseLangs(); // *must* be parsed second, because column names can be language-dependent!

            // tag
            map = GetHeader(TagWorksheetName);
            if (!ExistsCellValueInRow(LocalDbFieldName, map, TagWorksheetName)) return false;
            if (!ExistsCellValueInRow(PartnerDbFieldName, map, TagWorksheetName)) return false;
            if (!ExistsCellValueInRow("tag_name", map, TagWorksheetName)) return false;
            await Task.Run(() =>
            {
                _tags = WorksheetActiveRows(TagWorksheetName);
            });

            // role
            map = GetHeader(RoleWorksheetName);
            if (!ExistsCellValueInRow(LocalDbFieldName, map, RoleWorksheetName)) return false;
            if (!ExistsCellValueInRow(PartnerDbFieldName, map, RoleWorksheetName)) return false;
            if (!ExistsCellValueInRow("role_name", map, RoleWorksheetName)) return false;
            await Task.Run(() =>
            {
                _roles = WorksheetActiveRows(RoleWorksheetName);
            });

            // quality
            map = GetHeader(QualityWorksheetName);
            if (!ExistsCellValueInRow(LocalDbFieldName, map, QualityWorksheetName)) return false;
            if (!ExistsCellValueInRow(PartnerDbFieldName, map, QualityWorksheetName)) return false;
            if (!ExistsCellValueInRow("quality_name", map, QualityWorksheetName)) return false;
            await Task.Run(() =>
            {
                _qualities = WorksheetActiveRows(QualityWorksheetName);
            });

            // artist
            map = GetHeader(ArtistWorksheetName);
            if (!ExistsCellValueInRow(LocalDbFieldName, map, ArtistWorksheetName)) return false;
            if (!ExistsCellValueInRow(PartnerDbFieldName, map, ArtistWorksheetName)) return false;
            if (!ExistsCellValueInRow("id", map, ArtistWorksheetName)) return false;
            if (!ExistsCellValueInRow("birth", map, ArtistWorksheetName)) return false;
            if (!ExistsCellValueInRow("death", map, ArtistWorksheetName)) return false;
            foreach (var lang in CatalogContext.Instance.Langs)
            {
                if (!ExistsCellValueInRow("lastname_" + lang.ShortName, map, ArtistWorksheetName)) return false;
                if (!ExistsCellValueInRow("firstname_" + lang.ShortName, map, ArtistWorksheetName)) return false;
            }
            await Task.Run(() =>
            {
                _artists = WorksheetActiveRows(ArtistWorksheetName);
            });

            // work
            map = GetHeader(WorkWorksheetName);
            if (!ExistsCellValueInRow(LocalDbFieldName, map, WorkWorksheetName)) return false;
            if (!ExistsCellValueInRow(PartnerDbFieldName, map, WorkWorksheetName)) return false;
            if (!ExistsCellValueInRow("id", map, WorkWorksheetName)) return false;
            if (!ExistsCellValueInRow("id_parent", map, WorkWorksheetName)) return false;
            if (!ExistsCellValueInRow("catalog_number", map, WorkWorksheetName)) return false;
            if (!ExistsCellValueInRow("key", map, WorkWorksheetName)) return false;
            if (!ExistsCellValueInRow("year", map, WorkWorksheetName)) return false;
            var iWork = 1;
            while   (
                        ExistsCellValueInRow("id_contributor" + iWork, map, WorkWorksheetName)
                        && ExistsCellValueInRow("role_contributor" + iWork, map, WorkWorksheetName)
                    )
            {
                iWork++;
            }
            if (iWork == 1)
            {
                return false;
            }
            if (!ExistsCellValueInRow("movement_number", map, WorkWorksheetName)) return false;
            foreach (var lang in CatalogContext.Instance.Langs)
            {
                if (!ExistsCellValueInRow("title_" + lang.ShortName, map, WorkWorksheetName)) return false;
                if (!ExistsCellValueInRow("movement_title_" + lang.ShortName, map, WorkWorksheetName)) return false;
            }
            await Task.Run(() =>
            {
                _works = WorksheetActiveRows(WorkWorksheetName);
            });

            // asset
            map = GetHeader(AssetWorksheetName);
            if (!ExistsCellValueInRow(LocalDbFieldName, map, AssetWorksheetName)) return false;
            if (!ExistsCellValueInRow(PartnerDbFieldName, map, AssetWorksheetName)) return false;
            if (!ExistsCellValueInRow(IsrcIdFieldName, map, AssetWorksheetName)) return false;
            if (!ExistsCellValueInRow(WorkIdFieldName, map, AssetWorksheetName)) return false;
            var iAsset = 1;
            while   (
                        ExistsCellValueInRow("id_contributor" + iAsset, map, AssetWorksheetName)
                        && ExistsCellValueInRow("role_contributor" + iAsset, map, AssetWorksheetName)
                        && ExistsCellValueInRow("quality_contributor" + iAsset, map, AssetWorksheetName)
                    )
            {
                iAsset++;
            }
            if (iAsset == 1)
            {
                return false;
            }
            if (!ExistsCellValueInRow("c_name", map, AssetWorksheetName)) return false;
            if (!ExistsCellValueInRow("c_year", map, AssetWorksheetName)) return false;
            if (!ExistsCellValueInRow("p_name", map, AssetWorksheetName)) return false;
            if (!ExistsCellValueInRow("p_year", map, AssetWorksheetName)) return false;
            if (!ExistsCellValueInRow("recording_location", map, AssetWorksheetName)) return false;
            if (!ExistsCellValueInRow("recording_year", map, AssetWorksheetName)) return false;
            if (!ExistsCellValueInRow("available_separately", map, AssetWorksheetName)) return false;
            if (!ExistsCellValueInRow("catalog_tier", map, AssetWorksheetName)) return false;
            await Task.Run(() =>
            {
                _assets = WorksheetActiveRows(AssetWorksheetName);
            });

            // album
            map = GetHeader(AlbumWorksheetName);
            if (!ExistsCellValueInRow(LocalDbFieldName, map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow(PartnerDbFieldName, map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("album_id", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("action", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("c_name", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("c_year", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("p_name", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("p_year", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("recording_location", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("recording_year", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("catalog_tier", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("consumer_release_date", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("label", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("reference", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("ean", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("genre", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("subgenre", map, AlbumWorksheetName)) return false;
            if (CatalogContext.Instance.Langs.Any(lang => !ExistsCellValueInRow("title_" + lang.ShortName, map, AlbumWorksheetName)))
            {
                return false;
            }
            if (!ExistsCellValueInRow("primary_artist", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("recording_location", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("recording_year", map, AlbumWorksheetName)) return false;
            if (!ExistsCellValueInRow("redeliver", map, AlbumWorksheetName)) return false;
            await Task.Run(() =>
            {
                _albums = WorksheetActiveRows(AlbumWorksheetName);
            });

            // track
            map = GetHeader(TrackWorksheetName);
            if (!ExistsCellValueInRow(LocalDbFieldName, map, TrackWorksheetName)) return false;
            if (!ExistsCellValueInRow(PartnerDbFieldName, map, TrackWorksheetName)) return false;
            if (!ExistsCellValueInRow("album_id", map, TrackWorksheetName)) return false;
            if (!ExistsCellValueInRow("volume_index", map, TrackWorksheetName)) return false;
            if (!ExistsCellValueInRow("track_index", map, TrackWorksheetName)) return false;
            if (!ExistsCellValueInRow(IsrcIdFieldName, map, TrackWorksheetName)) return false;
            await Task.Run(() =>
            {
                _tracks = WorksheetActiveRows(TrackWorksheetName);
            });

            return true;
        }

        /// <summary>
        /// Returns a dictionary of cell objects (Key = column)
        /// </summary>
        /// <param name="row">
        ///     For Excel .xls(x), represents the row number
        ///     For Excel XML, represents the XmlNode row object
        /// </param>
        /// <param name="worksheetName">
        ///     Used only in Excel .xls(x) format.
        /// </param>
        /// <returns></returns>
        private Dictionary<int, object> CellMapByRow(object row, String worksheetName = "")
        {
            if (row == null)
            {
                return null;
            }

            var map = new Dictionary<int, object>();
            var index = 1;

            switch (_fileFormatType)
            {
                case FileFormatType.ExcelWorkbook:
                    var rowIndex = (int)row;
                    if (rowIndex < 1)
                    {
                        return null;
                    }
                    for (; index <= _excelLastColumn[worksheetName]; index++)
                    {
                        // try-catch within the loop is necessary due to COM exceptions but has an influence on performances for large files. 
                        try
                        {
                            Range cell = ((_Worksheet)_worksheets[worksheetName]).Cells[rowIndex, index];
                            if (cell != null && cell.Value != null && !String.IsNullOrEmpty(cell.Value.ToString()))
                            {
                                map[index] = cell;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(this, "TemplatedCatalogReader.CellMapByRow, worksheet=" + worksheetName + ", line=" + rowIndex + ", exception=" + ex.Message);
                            Notify(String.Format("A problem occurred while trying to read worksheet {0}, line {1}, column {2}.", worksheetName, rowIndex, index));
                        }
                    }
                    break;

                case FileFormatType.ExcelXml2003:
                    foreach (var cell in ((XmlNode)row).ChildNodes.Cast<XmlNode>().Where(n => String.Compare(n.Name, OfficeXml.WorksheetCell, StringComparison.Ordinal) == 0))
                    {
                        // Cell index has priority over deduced index
                        if (cell.Attributes[OfficeXml.CellIndex] != null)
                        {
                            int tryIndex;
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
            if (!_worksheets.ContainsKey(worksheetName) || _worksheets[worksheetName] == null)
            {
                return null;
            }

            if  (
                    _excelLastRow == null 
                    || !_excelLastRow.ContainsKey(worksheetName) 
                    || _excelLastColumn == null 
                    || !_excelLastColumn.ContainsKey(worksheetName)
                )
            {
                return null;
            }

            List<object> rows = null; 
            var filteredRows = new List<object>();

            switch (_fileFormatType)
            {
                case FileFormatType.ExcelWorkbook:
                    rows = Enumerable.Range(1, _excelLastRow[worksheetName]).Cast<object>().ToList();
                    break;
                case FileFormatType.ExcelXml2003:
                    rows = ((XmlNode)_worksheets[worksheetName]).ChildNodes.Cast<object>()
                    .Where(r => String.Compare(((XmlNode)r).Name, OfficeXml.WorksheetRow, StringComparison.Ordinal) == 0)
                    .ToList();
                    break;
            }

            // SETTINGS: return all lines
            if (String.Compare(worksheetName, SettingsWorksheetName, StringComparison.Ordinal) == 0)
            {
                if (rows != null) rows.RemoveAt(0); // Header line
                return rows;
            }

            // Any other tab
            if (rows == null)
            {
                return filteredRows;
            }
            foreach (var row in rows)
            {
                var map = CellMapByRow(row, worksheetName);

                // Any worksheet other than SETTINGS...
                var local = false;
                var partner = false;

                switch (_fileFormatType)
                {
                    case FileFormatType.ExcelWorkbook:
                        local = map.ContainsKey(_worksheetColumns[worksheetName][LocalDbFieldName]) 
                            && String.Compare(((Range)map[_worksheetColumns[worksheetName][LocalDbFieldName]]).Value.ToString().ToLower(), "active", StringComparison.Ordinal) == 0;
                        partner = map.ContainsKey(_worksheetColumns[worksheetName][PartnerDbFieldName]) 
                            && String.Compare(((Range)map[_worksheetColumns[worksheetName][PartnerDbFieldName]]).Value.ToString().ToLower(), "active", StringComparison.Ordinal) == 0;
                        break;

                    case FileFormatType.ExcelXml2003:
                        local = map.ContainsKey(_worksheetColumns[worksheetName][LocalDbFieldName]) && String.Compare(((XmlNode)map[_worksheetColumns[worksheetName][LocalDbFieldName]]).InnerText.ToLower(), "active", StringComparison.Ordinal) == 0;
                        partner = map.ContainsKey(_worksheetColumns[worksheetName][PartnerDbFieldName]) && String.Compare(((XmlNode)map[_worksheetColumns[worksheetName][PartnerDbFieldName]]).InnerText.ToLower(), "active", StringComparison.Ordinal) == 0;
                        break;
                }

                if  (
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
        private bool ExistsCellValueInRow(String cellValue, Dictionary<int, object> map, String worksheetName = "")
        {
            if (String.IsNullOrEmpty(cellValue) || map == null || map.Count == 0)
            {
                return false;
            }

            KeyValuePair<int, object>? element = null;

            switch (_fileFormatType)
            {
                case FileFormatType.ExcelWorkbook:
                    element = map.FirstOrDefault(e => 
                        String.Compare(((Range)e.Value).Value.ToString().Trim(), cellValue, StringComparison.Ordinal) == 0);
                    break;

                case FileFormatType.ExcelXml2003:
                    element = map.FirstOrDefault(e => 
                        String.Compare(((XmlNode)e.Value).InnerText.Trim(), cellValue, StringComparison.Ordinal) == 0);
                    break;
            }

            var exists = (element != null) && ((KeyValuePair<int, object>)element).Value != null;
            if (exists && !String.IsNullOrEmpty(worksheetName) && _worksheetColumns[worksheetName] != null)
            {
                var index = ((KeyValuePair<int, object>)element).Key;
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
            const int maxSheets = 64; // Security

            if (_worksheets.ContainsKey(worksheetName))
            {
                return true;
            }

            object table = null;

            switch (_fileFormatType)
            {
                case FileFormatType.ExcelWorkbook:
                    var sheets = _excelDocument.Worksheets;
                    var sheetsCount = sheets.Count;
                    var index = 1;
                    while (index <= sheetsCount && index <= maxSheets)
                    {
                        var excelWorksheet = (_Worksheet)sheets.Item[index];
                        if (excelWorksheet != null)
                        {
                            if (!String.IsNullOrEmpty(excelWorksheet.Name) && String.Compare(excelWorksheet.Name, worksheetName, StringComparison.Ordinal) == 0)
                            {
                                table = excelWorksheet;
                                var rngLast = excelWorksheet.Range["A1"].SpecialCells(XlCellType.xlCellTypeLastCell);
                                _excelLastRow[worksheetName] = rngLast.Row;
                                _excelLastColumn[worksheetName] = rngLast.Column;
                                break;
                            }
                        }
                        index++;
                    }
                    break;

                case FileFormatType.ExcelXml2003:
                    if  (
                            _xmlDocument == null 
                            || _xmlDocument.DocumentElement == null 
                        )
                    {
                        return false;
                    }
                    try
                    {
                        var xmlWorkbook = _xmlDocument.DocumentElement.ChildNodes.Cast<XmlNode>();
                        var xmlWorksheet = xmlWorkbook
                            .FirstOrDefault(
                                n =>
                                n.Name == OfficeXml.Worksheet
                                && n.Attributes[OfficeXml.WorksheetName] != null
                                && String.Compare(n.Attributes[OfficeXml.WorksheetName].InnerText, worksheetName, StringComparison.Ordinal) == 0
                            );

                        if (xmlWorksheet == null)
                        {
                            return false;
                        }
                        table = xmlWorksheet.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == OfficeXml.WorksheetTable);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("TemplatedCatalogReader.ExistsWorksheet, case=FileFormatType.ExcelXml2003, exception=" + ex.Message);
                        Instance.Notify(String.Format("A problem occurred while trying to check worksheet {0}.", worksheetName));
                        return false;
                    }
            }

            var exists = table != null;

            if (!exists)
            {
                return false;
            }

            // Add table element of the worksheet
            _worksheets[worksheetName] = table;
            _worksheetColumns[worksheetName] = new Dictionary<String, int>();

            return true;
        }

        /// <summary>
        /// Retrieves cell value according to the different implementations. Heart of the multi-format support.
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="columnIndex"></param>
        /// <param name="trim"></param>
        /// <param name="defaultValue">so as to adapt return type, if a number e.g. is expected</param>
        /// <returns></returns>
        private String CellContentWizard(Dictionary<int, object> cells, int columnIndex, String defaultValue = "", bool trim = true)
        {
            // _formatType nullity is not tested for the sake of performance
            if (cells == null || cells.Count == 0 || columnIndex <= 0)
            {
                return defaultValue;
            }

            var o = (cells.Keys.ToList().Contains(columnIndex))
                ? cells.FirstOrDefault(c => c.Key == columnIndex).Value
                : null;

            if (o == null)
            {
                return defaultValue;
            }

            String v;
            switch (_fileFormatType)
            {
                case FileFormatType.ExcelWorkbook:
                    v = (trim) ? ((Range)o).Value.ToString().Trim() : ((Range)o).Value.ToString();
                    return (String.IsNullOrEmpty(v)) ? defaultValue : v;

                case FileFormatType.ExcelXml2003:
                    v = (trim) ? ((XmlNode)(o)).InnerText.Trim() : ((XmlNode)(o)).InnerText;
                    return (String.IsNullOrEmpty(v)) ? defaultValue : v;

                default:
                    return defaultValue;
            }
        }

        /// <summary>
        /// Settings parser
        /// </summary>
        private async Task ParseSettings()
        {
            if (_settings == null)
            {
                return;
            }

            foreach (var row in _settings)
            {
                Dictionary<int, object> cells = null;
                await Task.Run(() =>
                {
                    cells = CellMapByRow(row, SettingsWorksheetName);
                });

                if (cells == null || cells.Count <= 0)
                {
                    continue;
                }

                var parameter = CellContentWizard(cells, _worksheetColumns[SettingsWorksheetName]["parameter"]);
                var value = CellContentWizard(cells, _worksheetColumns[SettingsWorksheetName]["value"]);

                if (String.IsNullOrEmpty(parameter))
                {
                    continue;
                }

                if (CatalogContext.Instance.Settings == null)
                {
                    CatalogContext.Instance.Settings = new CatalogSettings();
                }

                var settings = CatalogContext.Instance.Settings;

                switch (parameter.ToUpper())
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
                                case AlbumWorksheetName: settings.FormatDefault = ProductFormat.Album; break;
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
                    case "ASSET_AVAILABLE_SEPARATELY": settings.AvailableSeparatelyDefault = (String.IsNullOrEmpty(value) || String.Compare(value.Trim().ToLower(), "no", StringComparison.Ordinal) != 0); break;
                }
            }
        }

        /// <summary>
        /// Langs parser
        /// Since other worksheet rows count is unknown at this stage, do not update progress bar inside the function
        /// </summary>
        private async Task ParseLangs()
        {
            if (_langs == null)
            {
                return;
            }

            foreach (var row in _langs)
            {
                Dictionary<int, object> cells = null;
                await Task.Run(() =>
                {
                    cells = CellMapByRow(row, LangWorksheetName);
                });

                if (cells == null || cells.Count <= 0)
                {
                    continue;
                }

                var lang = new Lang
                {
                    LongName = CellContentWizard(cells, _worksheetColumns[LangWorksheetName]["long_name"]),
                    ShortName = CellContentWizard(cells, _worksheetColumns[LangWorksheetName]["short_name"]),
                    IsDefault = String.Compare(CellContentWizard(cells, _worksheetColumns[LangWorksheetName]["default"]).ToLower(), "yes", StringComparison.Ordinal) == 0,
                };

                // Short name is mandatory
                if (!String.IsNullOrEmpty(lang.ShortName))
                {
                    CatalogContext.Instance.Langs.Add(lang);
                }
            }
        }

        /// <summary>
        /// Tags parser
        /// </summary>
        private async Task ParseTags()
        {
            Notify("Parsing tags.");
            if (_tags == null)
            {
                return;
            }

            foreach (var row in _tags)
            {
                Dictionary<int, object> cells = null;
                await Task.Run(() =>
                {
                    cells = CellMapByRow(row, TagWorksheetName);
                });

                if (cells != null && cells.Count > 0)
                {
                    var tag = new Tag
                    {
                        Name = CellContentWizard(cells, _worksheetColumns[TagWorksheetName]["tag_name"]),
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
        private async Task ParseRoles()
        {
            Notify("Parsing roles.");
            if (_roles == null)
            {
                return;
            }

            foreach (var row in _roles)
            {
                Dictionary<int, object> cells = null;
                await Task.Run(() =>
                {
                    cells = CellMapByRow(row, RoleWorksheetName);
                });

                if (cells != null && cells.Count > 0)
                {
                    var role = new Role
                    {
                        Name = CellContentWizard(cells, _worksheetColumns[RoleWorksheetName]["role_name"]).ToLower(),
                    };

                    CatalogContext.Instance.Roles.Add(role);
                }

                if (_mainFormViewModel == null)
                {
                    continue;
                }

                _mainFormViewModel.InputProgressBarValue++;
            }
        }

        /// <summary>
        /// Qualities parser
        /// </summary>
        /// <returns></returns>
        private async Task ParseQualities()
        {
            Notify("Parsing qualities.");
            if (_qualities == null)
            {
                return;
            }

            foreach (var row in _qualities)
            {
                Dictionary<int, object> cells = null;
                await Task.Run(() =>
                {
                    cells = CellMapByRow(row, QualityWorksheetName);
                });

                if (cells != null && cells.Count > 0)
                {
                    var quality = new Quality
                    {
                        Name = CellContentWizard(cells, _worksheetColumns[QualityWorksheetName]["quality_name"]),
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

        /// <summary>
        /// Artists parser
        /// </summary>
        /// <returns></returns>
        private async Task ParseArtists()
        {
            Notify("Parsing artists.");
            if (_artists == null)
            {
                return;
            }

            foreach (var row in _artists)
            {
                Dictionary<int, object> cells = null;
                await Task.Run(() =>
                {
                    cells = CellMapByRow(row, ArtistWorksheetName);
                });

                if (cells != null && cells.Count > 0)
                {
                    var artist = new Artist
                    {
                        Id = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns[ArtistWorksheetName]["id"], "0")),
                        Birth = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[ArtistWorksheetName]["birth"], "0")),
                        Death = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[ArtistWorksheetName]["death"], "0")),
                        LastName = new Dictionary<String, String>
                        {
                            {CatalogContext.Instance.DefaultLang.ShortName, CellContentWizard(cells, _worksheetColumns[ArtistWorksheetName]["lastname_" + CatalogContext.Instance.DefaultLang.ShortName])},
                        },
                        FirstName = new Dictionary<String, String>
                        {
                            {CatalogContext.Instance.DefaultLang.ShortName, CellContentWizard(cells, _worksheetColumns[ArtistWorksheetName]["firstname_" + CatalogContext.Instance.DefaultLang.ShortName])},
                        },
                    };

                    if (artist.Id <= 0 || String.IsNullOrEmpty(artist.LastName[CatalogContext.Instance.DefaultLang.ShortName]))
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
                    var otherLangs = CatalogContext.Instance.Langs.Where(l => l != CatalogContext.Instance.DefaultLang).ToList();
                    foreach (var lang in otherLangs)
                    {
                        var last = CellContentWizard(cells, _worksheetColumns[ArtistWorksheetName]["lastname_" + lang.ShortName]);
                        var first = CellContentWizard(cells, _worksheetColumns[ArtistWorksheetName]["firstname_" + lang.ShortName]);

                        if (String.IsNullOrEmpty(last))
                        {
                            last = artist.LastName[CatalogContext.Instance.DefaultLang.ShortName];
                        }
                        if (String.IsNullOrEmpty(first))
                        {
                            first = artist.FirstName[CatalogContext.Instance.DefaultLang.ShortName];
                        }

                        artist.LastName[lang.ShortName] = last;
                        artist.FirstName[lang.ShortName] = first;
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
        private async Task ParseWorks()
        {
            Notify("Parsing works.");
            if (_works == null)
            {
                return;
            }

            foreach (var row in _works)
            {
                Dictionary<int, object> cells = null;
                await Task.Run(() =>
                {
                    cells = CellMapByRow(row, WorkWorksheetName);
                });

                if (cells != null && cells.Count > 0)
                {

                    var shortKey = CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["key"]);
                    var work = new Work
                    {
                        Id = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["id"], "0")),
                        Parent = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["id_parent"], "0")),
                        ClassicalCatalog = CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["catalog_number"]),
                        Tonality = (_shortenedKeys.ContainsKey(shortKey)) ? _shortenedKeys[shortKey] : (Key?)null,
                        Year = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["year"], "0")),
                        MovementNumber = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["movement_number"], "0")),
                        Title = new Dictionary<String, String>
                        {
                            {CatalogContext.Instance.DefaultLang.ShortName, CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["title_" + CatalogContext.Instance.DefaultLang.ShortName])},  
                        },
                        MovementTitle = new Dictionary<String, String>
                        {
                            {CatalogContext.Instance.DefaultLang.ShortName, CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["movement_title_" + CatalogContext.Instance.DefaultLang.ShortName])},  
                        },
                        Contributors = new Dictionary<int, String>(),
                    };

                    if (work.Id <= 0)
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
                    var otherLangs = CatalogContext.Instance.Langs.Where(l => l != CatalogContext.Instance.DefaultLang).ToList();
                    foreach (var lang in otherLangs)
                    {
                        var title = CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["title_" + lang.ShortName]);
                        var movementTitle = CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["movement_title_" + lang.ShortName]);

                        work.Title[lang.ShortName] = (String.IsNullOrEmpty(title)) ? work.Title[CatalogContext.Instance.DefaultLang.ShortName] : title;
                        work.MovementTitle[lang.ShortName] = (String.IsNullOrEmpty(movementTitle)) ? work.MovementTitle[CatalogContext.Instance.DefaultLang.ShortName] : movementTitle;
                    }

                    var i = 1;
                    while   (
                                _worksheetColumns[WorkWorksheetName].ContainsKey("id_contributor" + i)
                                && _worksheetColumns[WorkWorksheetName].ContainsKey("role_contributor" + i)
                            )
                    {
                        var idContributor = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["id_contributor" + i], "0"));
                        var roleName = CellContentWizard(cells, _worksheetColumns[WorkWorksheetName]["role_contributor" + i]).ToLower();
                        var roleContributor = CatalogContext.Instance.Roles.FirstOrDefault(r => String.Compare(r.Name, roleName, StringComparison.Ordinal) == 0);

                        if (idContributor > 0 && roleContributor != null)
                        {
                            work.Contributors[idContributor] = roleContributor.Name;
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
        /// Assets parser
        /// </summary>
        private async Task ParseAssets()
        {
            Notify("Parsing assets.");
            if (_assets == null)
            {
                return;
            }

            foreach (var row in _assets)
            {
                Dictionary<int, object> cells = null;
                await Task.Run(() =>
                {
                    cells = CellMapByRow(row, AssetWorksheetName);
                });

                if (cells != null && cells.Count > 0)
                {
                    var asset = new Asset
                    {
                        Id = CellContentWizard(cells, _worksheetColumns[AssetWorksheetName][IsrcIdFieldName]),
                        Work = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns[AssetWorksheetName][WorkIdFieldName], "0")),
                        CName = CellContentWizard(cells, _worksheetColumns[AssetWorksheetName]["c_name"]),
                        CYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[AssetWorksheetName]["c_year"], "0")),
                        PName = CellContentWizard(cells, _worksheetColumns[AssetWorksheetName]["p_name"]),
                        PYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[AssetWorksheetName]["p_year"], "0")),
                        RecordingLocation = CellContentWizard(cells, _worksheetColumns[AssetWorksheetName]["recording_location"]),
                        RecordingYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[AssetWorksheetName]["recording_year"], "0")),
                        AvailableSeparately = (String.Compare(CellContentWizard(cells, _worksheetColumns[AssetWorksheetName]["available_separately"]).ToLower(), "no", StringComparison.Ordinal) != 0) && CatalogContext.Instance.Settings.AvailableSeparatelyDefault,
                        Contributors = new Dictionary<int, Dictionary<String, String>>(),
                    };

                    if (String.IsNullOrEmpty(asset.Id) || asset.Id.Length < 12 || asset.Id.Length > 15 || asset.Work <= 0)
                    {
                        if (_mainFormViewModel != null)
                        {
                            _mainFormViewModel.InputProgressBarValue++;
                        }
                        continue;
                    }

                    if (String.IsNullOrEmpty(asset.CName))
                    {
                        asset.CName = CatalogContext.Instance.Settings.LabelDefault;
                    }
                    if (asset.CYear < BabelMetaConfig.RecordingYearLowerBound || asset.CYear > BabelMetaConfig.RecordingYearUpperBound)
                    {
                        asset.CYear = null;
                    }
                    if (String.IsNullOrEmpty(asset.PName))
                    {
                        asset.PName = CatalogContext.Instance.Settings.LabelDefault;
                    }
                    if (asset.PYear < BabelMetaConfig.RecordingYearLowerBound || asset.PYear > BabelMetaConfig.RecordingYearUpperBound)
                    {
                        asset.PYear = null;
                    }
                    if (asset.RecordingYear < BabelMetaConfig.RecordingYearLowerBound || asset.RecordingYear > BabelMetaConfig.RecordingYearUpperBound)
                    {
                        asset.RecordingYear = null;
                    }

                    var i = 1;
                    while   (
                                _worksheetColumns[AssetWorksheetName].ContainsKey("id_contributor" + i)
                                && _worksheetColumns[AssetWorksheetName].ContainsKey("role_contributor" + i)
                                && _worksheetColumns[AssetWorksheetName].ContainsKey("quality_contributor" + i)
                            )
                    {
                        var idContributor = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns[AssetWorksheetName]["id_contributor" + i], "0"));
                        var roleContributorName = CellContentWizard(cells, _worksheetColumns[AssetWorksheetName]["role_contributor" + i]).ToLower();
                        var roleContributor = CatalogContext.Instance.Roles.FirstOrDefault(c => String.Compare(c.Name, roleContributorName, StringComparison.Ordinal) == 0);
                        var qualityContributorName = CellContentWizard(cells, _worksheetColumns[AssetWorksheetName]["quality_contributor" + i]);
                        var qualityContributor = CatalogContext.Instance.Qualities.FirstOrDefault(c => String.Compare(c.Name, qualityContributorName, StringComparison.Ordinal) == 0);

                        // Contributor valid
                        if  (
                                idContributor > 0
                                && roleContributor != null
                                && !String.IsNullOrEmpty(roleContributor.Name)
                                && qualityContributor != null
                                && !String.IsNullOrEmpty(qualityContributor.Name)
                            )
                        {
                            if (!asset.Contributors.ContainsKey(idContributor))
                            {
                                asset.Contributors[idContributor] = new Dictionary<String, String>();
                            }
                            asset.Contributors[idContributor][roleContributor.Name] = qualityContributor.Name;
                        }
                        i++;
                    }

                    var tier = CellContentWizard(cells, _worksheetColumns[AssetWorksheetName]["catalog_tier"]).ToLower();
                    if (!String.IsNullOrEmpty(tier))
                    {
                        switch (tier)
                        {
                            case "back": asset.Tier = CatalogTier.Back; break;
                            case "budget": asset.Tier = CatalogTier.Budget; break;
                            case "free": asset.Tier = CatalogTier.Free; break;
                            case "front": asset.Tier = CatalogTier.Front; break;
                            case "mid": asset.Tier = CatalogTier.Mid; break;
                            case "premium": asset.Tier = CatalogTier.Premium; break;
                        }
                    }

                    // If at least asset has one contributor, record entry
                    if (asset.Contributors.Count > 0)
                    {
                        CatalogContext.Instance.Assets.Add(asset);
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
        private async Task ParseAlbums()
        {
            Notify("Parsing albums.");
            if (_albums == null)
            {
                return;
            }

            foreach (var row in _albums)
            {
                Dictionary<int, object> cells = null;
                await Task.Run(() =>
                {
                    cells = CellMapByRow(row, AlbumWorksheetName);
                });

                if (cells != null && cells.Count > 0)
                {
                    {
                        var genreTag = CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["genre"]);
                        var subgenreTag = CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["subgenre"]);
                        Album.ActionType? actionTypeValue = null;
                        switch (CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["action"]).ToLower())
                        {
                            case "insert":
                                actionTypeValue = Album.ActionType.Insert;
                                break;
                            case "update":
                                actionTypeValue = Album.ActionType.Update;
                                break;
                        }
                        var album = new Album
                        {
                            Id = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["album_id"], "0")),
                            ActionTypeValue = actionTypeValue,
                            CName = CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["c_name"]),
                            CYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["c_year"], "0")),
                            PName = CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["p_name"]),
                            PYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["p_year"], "0")),
                            RecordingLocation = CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["recording_location"]),
                            RecordingYear = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["recording_year"], "0")),
                            Owner = CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["label"]),
                            CatalogReference = CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["reference"]),
                            Ean = Convert.ToInt64(CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["ean"], "0")),
                            ConsumerReleaseDate = CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["consumer_release_date"]).ToDateTime(),
                            Title = new Dictionary<String, String>
                        {
                            {CatalogContext.Instance.DefaultLang.ShortName, CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["title_" + CatalogContext.Instance.DefaultLang.ShortName])},
                        },
                            Genre = (CatalogContext.Instance.Tags.Exists(t => String.Compare(t.Name, genreTag, StringComparison.Ordinal) == 0))
                                ? CatalogContext.Instance.Tags.FirstOrDefault(t => String.Compare(t.Name, genreTag, StringComparison.Ordinal) == 0)
                                : null,
                            Subgenre = (!String.IsNullOrEmpty(subgenreTag) && CatalogContext.Instance.Tags.Exists(t => String.Compare(t.Name, subgenreTag, StringComparison.Ordinal) == 0))
                                ? CatalogContext.Instance.Tags.FirstOrDefault(t => String.Compare(t.Name, subgenreTag, StringComparison.Ordinal) == 0)
                                : null,
                            PrimaryArtistId = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["primary_artist"], "0")),
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

                        if (album.ActionTypeValue == null)
                        {
                            album.ActionTypeValue = Album.ActionType.Insert;
                        }

                        if (String.IsNullOrEmpty(album.CName))
                        {
                            album.CName = CatalogContext.Instance.Settings.LabelDefault;
                        }

                        if (album.CYear < BabelMetaConfig.RecordingYearLowerBound || album.CYear > BabelMetaConfig.RecordingYearUpperBound)
                        {
                            album.CYear = null;
                        }

                        if (String.IsNullOrEmpty(album.PName))
                        {
                            album.PName = CatalogContext.Instance.Settings.LabelDefault;
                        }

                        if (album.PYear < BabelMetaConfig.RecordingYearLowerBound || album.PYear > BabelMetaConfig.RecordingYearUpperBound)
                        {
                            album.PYear = null;
                        }

                        if (album.RecordingYear < BabelMetaConfig.RecordingYearLowerBound || album.RecordingYear > BabelMetaConfig.RecordingYearUpperBound)
                        {
                            album.RecordingYear = null;
                        }

                        if (String.IsNullOrEmpty(album.Owner))
                        {
                            album.Owner = CatalogContext.Instance.Settings.LabelDefault;
                        }

                        var tier = CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["catalog_tier"]).ToLower();
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
                        var otherLangs = CatalogContext.Instance.Langs.Where(l => l != CatalogContext.Instance.DefaultLang).ToList();
                        foreach (var lang in otherLangs)
                        {
                            var title = CellContentWizard(cells, _worksheetColumns[AlbumWorksheetName]["title_" + lang.ShortName]);
                            album.Title[lang.ShortName] = (!String.IsNullOrEmpty(title)) ? album.Title[CatalogContext.Instance.DefaultLang.ShortName] : title;
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
        /// Tracks parser
        /// </summary>
        private async Task ParseTracks()
        {
            Notify("Parsing tracks.");
            if (_tracks == null)
            {
                return;
            }

            foreach (var row in _tracks)
            {
                Dictionary<int, object> cells = null;
                await Task.Run(() =>
                {
                    cells = CellMapByRow(row, TrackWorksheetName);
                });

                if (cells != null && cells.Count > 0)
                {
                    var albumId = Convert.ToInt32(CellContentWizard(cells, _worksheetColumns[TrackWorksheetName]["album_id"], "0"));
                    var volumeIndex = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[TrackWorksheetName]["volume_index"], "0"));
                    var trackIndex = Convert.ToInt16(CellContentWizard(cells, _worksheetColumns[TrackWorksheetName]["track_index"], "0"));

                    if (!CatalogContext.Instance.Albums.Exists(a => a.Id == albumId))
                    {
                        continue;
                    }

                    var album = CatalogContext.Instance.Albums.FirstOrDefault(a => a.Id == albumId);
                    if (album == null)
                    {
                        if (_mainFormViewModel != null)
                        {
                            _mainFormViewModel.InputProgressBarValue++;
                        }
                        continue;
                    }

                    if (album.Tracks == null)
                    {
                        album.Tracks = new Dictionary<short, Dictionary<short, String>>();
                    }

                    if (volumeIndex <= 0)
                    {
                        if (_mainFormViewModel != null)
                        {
                            _mainFormViewModel.InputProgressBarValue++;
                        }
                        continue;
                    }

                    if (!album.Tracks.ContainsKey(volumeIndex))
                    {
                        album.Tracks[volumeIndex] = new Dictionary<short, String>();
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
                    var isrcId = CellContentWizard(cells, _worksheetColumns[TrackWorksheetName][IsrcIdFieldName]);
                    if (String.IsNullOrEmpty(isrcId) || isrcId.Length < 12 || isrcId.Length > 15)
                    {
                        if (_mainFormViewModel != null)
                        {
                            _mainFormViewModel.InputProgressBarValue++;
                        }
                        continue;
                    }

                    // Conditions ok to add asset to album
                    album.Tracks[volumeIndex][trackIndex] = isrcId;
                }

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

        // Complete fields that rely on posterior data, e.g. on Assets
        private static void FinalizeAlbums()
        {
            if (CatalogContext.Instance.Albums == null)
            {
                return;
            }

            foreach (var album in CatalogContext.Instance.Albums)
            {
                // Primary Artist field automatic computation
                var artistFrequencies = new Dictionary<int, int>(); // Keys are primary artists, Values are occurrences in tracks
                if (album.PrimaryArtistId == null)
                {
                    try
                    {
                        if (album.Tracks == null)
                        {
                            continue;
                        }
                        foreach (var volume in album.Tracks)
                        {
                            foreach (var track in volume.Value)
                            {
                                var asset = CatalogContext.Instance.Assets.FirstOrDefault(e => String.Compare(e.Id, track.Value, StringComparison.Ordinal) == 0);
                                if (asset == null || String.IsNullOrEmpty(asset.Id))
                                {
                                    continue;
                                }
                                foreach (KeyValuePair<int, Dictionary<String, String>> contributor in asset.Contributors)
                                    foreach (KeyValuePair<String, String> roleQuality in contributor.Value)
                                    {
                                        // Retrieve the parent object for that key.
                                        var roleObject =
                                            CatalogContext.Instance.Roles.FirstOrDefault(r => String.Compare(
                                                r.Name, roleQuality.Key, StringComparison.Ordinal) == 0);
                                        if (roleObject == null)
                                        {
                                            continue;
                                        }
                                        // Retrieve the parent object for that key.
                                        var qualityObject =
                                            CatalogContext.Instance.Qualities.FirstOrDefault(q => String.Compare(
                                                q.Name, roleQuality.Value, StringComparison.Ordinal) == 0);
                                        if (qualityObject == null)
                                        {
                                            continue;
                                        }
                                        if  (
                                                roleObject.Reference != Role.QualifiedName.Performer
                                                || String.Compare(qualityObject.Name.ToLower(), "primary", StringComparison.Ordinal) != 0
                                            )
                                        {
                                            continue;
                                        }
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
                    catch (Exception ex)
                    {
                        Debug.WriteLine("TemplatedCatalogReader.FinalizeAlbums, exception=" + ex.Message);
                        Instance.Notify("The primary artist detection went wrong in album " + album.CatalogReference);
                    }

                    // Now select the most frequent primary performer
                    try
                    {
                        var max = artistFrequencies.Values.ToList().Max();
                        album.PrimaryArtistId = artistFrequencies.Keys.ToList().FirstOrDefault(e => artistFrequencies[e] == max);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("TemplatedCatalogReader.FinalizeAlbums, exception=" + ex.Message);
                        Instance.Notify("The primary artist setup went wrong in album " + album.CatalogReference);
                    }
                }

                // Total discs
                if (album.Tracks != null && album.Tracks.Count > 0)
                {
                    album.TotalDiscs = album.Tracks.Keys.ToList().Max();
                }

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
        private static void FinalizeAssets()
        {
            if (CatalogContext.Instance.Assets == null || CatalogContext.Instance.Albums == null)
            {
                return;
            }

            // This update may raise undesired affectations if the isrc is present in more than one album (compilations, etc.) 
            try
            {
                foreach (var asset in CatalogContext.Instance.Assets)
                {
                    if (asset.Tier != null) continue;
                    var album = CatalogContext.Instance.Albums.FirstOrDefault(
                        a => a.Tracks.Values.ToList().Exists(v => v.Values.ToList().Contains(asset.Id)));
                    if (album != null)
                    {
                        asset.Tier = album.Tier;
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TemplatedCatalogReader.FinalizeAssets, exception=" + ex.Message);
                Instance.Notify("A problem occurred in assets finalization.");
            }
        }

        public void Notify(String message)
        {
            if (_mainFormViewModel == null || _mainFormViewModel.MainFormDispatcher == null || String.IsNullOrEmpty(message))
            {
                Debug.WriteLine("TemplatedCatalogReader.Notify, wrong view model or empty message");
                return;
            }
            var methodInvoker = new MethodInvoker(() =>
            {
                _mainFormViewModel.Notification = message;
            });

            _mainFormViewModel.MainFormDispatcher.Invoke(methodInvoker);
        }
    }
}
