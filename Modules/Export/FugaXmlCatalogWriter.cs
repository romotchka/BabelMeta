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
        private String _folder = String.Empty;

        MainFormViewModel _viewModel = null;

        ingestion _ingestion = null;

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

        public ReturnCodes Generate(String folder, MainFormViewModel viewModel = null)
        {
            if (String.IsNullOrEmpty(folder))
            {
                return ReturnCodes.ModulesExportFugaXmlGenerateNullFolderName;
            }
            _folder = folder;
            _viewModel = viewModel;

            foreach (Album album in CatalogContext.Instance.Albums)
            {
                _ingestion = null;
                _ingestion = new ingestion();
                _ingestion.album = new ingestionAlbum();
                _ingestion.album.tracks = new ingestionAlbumTracks();

                GenerateAlbumWiseData();

                foreach (KeyValuePair<Int16, Dictionary<Int16, String>> volume in album.Assets)
                {
                    GenerateTrackWiseData(volume);
                }

                //String s = i.Serialize();
                //byte[] b = Encoding.UTF8.GetBytes(s);

            }      

            return ReturnCodes.Ok;
        }

        private void GenerateAlbumWiseData()
        {

        }

        private void GenerateTrackWiseData(KeyValuePair<Int16, Dictionary<Int16, String>> volume)
        {
            if (_ingestion == null || _ingestion.album == null)
            {
                return;
            }
            Int16 volumeIndex = volume.Key;
            Dictionary<Int16, String> volumeTracks = volume.Value;
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

                _ingestion.album.tracks.Items.Add(asset);


                // File generation

            }
        }

    }
}
