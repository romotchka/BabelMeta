/*
 * Classical Metadata Converter
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

            foreach (Album album in CatalogContext.Instance.Albums)
            {
                ingestion i = new ingestion();
                i.album = new ingestionAlbum();
                i.album.tracks = new ingestionAlbumTracks();

                GenerateAlbumWiseData(i.album);

                foreach (KeyValuePair<Int16, Dictionary<Int16, String>> volume in album.Assets)
                {
                    GenerateVolumeWiseData(i.album, volume);
                }

                String s = i.Serialize();
                byte[] b = Encoding.UTF8.GetBytes(s);

            }      

            return ReturnCodes.Ok;
        }

        private void GenerateAlbumWiseData(ingestionAlbum album)
        {

        }

        private void GenerateVolumeWiseData(ingestionAlbum album, KeyValuePair<Int16, Dictionary<Int16, String>> volume)
        {
            Int16 volumeIndex = volume.Key;
            Dictionary<Int16, String> volumeTracks = volume.Value;
            foreach (KeyValuePair<Int16, String> track in volumeTracks)
            {
                GenerateTrackWiseData(album, track);
            }
        }

        private void GenerateTrackWiseData(ingestionAlbum album, KeyValuePair<Int16, String> track)
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
        }
    }
}
