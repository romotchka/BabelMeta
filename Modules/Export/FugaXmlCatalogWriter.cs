/*
 * Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using MetadataConverter.Model;
using MetadataConverter.Modules.Export.FugaXml;
using MetadataConverter.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetadataConverter.Modules.Export
{
    public class FugaXmlCatalogWriter : ICatalogWriter
    {
        MainFormViewModel _viewModel = null;

        private static FugaXmlCatalogWriter _instance;

        private FugaXmlCatalogWriter()
        {

        }

        public static FugaXmlCatalogWriter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FugaXmlCatalogWriter();

                }
                return _instance;
            }
        }

        public ReturnCodes Generate(String rootFolder, MainFormViewModel viewModel = null)
        {
            if (String.IsNullOrEmpty(rootFolder))
            {
                return ReturnCodes.ModulesExportFugaXmlGenerateNullFolderName;
            }

            if ((rootFolder.ToCharArray())[rootFolder.Length-1] != '\\')
            {
                rootFolder += "\\";
            }

            _viewModel = viewModel;

            foreach (Album album in CatalogContext.Instance.Albums)
            {
                ingestion i = null;
                i = new ingestion();
                i.album = new ingestionAlbum();
                i.album.tracks = new ingestionAlbumTracks();

                GenerateAlbumWiseData(album,i);

                foreach (KeyValuePair<Int16, Dictionary<Int16, String>> volume in album.Assets)
                {
                    GenerateTrackWiseData(album, volume, i);
                }

                // There MUST be a subfolder for each album
                if (String.IsNullOrEmpty(i.album.upc_code))
                {
                    continue;
                }
                String subfolder = rootFolder + i.album.upc_code;
                if (!Directory.Exists(subfolder))
                {
                    Directory.CreateDirectory(subfolder);
                }
                subfolder += "\\";

                TextWriter tw = new StreamWriter(subfolder + i.album.upc_code + ".xml", false, Encoding.UTF8);
                tw.Write(i.Serialize());
                tw.Close();

            }      

            return ReturnCodes.Ok;
        }

        private void GenerateAlbumWiseData(Album album, ingestion i)
        {
            if (album == null || i == null)
            {
                return;
            }
            i.album.upc_code = album.Ean.ToString();
        }

        private void GenerateTrackWiseData(Album album, KeyValuePair<Int16, Dictionary<Int16, String>> volume, ingestion i)
        {
            if (album == null || i == null)
            {
                return;
            }
            Int16 volumeIndex = volume.Key;
            Dictionary<Int16, String> volumeTracks = volume.Value;

            List<Artist> artistsBuffer = new List<Artist>();

            foreach (KeyValuePair<Int16, String> track in volumeTracks)
            {
                Int16 trackIndex = track.Key;
                String isrcId = track.Value;
                Isrc isrc = CatalogContext.Instance.Isrcs.FirstOrDefault(e => e.Id.CompareTo(isrcId) == 0);
                if (isrc == null)
                {
                    return;
                }
                Work work = CatalogContext.Instance.Works.FirstOrDefault(e => e.Id == isrc.Work);
                if (work == null)
                {
                    return;
                }

                ingestionAlbumTracksClassical_track asset = new ingestionAlbumTracksClassical_track();

                asset.on_disc = volumeIndex.ToString();
                asset.sequence_number = trackIndex.ToString();

                i.album.tracks.Items.Add(asset);

            }
        }

    }
}
