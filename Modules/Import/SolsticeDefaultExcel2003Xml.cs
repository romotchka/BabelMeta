﻿/*
 * Classical Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using MetadataConverter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MetadataConverter.Modules.Import
{
    /// <summary>
    /// This default Input format consists in a Excel 2003 XML export of worksheets:
    /// lang, role, tag, artist, work, isrc, featuring, album, asset
    /// </summary>
    public class SolsticeDefaultExcel2003Xml : ICatalogReader
    {
        private XmlDocument _document;

        private MainFormViewModel _mainFormViewModel;

        private const int _initPayload = 7; // Conventional value reflecting the init payload - feel free to put any reasonable value

        /// <summary>
        /// Worksheet nodes
        /// </summary>
        private Dictionary<String, XmlNode> _worksheets;

        /// <summary>
        /// Column indexes by worksheets (permits flexibility in the worksheet structure)
        /// </summary>
        private Dictionary<String,Dictionary<String,Int32>> _worksheetColumns;

        private List<XmlNode> _langs;
        private List<XmlNode> _tags;
        private List<XmlNode> _roles;
        private List<XmlNode> _artists;
        private List<XmlNode> _works;
        private List<XmlNode> _isrcs;
        private List<XmlNode> _albums;
        private List<XmlNode> _assets;

        private static SolsticeDefaultExcel2003Xml _instance;

        private SolsticeDefaultExcel2003Xml() 
        {
            _worksheets = new Dictionary<String,XmlNode>();
            _worksheetColumns = new Dictionary<String, Dictionary<String, Int32>>();
        }

        public static SolsticeDefaultExcel2003Xml Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new SolsticeDefaultExcel2003Xml();

                }
                return _instance;
            }
        }

        public void Parse(Stream s, MainFormViewModel viewModel = null)
        {
            if (s == null)
            {
                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarMax = 100;
                    _mainFormViewModel.InputProgressBarValue = 100;
                }
                return;
            }
            _document = new XmlDocument();
            _document.Load(s);

            if (viewModel != null)
            {
                _mainFormViewModel = viewModel;
                _mainFormViewModel.InputProgressBarValue = 0;
                _mainFormViewModel.InputProgressBarMax = _initPayload; 
            }

            if (!IsValidWorkbook())
            {
                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue = _mainFormViewModel.InputProgressBarMax;
                }
                return;
            }

            if (_mainFormViewModel != null)
            {
                if (_langs != null) _mainFormViewModel.InputProgressBarMax += _langs.Count;
                if (_tags != null) _mainFormViewModel.InputProgressBarMax += _tags.Count;
                if (_roles != null) _mainFormViewModel.InputProgressBarMax += _roles.Count;
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
            ParseArtists();
            ParseWorks();
            ParseIsrcs();
            ParseAlbums();
            ParseAssets();
        }

        private bool IsValidWorkbook()
        {
            if (_document == null || _document.DocumentElement == null)
            {
                return false;
            }

            // Check worksheets
            if (!ExistsWorksheet("lang")) return false;
            if (!ExistsWorksheet("tag")) return false;
            if (!ExistsWorksheet("role")) return false;
            if (!ExistsWorksheet("artist")) return false;
            if (!ExistsWorksheet("work")) return false;
            if (!ExistsWorksheet("isrc")) return false;
            if (!ExistsWorksheet("album")) return false;
            if (!ExistsWorksheet("asset")) return false;

            // Check columns and identify their indexes
            Dictionary<Int32, XmlNode> map;
            int i;

            // Strict order so as to enable referential integrity check

            map = CellMapByRow(_worksheets["lang"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
            if (!ExistsCellValueInRow("local_db", map, "lang")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "lang")) return false;
            if (!ExistsCellValueInRow("long_name", map, "lang")) return false;
            if (!ExistsCellValueInRow("short_name", map, "lang")) return false;
            if (!ExistsCellValueInRow("default", map, "lang")) return false;
            _langs = WorksheetActiveRows("lang");

            // Langs is the worksheet to be parsed first because some worksheet columns are lang-dependent
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
            while   (
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
            foreach (Lang lang in CatalogContext.Instance.Langs)
            {
                if (!ExistsCellValueInRow("title_" + lang.ShortName, map, "work")) return false;
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
            _isrcs = WorksheetActiveRows("isrc");

            map = CellMapByRow(_worksheets["album"].ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.CompareTo(OfficeXml.WorksheetRow) == 0));
            if (!ExistsCellValueInRow("local_db", map, "album")) return false;
            if (!ExistsCellValueInRow("partner_db", map, "album")) return false;
            if (!ExistsCellValueInRow("album_id", map, "album")) return false;
            if (!ExistsCellValueInRow("label", map, "album")) return false;
            if (!ExistsCellValueInRow("reference", map, "album")) return false;
            if (!ExistsCellValueInRow("ean", map, "album")) return false;
            if (!ExistsCellValueInRow("genre", map, "album")) return false;
            if (!ExistsCellValueInRow("subgenre", map, "album")) return false;
            foreach (Lang lang in CatalogContext.Instance.Langs)
            {
                if (!ExistsCellValueInRow("title_" + lang.ShortName, map, "album")) return false;
            }
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

        private Dictionary<Int32,XmlNode> CellMapByRow(XmlNode row)
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
                map.Add(index, cell);
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
                // Filtering active rows only
                bool local = map.ContainsKey(_worksheetColumns[worksheetName]["local_db"]) && map[_worksheetColumns[worksheetName]["local_db"]].InnerText.CompareTo("active") == 0;
                bool partner = map.ContainsKey(_worksheetColumns[worksheetName]["partner_db"]) && map[_worksheetColumns[worksheetName]["partner_db"]].InnerText.CompareTo("active") == 0;
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
                // Erase previous value, if any
                _worksheetColumns[worksheetName].Remove(cellValue);
                _worksheetColumns[worksheetName].Add(cellValue, index);
                
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

            if  (exists)
            {
                // Add table element of the worksheet
                _worksheets.Add(worksheetName, table);
                _worksheetColumns.Add(worksheetName, new Dictionary<String, Int32>());
            }

            return exists;
        }

        /// <summary>
        /// Since other worksheet rows count is unknown at this stage, do not update progress bar inside the function
        /// </summary>
        private void ParseLangs()
        {
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

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
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

                if (_mainFormViewModel != null)
                {
                    _mainFormViewModel.InputProgressBarValue++;
                }
            }
        }

    }
}
