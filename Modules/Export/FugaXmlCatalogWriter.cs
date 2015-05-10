/*
 * Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using MetadataConverter.Helpers;
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
        // Translation of standardized roles in platform-specific ones
        // TODO: Complete.
        private readonly Dictionary<Role.QualifiedName, contributorRole> _roleConverter =
            new Dictionary<Role.QualifiedName, contributorRole>
            {
                {Role.QualifiedName.Arranger, contributorRole.Arranger},
                {Role.QualifiedName.Composer, contributorRole.Composer},
                {Role.QualifiedName.Conductor, contributorRole.Conductor},
                {Role.QualifiedName.Engineer, contributorRole.Engineer},
                {Role.QualifiedName.Ensemble, contributorRole.Ensemble},
                {Role.QualifiedName.Performer, contributorRole.Performer},
                {Role.QualifiedName.Transcriptor, contributorRole.Arranger},
            };

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

            // Artists buffer for performance improvement
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
                // The asset's work is either standalone or a child work.
                Work childWork = CatalogContext.Instance.Works.FirstOrDefault(e => e.Id == isrc.Work);
                if (childWork == null)
                {
                    return;
                }
                Work parentWork = (childWork.Parent > 0)
                    ? CatalogContext.Instance.Works.FirstOrDefault(w => w.Id == childWork.Parent)
                    : null;

                ingestionAlbumTracksClassical_track asset = new ingestionAlbumTracksClassical_track();

                // TODO asset.additional_artists
                // TODO asset.allow_preorder_preview
                // TODO asset.allow_preorder_previewSpecified
                // TODO asset.alternate_genre
                // TODO asset.alternate_genreSpecified
                // TODO asset.alternate_subgenre

                asset.always_send_display_title = true;

                asset.always_send_display_titleSpecified = true;

                // TODO asset.available_separately
                // TODO asset.catalog_tier 
                // TODO asset.catalog_tierSpecified

                asset.classical_catalog = (parentWork == null) ? childWork.ClassicalCatalog : parentWork.ClassicalCatalog;

                asset.contributors = new List<contributor>(); // These represent work-related contributors like typically Composer, Arranger, etc.
                foreach (KeyValuePair<Int32, Role> workContributor in childWork.Contributors)
                {
                    Artist artist = CatalogContext.Instance.Artists.FirstOrDefault(a => a.Id == workContributor.Key);
                    Role role = CatalogContext.Instance.Roles.FirstOrDefault(r => r.Name.CompareTo(workContributor.Value.Name) == 0);
                    contributorRole cRole = (_roleConverter.ContainsKey(role.Reference)) ? _roleConverter[role.Reference] : contributorRole.ContributingArtist;

                    asset.contributors.Add(new contributor()
                    {
                        name = (artist.FirstName[CatalogContext.Instance.DefaultLang] + " " + artist.LastName[CatalogContext.Instance.DefaultLang]).Trim(),
                        role = cRole,
                    });
                }

                // TODO asset.country_of_commissioning
                // TODO asset.country_of_recording

                asset.display_title = (parentWork == null)
                    ? childWork.Title[CatalogContext.Instance.DefaultLang]
                    : parentWork.Title[CatalogContext.Instance.DefaultLang] + Syntax.HierarchicalSeparator(CatalogContext.Instance.DefaultLang) + childWork.Title[CatalogContext.Instance.DefaultLang];

                // TODO asset.extra1
                // TODO asset.extra2
                // TODO asset.extra3
                // TODO asset.extra4
                // TODO asset.extra5
                // TODO asset.extra6
                // TODO asset.extra7
                // TODO asset.extra8
                // TODO asset.extra9
                // TODO asset.extra9Specified
                // TODO asset.extra10
                // TODO asset.extra10Specified

                asset.isrc_code = isrcId;

                // TODO etc.

                asset.on_disc = volumeIndex.ToString();

                asset.sequence_number = trackIndex.ToString();

                // Add asset
                i.album.tracks.Items.Add(asset);
            }
        }
    }
}
