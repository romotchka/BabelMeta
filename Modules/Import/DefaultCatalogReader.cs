/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using BabelMeta.Model;
using BabelMeta.AppConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using BabelMeta.Model.Config;

namespace BabelMeta.Modules.Import
{
    /// <summary>
    /// This default Input format consists in a Excel 2003 XML export of worksheets:
    /// lang, role, tag, artist, work, isrc, featuring, album, asset
    /// </summary>
    public class DefaultCatalogReader : ICatalogReader
    {
        private XmlDocument _document;

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
        private Dictionary<String, XmlNode> _worksheets;

        /// <summary>
        /// Column indexes by worksheets (permits flexibility in the worksheet structure)
        /// </summary>
        private Dictionary<String, Dictionary<String, Int32>> _worksheetColumns;

        private List<XmlNode> _settings;
        private List<XmlNode> _langs;
        private List<XmlNode> _tags;
        private List<XmlNode> _roles;
        private List<XmlNode> _qualities;
        private List<XmlNode> _artists;
        private List<XmlNode> _works;
        private List<XmlNode> _isrcs;
        private List<XmlNode> _albums;
        private List<XmlNode> _assets;

        private static DefaultCatalogReader _instance;

        private DefaultCatalogReader()
        {
            _worksheets = new Dictionary<String, XmlNode>();
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

        public ReturnCodes Parse(Stream s, MainFormViewModel viewModel = null)
        {
            if (s == null)
            {
                return ReturnCodes.ModulesImportDefaultParseEmptyStream;
            }

            _document = new XmlDocument();
            _document.Load(s);

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

            CatalogContext.Instance.Initialized = true;
            _mainFormViewModel.InputProgressBarValue = _mainFormViewModel.InputProgressBarMax;
            return ReturnCodes.Ok;
        }

        private bool IsValidWorkbook()
        {
            if (_document == null || _document.DocumentElement == null)
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
            Dictionary<Int32, XmlNode> map;
            int i;

            // Strict order so as to enable referential integrity check

            map = CellMapByRow(_worksheets["SETTINGS"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
            if (!ExistsCellValueInRow("parameter", map, "SETTINGS")) return false;
            if (!ExistsCellValueInRow("value", map, "SETTINGS")) return false;
            _settings = WorksheetActiveRows("SETTINGS");

            // Langs is the next worksheet to be parsed first because some worksheet columns are lang-dependent
            ParseSettings();

            map = CellMapByRow(_worksheets["lang"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
            if (!ExistsCellValueInRow("local_db", map, "lang")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "lang")) return false;
            if (!ExistsCellValueInRow("long_name", map, "lang")) return false;
            if (!ExistsCellValueInRow("short_name", map, "lang")) return false;
            if (!ExistsCellValueInRow("default", map, "lang")) return false;
            _langs = WorksheetActiveRows("lang");

            // Langs is the next worksheet to be parsed first because some worksheet columns are lang-dependent
            ParseLangs();

            map = CellMapByRow(_worksheets["tag"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
            if (!ExistsCellValueInRow("local_db", map, "tag")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "tag")) return false;
            if (!ExistsCellValueInRow("tag_name", map, "tag")) return false;
            _tags = WorksheetActiveRows("tag");

            map = CellMapByRow(_worksheets["role"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
            if (!ExistsCellValueInRow("local_db", map, "role")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "role")) return false;
            if (!ExistsCellValueInRow("role_name", map, "role")) return false;
            _roles = WorksheetActiveRows("role");

            map = CellMapByRow(_worksheets["quality"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
            if (!ExistsCellValueInRow("local_db", map, "quality")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "quality")) return false;
            if (!ExistsCellValueInRow("quality_name", map, "quality")) return false;
            _qualities = WorksheetActiveRows("quality");

            map = CellMapByRow(_worksheets["artist"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
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

            map = CellMapByRow(_worksheets["work"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
            if (!ExistsCellValueInRow("local_db", map, "work")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "work")) return false;
            if (!ExistsCellValueInRow("id", map, "work")) return false;
            if (!ExistsCellValueInRow("id_parent", map, "work")) return false;
            if (!ExistsCellValueInRow("catalog_number", map, "work")) return false;
            if (!ExistsCellValueInRow("key", map, "work")) return false;
            if (!ExistsCellValueInRow("year", map, "work")) return false;
            i = 1;
            while (
                        ExistsCellValueInRow("id_contributor" + i.ToString(), map, "work")
                        && ExistsCellValueInRow("role_contributor" + i.ToString(), map, "work")
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

            map = CellMapByRow(_worksheets["isrc"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
            if (!ExistsCellValueInRow("local_db", map, "isrc")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "isrc")) return false;
            if (!ExistsCellValueInRow("isrc_id", map, "isrc")) return false;
            if (!ExistsCellValueInRow("work_id", map, "isrc")) return false;
            i = 1;
            while (
                        ExistsCellValueInRow("id_contributor" + i.ToString(), map, "isrc")
                        && ExistsCellValueInRow("role_contributor" + i.ToString(), map, "isrc")
                        && ExistsCellValueInRow("quality_contributor" + i.ToString(), map, "isrc")
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

            map = CellMapByRow(_worksheets["album"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
            if (!ExistsCellValueInRow("local_db", map, "album")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "album")) return false;
            if (!ExistsCellValueInRow("album_id", map, "album")) return false;
            if (!ExistsCellValueInRow("c_name", map, "album")) return false;
            if (!ExistsCellValueInRow("c_year", map, "album")) return false;
            if (!ExistsCellValueInRow("p_name", map, "album")) return false;
            if (!ExistsCellValueInRow("p_year", map, "album")) return false;
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

            map = CellMapByRow(_worksheets["asset"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
            if (!ExistsCellValueInRow("local_db", map, "asset")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "asset")) return false;
            if (!ExistsCellValueInRow("album_id", map, "asset")) return false;
            if (!ExistsCellValueInRow("volume_index", map, "asset")) return false;
            if (!ExistsCellValueInRow("track_index", map, "asset")) return false;
            if (!ExistsCellValueInRow("isrc_id", map, "asset")) return false;
            _assets = WorksheetActiveRows("asset");

            return true;
        }

        private Dictionary<Int32, XmlNode> CellMapByRow(XmlNode row)
        {
            if (row == null)
            {
                return null;
            }

            Dictionary<Int32, XmlNode> map = new Dictionary<Int32, XmlNode>();
            Int32 index = 1;
            foreach (XmlNode cell in row.ChildNodes.Cast<XmlNode>().Where(n => n.Name.CompareTo(OfficeXml.WorksheetCell) == 0))
            {
                // Cell index has priority over deduced index
                if (cell.Attributes[OfficeXml.CellIndex] != null)
                {
                    index = Convert.ToInt32(cell.Attributes[OfficeXml.CellIndex].InnerText);
                }
                map[index] = cell;
                index++;
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
        private List<XmlNode> WorksheetActiveRows(String worksheetName, bool localActive = false, bool partnerActive = false)
        {
            if (String.IsNullOrEmpty(worksheetName))
            {
                return null;
            }

            if (_worksheets[worksheetName] == null)
            {
                return null;
            }

            List<XmlNode> rows = _worksheets[worksheetName].ChildNodes.Cast<XmlNode>()
                .Where(r => r.Name.CompareTo(OfficeXml.WorksheetRow) == 0)
                .ToList();
            List<XmlNode> filteredRows = new List<XmlNode>();

            foreach (XmlNode row in rows)
            {
                Dictionary<Int32, XmlNode> map = CellMapByRow(row);
                // Filtering active rows only (except for SETTINGS)
                if (worksheetName.CompareTo("SETTINGS") == 0)
                {
                    filteredRows.Add(row);
                }
                else
                {
                    bool local = map.ContainsKey(_worksheetColumns[worksheetName]["local_db"]) && map[_worksheetColumns[worksheetName]["local_db"]].InnerText.CompareTo("active") == 0;
                    bool partner = map.ContainsKey(_worksheetColumns[worksheetName]["partner_db"]) && map[_worksheetColumns[worksheetName]["partner_db"]].InnerText.CompareTo("active") == 0;
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
            }
            return filteredRows;
        }

        /// <summary>
        /// Tests occurrence of a cell value in a given row.
        /// </summary>
        /// <param name="cellValue"></param>
        /// <param name="map"></param>
        /// <param name="worksheetName">If provided, the value is considered a header name and is recorded along with column index</param>
        /// <returns></returns>
        private bool ExistsCellValueInRow(String cellValue, Dictionary<Int32, XmlNode> map, String worksheetName = "")
        {
            if (String.IsNullOrEmpty(cellValue) || map == null || map.Count == 0)
            {
                return false;
            }

            KeyValuePair<Int32, XmlNode>? element = map.FirstOrDefault(e => e.Value.InnerText.Trim().CompareTo(cellValue) == 0);
            bool exists = (element != null) && ((KeyValuePair<Int32, XmlNode>)element).Value != null;

            if (exists && !String.IsNullOrEmpty(worksheetName) && _worksheetColumns[worksheetName] != null)
            {
                Int32 index = ((KeyValuePair<Int32, XmlNode>)element).Key;
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
            if (String.IsNullOrEmpty(worksheetName))
            {
                return false;
            }

            if (_worksheets.ContainsKey(worksheetName))
            {
                return true;
            }

            XmlNode worksheet = _document.DocumentElement.ChildNodes.Cast<XmlNode>()
                .FirstOrDefault(
                    n =>
                    n.Name == OfficeXml.Worksheet
                    && n.Attributes[OfficeXml.WorksheetName] != null
                    && n.Attributes[OfficeXml.WorksheetName].InnerText.CompareTo(worksheetName) == 0
                );

            if (worksheet == null)
            {
                return false;
            }

            XmlNode table = worksheet.ChildNodes.Cast<XmlNode>()
                .FirstOrDefault(n => n.Name == OfficeXml.WorksheetTable);

            bool exists = table != null;

            if (exists)
            {
                // Add table element of the worksheet
                _worksheets[worksheetName] = table;
                _worksheetColumns[worksheetName] = new Dictionary<String, Int32>();
            }

            return exists;
        }

        private void ParseSettings()
        {
            if (_settings == null)
            {
                return;
            }
            foreach (XmlNode row in _settings)
            {
                Dictionary<Int32, XmlNode> cells = CellMapByRow(row);
                if (cells != null && cells.Count > 0)
                {
                    String parameter = String.Empty;
                    String value = String.Empty;
                    List<Int32> keys = cells.Keys.ToList();

                    if (keys.Contains(_worksheetColumns["SETTINGS"]["parameter"]))
                    {
                        parameter = cells.FirstOrDefault(c => c.Key == _worksheetColumns["SETTINGS"]["parameter"]).Value.InnerText.Trim();
                    }

                    if (keys.Contains(_worksheetColumns["SETTINGS"]["value"]))
                    {
                        value = cells.FirstOrDefault(c => c.Key == _worksheetColumns["SETTINGS"]["value"]).Value.InnerText.Trim();
                    }

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
                                        case "premium": settings.CatalogTierDefault = CatalogTier.Premium ; break;
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
        /// Since other worksheet rows count is unknown at this stage, do not update progress bar inside the function
        /// </summary>
        private void ParseLangs()
        {
            if (_langs == null)
            {
                return;
            }

            foreach (XmlNode row in _langs)
            {
                Dictionary<Int32, XmlNode> cells = CellMapByRow(row);
                if (cells != null && cells.Count > 0)
                {
                    Lang lang = new Lang();
                    List<Int32> keys = cells.Keys.ToList();

                    if (keys.Contains(_worksheetColumns["lang"]["long_name"]))
                    {
                        lang.LongName = cells.FirstOrDefault(c => c.Key == _worksheetColumns["lang"]["long_name"]).Value.InnerText.Trim().ToLower();
                    }

                    if (keys.Contains(_worksheetColumns["lang"]["short_name"]))
                    {
                        lang.ShortName = cells.FirstOrDefault(c => c.Key == _worksheetColumns["lang"]["short_name"]).Value.InnerText.Trim().ToLower();
                    }

                    if (keys.Contains(_worksheetColumns["lang"]["default"]))
                    {
                        lang.IsDefault = cells.FirstOrDefault(c => c.Key == _worksheetColumns["lang"]["default"]).Value.InnerText.Trim().ToLower().CompareTo("yes") == 0;
                    }

                    // Short name is mandatory
                    if (!String.IsNullOrEmpty(lang.ShortName))
                    {
                        CatalogContext.Instance.Langs.Add(lang);
                    }
                }
            }
        }

        private void ParseTags()
        {
            if (_tags == null)
            {
                return;
            }

            foreach (XmlNode row in _tags)
            {
                Dictionary<Int32, XmlNode> cells = CellMapByRow(row);
                if (cells != null && cells.Count > 0)
                {
                    Tag tag = new Tag();
                    List<Int32> keys = cells.Keys.ToList();

                    if (keys.Contains(_worksheetColumns["tag"]["tag_name"]))
                    {
                        tag.Name = cells.FirstOrDefault(c => c.Key == _worksheetColumns["tag"]["tag_name"]).Value.InnerText.Trim();
                    }

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

        private void ParseRoles()
        {
            if (_roles == null)
            {
                return;
            }

            foreach (XmlNode row in _roles)
            {
                Dictionary<Int32, XmlNode> cells = CellMapByRow(row);
                if (cells != null && cells.Count > 0)
                {
                    Role role = new Role();
                    List<Int32> keys = cells.Keys.ToList();

                    if (keys.Contains(_worksheetColumns["role"]["role_name"]))
                    {
                        role.Name = cells.FirstOrDefault(c => c.Key == _worksheetColumns["role"]["role_name"]).Value.InnerText.Trim();

                        // Attempt to retrieve a qualified name (standardized)
                        switch (role.Name.ToLower())
                        {
                            case "arranger": role.Reference = Role.QualifiedName.Arranger; break;
                            case "composer": role.Reference = Role.QualifiedName.Composer; break;
                            case "conductor": role.Reference = Role.QualifiedName.Conductor; break;
                            case "engineer": role.Reference = Role.QualifiedName.Engineer; break;
                            case "ensemble": role.Reference = Role.QualifiedName.Ensemble; break;
                            case "performer": role.Reference = Role.QualifiedName.Performer; break;
                            // TODO
                            default: role.Reference = Role.QualifiedName.ContributingArtist; break;
                        }
                    }

                    if (!String.IsNullOrEmpty(role.Name))
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

            foreach (XmlNode row in _qualities)
            {
                Dictionary<Int32, XmlNode> cells = CellMapByRow(row);
                if (cells != null && cells.Count > 0)
                {
                    Quality quality = new Quality();
                    List<Int32> keys = cells.Keys.ToList();

                    if (keys.Contains(_worksheetColumns["quality"]["quality_name"]))
                    {
                        quality.Name = cells.FirstOrDefault(c => c.Key == _worksheetColumns["quality"]["quality_name"]).Value.InnerText.Trim();
                    }

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

            foreach (XmlNode row in _artists)
            {
                Dictionary<Int32, XmlNode> cells = CellMapByRow(row);
                if (cells != null && cells.Count > 0)
                {
                    Artist artist = new Artist();
                    List<Int32> keys = cells.Keys.ToList();

                    // Id is mandatory and > 0
                    if (keys.Contains(_worksheetColumns["artist"]["id"]))
                    {
                        artist.Id = Convert.ToInt32(cells.FirstOrDefault(c => c.Key == _worksheetColumns["artist"]["id"]).Value.InnerText.Trim());
                        if (artist == null || artist.Id <= 0)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    if (keys.Contains(_worksheetColumns["artist"]["birth"]))
                    {
                        artist.Birth = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["artist"]["birth"]).Value.InnerText.Trim());
                        if (artist.Birth != null && artist.Birth < 0) { artist.Birth = null; }
                    }

                    if (keys.Contains(_worksheetColumns["artist"]["death"]))
                    {
                        artist.Death = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["artist"]["death"]).Value.InnerText.Trim());
                        if (artist.Death != null && artist.Death < 0) { artist.Death = null; }
                    }

                    artist.LastName = new Dictionary<Lang, String>();
                    artist.FirstName = new Dictionary<Lang, String>();
                    // Object may have several lang-dependent field sets
                    foreach (Lang lang in CatalogContext.Instance.Langs)
                    {
                        String last = String.Empty;
                        String first = String.Empty;

                        if (keys.Contains(_worksheetColumns["artist"]["lastname_" + lang.ShortName]))
                        {
                            last = cells.FirstOrDefault(c => c.Key == _worksheetColumns["artist"]["lastname_" + lang.ShortName]).Value.InnerText.Trim();
                        }

                        if (keys.Contains(_worksheetColumns["artist"]["firstname_" + lang.ShortName]))
                        {
                            first = cells.FirstOrDefault(c => c.Key == _worksheetColumns["artist"]["firstname_" + lang.ShortName]).Value.InnerText.Trim();
                        }

                        // If, at least, last name is not empty, record both fields
                        if (!String.IsNullOrEmpty(last))
                        {
                            artist.LastName[lang] = last;
                            artist.FirstName[lang] = first;
                        }
                    }

                    // If, at least, one language set is available (default or not), save the entry 
                    if (artist.LastName.Count > 0)
                    {
                        CatalogContext.Instance.Artists.Add(artist);
                    }
                }

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

        private void ParseWorks()
        {
            if (_works == null)
            {
                return;
            }

            foreach (XmlNode row in _works)
            {
                Dictionary<Int32, XmlNode> cells = CellMapByRow(row);
                if (cells != null && cells.Count > 0)
                {
                    Work work = new Work();
                    List<Int32> keys = cells.Keys.ToList();

                    // Id is mandatory and > 0
                    if (keys.Contains(_worksheetColumns["work"]["id"]))
                    {
                        work.Id = Convert.ToInt32(cells.FirstOrDefault(c => c.Key == _worksheetColumns["work"]["id"]).Value.InnerText.Trim());
                        if (work == null || work.Id <= 0)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    // Parent work id
                    if (keys.Contains(_worksheetColumns["work"]["id_parent"]))
                    {
                        Int32 parent = Convert.ToInt32(cells.FirstOrDefault(c => c.Key == _worksheetColumns["work"]["id_parent"]).Value.InnerText.Trim());
                        if (parent > 0)
                        {
                            work.Parent = parent;
                        }
                    }

                    if (keys.Contains(_worksheetColumns["work"]["catalog_number"]))
                    {
                        work.ClassicalCatalog = cells.FirstOrDefault(c => c.Key == _worksheetColumns["work"]["catalog_number"]).Value.InnerText.Trim();
                    }

                    if (keys.Contains(_worksheetColumns["work"]["key"]))
                    {
                        String shortKey = cells.FirstOrDefault(c => c.Key == _worksheetColumns["work"]["key"]).Value.InnerText.Trim();
                        if (_shortenedKeys.ContainsKey(shortKey))
                        {
                            work.Tonality = _shortenedKeys[shortKey];
                        }
                    }

                    if (keys.Contains(_worksheetColumns["work"]["year"]))
                    {
                        work.Year = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["work"]["year"]).Value.InnerText.Trim());
                        if (work.Year != null && work.Year < 0) { work.Year = null; }
                    }

                    work.Contributors = new Dictionary<Int32, Role>();
                    int i = 1;
                    while (
                                (_worksheetColumns["work"].ContainsKey("id_contributor" + i.ToString()))
                                && (_worksheetColumns["work"].ContainsKey("role_contributor" + i.ToString()))
                            )
                    {
                        Int32 idContributor = 0;
                        Role roleContributor = null;

                        if (keys.Contains(_worksheetColumns["work"]["id_contributor" + i.ToString()]))
                        {
                            idContributor = Convert.ToInt32(cells.FirstOrDefault(c => c.Key == _worksheetColumns["work"]["id_contributor" + i.ToString()]).Value.InnerText.Trim());
                        }

                        if (keys.Contains(_worksheetColumns["work"]["role_contributor" + i.ToString()]))
                        {
                            String roleContributorName = cells.FirstOrDefault(c => c.Key == _worksheetColumns["work"]["role_contributor" + i.ToString()]).Value.InnerText.Trim();
                            if (!String.IsNullOrEmpty(roleContributorName))
                            {
                                roleContributor = CatalogContext.Instance.Roles.FirstOrDefault(
                                    c => c.Name.CompareTo(roleContributorName) == 0
                                );
                            }
                        }

                        // Contributor valid
                        if (idContributor > 0 && roleContributor != null)
                        {
                            work.Contributors[idContributor] = roleContributor;
                        }

                        i++;
                    }

                    if (keys.Contains(_worksheetColumns["work"]["movement_number"]))
                    {
                        work.MovementNumber = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["work"]["movement_number"]).Value.InnerText.Trim());
                        if (work.MovementNumber != null && work.MovementNumber <= 0) { work.MovementNumber = null; }
                    }

                    work.Title = new Dictionary<Lang, String>();
                    // Object may have several lang-dependent field sets
                    foreach (Lang lang in CatalogContext.Instance.Langs)
                    {
                        String title = String.Empty;
                        if (keys.Contains(_worksheetColumns["work"]["title_" + lang.ShortName]))
                        {
                            title = cells.FirstOrDefault(c => c.Key == _worksheetColumns["work"]["title_" + lang.ShortName]).Value.InnerText.Trim();
                        }

                        if (!String.IsNullOrEmpty(title))
                        {
                            work.Title[lang] = title;
                        }
                    }

                    work.MovementTitle = new Dictionary<Lang, String>();
                    // Object may have several lang-dependent field sets
                    foreach (Lang lang in CatalogContext.Instance.Langs)
                    {
                        String movementTitle = String.Empty;
                        if (keys.Contains(_worksheetColumns["work"]["movement_title_" + lang.ShortName]))
                        {
                            movementTitle = cells.FirstOrDefault(c => c.Key == _worksheetColumns["work"]["movement_title_" + lang.ShortName]).Value.InnerText.Trim();
                        }

                        if (!String.IsNullOrEmpty(movementTitle))
                        {
                            work.MovementTitle[lang] = movementTitle;
                        }
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

        private void ParseIsrcs()
        {
            if (_isrcs == null)
            {
                return;
            }

            foreach (XmlNode row in _isrcs)
            {
                Dictionary<Int32, XmlNode> cells = CellMapByRow(row);
                if (cells != null && cells.Count > 0)
                {
                    Isrc isrc = new Isrc();
                    List<Int32> keys = cells.Keys.ToList();

                    // Id is mandatory and has a minimal standardized length
                    if (keys.Contains(_worksheetColumns["isrc"]["isrc_id"]))
                    {
                        isrc.Id = cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["isrc_id"]).Value.InnerText.Trim();
                        if (isrc == null || isrc.Id.Length < 12 || isrc.Id.Length > 15)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    // Work id, mandatory
                    if (keys.Contains(_worksheetColumns["isrc"]["work_id"]))
                    {
                        Int32 workId = Convert.ToInt32(cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["work_id"]).Value.InnerText.Trim());
                        if (workId > 0)
                        {
                            isrc.Work = workId;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    isrc.Contributors = new Dictionary<Int32, Dictionary<Role, Quality>>();
                    int i = 1;
                    while (
                                (_worksheetColumns["isrc"].ContainsKey("id_contributor" + i.ToString()))
                                && (_worksheetColumns["isrc"].ContainsKey("role_contributor" + i.ToString()))
                                && (_worksheetColumns["isrc"].ContainsKey("quality_contributor" + i.ToString()))
                            )
                    {
                        Int32 idContributor = 0;
                        Role roleContributor = null;
                        Quality qualityContributor = null;

                        if (keys.Contains(_worksheetColumns["isrc"]["id_contributor" + i.ToString()]))
                        {
                            idContributor = Convert.ToInt32(cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["id_contributor" + i.ToString()]).Value.InnerText.Trim());
                        }

                        if (keys.Contains(_worksheetColumns["isrc"]["role_contributor" + i.ToString()]))
                        {
                            String roleContributorName = cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["role_contributor" + i.ToString()]).Value.InnerText.Trim();
                            if (!String.IsNullOrEmpty(roleContributorName))
                            {
                                roleContributor = CatalogContext.Instance.Roles.FirstOrDefault(
                                    c => c.Name.CompareTo(roleContributorName) == 0
                                );
                            }
                        }

                        if (keys.Contains(_worksheetColumns["isrc"]["quality_contributor" + i.ToString()]))
                        {
                            String qualityContributorName = cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["quality_contributor" + i.ToString()]).Value.InnerText.Trim();
                            if (!String.IsNullOrEmpty(qualityContributorName))
                            {
                                qualityContributor = CatalogContext.Instance.Qualities.FirstOrDefault(
                                    c => c.Name.CompareTo(qualityContributorName) == 0
                                );
                            }
                        }

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

                    if (keys.Contains(_worksheetColumns["isrc"]["c_name"]))
                    {
                        isrc.CName = cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["c_name"]).Value.InnerText.Trim();
                        if (String.IsNullOrEmpty(isrc.CName))
                        {
                            isrc.CName = CatalogContext.Instance.Settings.LabelDefault;
                        }
                    }
                    else
                    {
                        isrc.CName = CatalogContext.Instance.Settings.LabelDefault;
                    }

                    if (keys.Contains(_worksheetColumns["isrc"]["c_year"]))
                    {
                        isrc.CYear = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["c_year"]).Value.InnerText.Trim());
                    }
                    if (isrc.CYear < 1900 || isrc.CYear > 2100)
                    {
                        isrc.CYear = null;
                    }

                    if (keys.Contains(_worksheetColumns["isrc"]["p_name"]))
                    {
                        isrc.PName = cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["p_name"]).Value.InnerText.Trim();
                        if (String.IsNullOrEmpty(isrc.PName))
                        {
                            isrc.PName = CatalogContext.Instance.Settings.LabelDefault;
                        }
                    }
                    else
                    {
                        isrc.PName = CatalogContext.Instance.Settings.LabelDefault;
                    }

                    if (keys.Contains(_worksheetColumns["isrc"]["p_year"]))
                    {
                        isrc.PYear = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["p_year"]).Value.InnerText.Trim());
                    }
                    if (isrc.PYear < 1900 || isrc.PYear > 2100)
                    {
                        isrc.PYear = null;
                    }


                    if (keys.Contains(_worksheetColumns["isrc"]["recording_location"]))
                    {
                        isrc.RecordingLocation = cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["recording_location"]).Value.InnerText.Trim();
                    }

                    if (keys.Contains(_worksheetColumns["isrc"]["recording_year"]))
                    {
                        isrc.RecordingYear = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["recording_year"]).Value.InnerText.Trim());
                    }
                    if (isrc.RecordingYear < 1900 || isrc.RecordingYear > 2100)
                    {
                        isrc.RecordingYear = null;
                    }

                    if (keys.Contains(_worksheetColumns["isrc"]["available_separately"]))
                    {
                        String value = cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["available_separately"]).Value.InnerText.Trim();
                        isrc.AvailableSeparately = (!String.IsNullOrEmpty(value) && value.Trim().ToLower().CompareTo("no") == 0)
                            ? false
                            : CatalogContext.Instance.Settings.AvailableSeparatelyDefault;
                    }
                    else
                    {
                        isrc.AvailableSeparately = CatalogContext.Instance.Settings.AvailableSeparatelyDefault;
                    }

                    // Catalog tier default is album-wise catalog tier, done at a later stage, after Album parsing
                    if (keys.Contains(_worksheetColumns["isrc"]["catalog_tier"]))
                    {
                        String tier = cells.FirstOrDefault(c => c.Key == _worksheetColumns["isrc"]["catalog_tier"]).Value.InnerText.ToLower().Trim();
                        if (!String.IsNullOrEmpty(tier))
                        {
                            switch (tier)
                            {
                                case "back": isrc.Tier = CatalogTier.Back; break;
                                case "budget": isrc.Tier = CatalogTier.Budget; break;
                                case "front": isrc.Tier = CatalogTier.Front; break;
                                case "mid": isrc.Tier = CatalogTier.Mid; break;
                                case "premium": isrc.Tier = CatalogTier.Premium; break;
                            }
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

        private void ParseAlbums()
        {
            if (_albums == null)
            {
                return;
            }

            foreach (XmlNode row in _albums)
            {
                Dictionary<Int32, XmlNode> cells = CellMapByRow(row);
                if (cells != null && cells.Count > 0)
                {
                    Album album = new Album();
                    List<Int32> keys = cells.Keys.ToList();

                    // Id is mandatory and > 0
                    if (keys.Contains(_worksheetColumns["album"]["album_id"]))
                    {
                        album.Id = Convert.ToInt32(cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["album_id"]).Value.InnerText.Trim());
                        if (album == null || album.Id <= 0)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    if (keys.Contains(_worksheetColumns["album"]["c_name"]))
                    {
                        album.CName = cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["c_name"]).Value.InnerText.Trim();
                        if (String.IsNullOrEmpty(album.CName))
                        {
                            album.CName = CatalogContext.Instance.Settings.LabelDefault;
                        }
                    }
                    else
                    {
                        album.CName = CatalogContext.Instance.Settings.LabelDefault;
                    }

                    if (keys.Contains(_worksheetColumns["album"]["c_year"]))
                    {
                        album.CYear = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["c_year"]).Value.InnerText.Trim());
                    }
                    if (album.CYear < 1900 || album.CYear > 2100)
                    {
                        album.CYear = null;
                    }

                    if (keys.Contains(_worksheetColumns["album"]["p_name"]))
                    {
                        album.PName = cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["p_name"]).Value.InnerText.Trim();
                        if (String.IsNullOrEmpty(album.PName))
                        {
                            album.PName = CatalogContext.Instance.Settings.LabelDefault;
                        }
                    }
                    else
                    {
                        album.PName = CatalogContext.Instance.Settings.LabelDefault;
                    }

                    if (keys.Contains(_worksheetColumns["album"]["p_year"]))
                    {
                        album.PYear = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["p_year"]).Value.InnerText.Trim());
                    }
                    if (album.PYear < 1900 || album.PYear > 2100)
                    {
                        album.PYear = null;
                    }

                    if (keys.Contains(_worksheetColumns["album"]["catalog_tier"]))
                    {
                        String tier = cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["catalog_tier"]).Value.InnerText.ToLower().Trim();
                        if (!String.IsNullOrEmpty(tier))
                        {
                            switch (tier)
                            {
                                case "back": album.Tier = CatalogTier.Back; break;
                                case "budget": album.Tier = CatalogTier.Budget; break;
                                case "front": album.Tier = CatalogTier.Front; break;
                                case "mid": album.Tier = CatalogTier.Mid; break;
                                case "premium": album.Tier = CatalogTier.Premium; break;
                                default: album.Tier = CatalogContext.Instance.Settings.CatalogTierDefault; break;
                            }
                        }
                        else
                        {
                            album.Tier = CatalogContext.Instance.Settings.CatalogTierDefault;
                        }
                    }
                    else
                    {
                        album.Tier = CatalogContext.Instance.Settings.CatalogTierDefault;
                    }

                    if (keys.Contains(_worksheetColumns["album"]["consumer_release_date"]))
                    {
                        String dateString = cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["consumer_release_date"]).Value.InnerText.Trim();
                    }
                    else
                    {
                        // TODO add auto date
                    }

                    if (keys.Contains(_worksheetColumns["album"]["label"]))
                    {
                        String label = cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["label"]).Value.InnerText.Trim();
                        if (!String.IsNullOrEmpty(label))
                        {
                            album.Owner = label;
                        }
                        else
                        {
                            album.Owner = CatalogContext.Instance.Settings.LabelDefault;
                        }
                    }
                    else
                    {
                        album.Owner = CatalogContext.Instance.Settings.LabelDefault;
                    }

                    if (keys.Contains(_worksheetColumns["album"]["reference"]))
                    {
                        String reference = cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["reference"]).Value.InnerText.Trim();
                        if (!String.IsNullOrEmpty(reference))
                        {
                            album.CatalogReference = reference;
                        }
                    }

                    if (keys.Contains(_worksheetColumns["album"]["ean"]))
                    {
                        album.Ean = Convert.ToInt64(cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["ean"]).Value.InnerText.Trim());
                        if (album.Ean != null && album.Ean < 0) { album.Ean = null; }
                    }

                    if (keys.Contains(_worksheetColumns["album"]["genre"]))
                    {
                        String tagName = cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["genre"]).Value.InnerText.Trim();
                        if (!String.IsNullOrEmpty(tagName) && CatalogContext.Instance.Tags.Exists(t => t.Name.CompareTo(tagName) == 0))
                        {
                            album.Genre = CatalogContext.Instance.Tags.FirstOrDefault(t => t.Name.CompareTo(tagName) == 0);
                        }
                        else
                        {
                            // TODO (add dictionary etc.)
                        }
                    }

                    if (keys.Contains(_worksheetColumns["album"]["subgenre"]))
                    {
                        String tagName = cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["subgenre"]).Value.InnerText.Trim();
                        if (!String.IsNullOrEmpty(tagName) && CatalogContext.Instance.Tags.Exists(t => t.Name.CompareTo(tagName) == 0))
                        {
                            album.Subgenre = CatalogContext.Instance.Tags.FirstOrDefault(t => t.Name.CompareTo(tagName) == 0);
                        }
                    }

                    album.Title = new Dictionary<Lang, String>();
                    // Object may have several lang-dependent field sets
                    foreach (Lang lang in CatalogContext.Instance.Langs)
                    {
                        String title = String.Empty;

                        if (keys.Contains(_worksheetColumns["album"]["title_" + lang.ShortName]))
                        {
                            title = cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["title_" + lang.ShortName]).Value.InnerText.Trim();
                        }

                        // If, at least, last name is not empty, record both fields
                        if (!String.IsNullOrEmpty(title))
                        {
                            album.Title[lang] = title;
                        }
                    }

                    // Primary artist, if empty, will be updated once ParseIsrc is completed
                    if (keys.Contains(_worksheetColumns["album"]["primary_artist"]))
                    {
                        album.PrimaryArtistId = Convert.ToInt32(cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["primary_artist"]).Value.InnerText.Trim());
                        if (album == null || album.Id <= 0)
                        {
                            album.PrimaryArtistId = null;
                        }
                    }

                    if (keys.Contains(_worksheetColumns["album"]["recording_location"]))
                    {
                        album.RecordingLocation = cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["recording_location"]).Value.InnerText.Trim();
                    }

                    if (keys.Contains(_worksheetColumns["album"]["recording_year"]))
                    {
                        album.RecordingYear = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["album"]["recording_year"]).Value.InnerText.Trim());
                    }
                    if (album.RecordingYear < 1900 || album.RecordingYear > 2100)
                    {
                        album.RecordingYear = null;
                    }

                    // Hard-coded value
                    album.Redeliver = false;


                    // If, at least, one language set is available (default or not), save the entry 
                    if (album.Title.Count > 0)
                    {
                        CatalogContext.Instance.Albums.Add(album);
                    }
                }

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

        private void ParseAssets()
        {
            if (_assets == null)
            {
                return;
            }

            foreach (XmlNode row in _assets)
            {
                Dictionary<Int32, XmlNode> cells = CellMapByRow(row);
                if (cells != null && cells.Count > 0)
                {
                    Int32 albumId = 0;
                    Int16 volumeIndex = 0;
                    Int16 trackIndex = 0;
                    String isrcId = String.Empty;
                    Album album = null;
                    List<Int32> keys = cells.Keys.ToList();

                    // Id is mandatory and referential integrity is checked
                    if (keys.Contains(_worksheetColumns["asset"]["album_id"]))
                    {
                        albumId = Convert.ToInt32(cells.FirstOrDefault(c => c.Key == _worksheetColumns["asset"]["album_id"]).Value.InnerText.Trim());
                        if (!CatalogContext.Instance.Albums.Exists(a => a.Id == albumId))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    album = CatalogContext.Instance.Albums.FirstOrDefault(a => a.Id == albumId);
                    if (album == null)
                    {
                        continue;
                    }
                    if (album.Assets == null)
                    {
                        album.Assets = new Dictionary<Int16, Dictionary<Int16, String>>();
                    }

                    if (keys.Contains(_worksheetColumns["asset"]["volume_index"]))
                    {
                        volumeIndex = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["asset"]["volume_index"]).Value.InnerText.Trim());
                        if (volumeIndex <= 0)
                        {
                            continue;
                        }

                        if (!album.Assets.ContainsKey(volumeIndex))
                        {
                            album.Assets[volumeIndex] = new Dictionary<Int16, String>();
                        }
                    }

                    if (keys.Contains(_worksheetColumns["asset"]["track_index"]))
                    {
                        trackIndex = Convert.ToInt16(cells.FirstOrDefault(c => c.Key == _worksheetColumns["asset"]["track_index"]).Value.InnerText.Trim());
                        if (trackIndex <= 0 || trackIndex > 99)
                        {
                            continue;
                        }
                    }

                    // Id is mandatory and has a minimal standardized length
                    if (keys.Contains(_worksheetColumns["asset"]["isrc_id"]))
                    {
                        isrcId = cells.FirstOrDefault(c => c.Key == _worksheetColumns["asset"]["isrc_id"]).Value.InnerText.Trim();
                        if (String.IsNullOrEmpty(isrcId) || isrcId.Length < 12 || isrcId.Length > 15)
                        {
                            continue;
                        }
                    }
                    else
                    {
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
