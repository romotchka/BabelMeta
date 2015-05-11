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

        // Exact matching between Key terminology
        private readonly Dictionary<Key, ingestionAlbumTracksClassical_trackKey> _keyConverter =
            new Dictionary<Key, ingestionAlbumTracksClassical_trackKey>
            {
                {Key.AFlatMajor, ingestionAlbumTracksClassical_trackKey.A_FLAT_MAJOR},
                {Key.AMajor,ingestionAlbumTracksClassical_trackKey.A_MAJOR},
                {Key.AMinor,ingestionAlbumTracksClassical_trackKey.A_MINOR},
                {Key.BFlatMajor,ingestionAlbumTracksClassical_trackKey.B_FLAT_MAJOR},
                {Key.BFlatMinor,ingestionAlbumTracksClassical_trackKey.B_FLAT_MINOR},
                {Key.BMajor,ingestionAlbumTracksClassical_trackKey.B_MAJOR},
                {Key.BMinor,ingestionAlbumTracksClassical_trackKey.B_MINOR},
                {Key.CMajor,ingestionAlbumTracksClassical_trackKey.C_MAJOR},
                {Key.CMinor,ingestionAlbumTracksClassical_trackKey.C_MINOR},
                {Key.CSharpMinor,ingestionAlbumTracksClassical_trackKey.C_SHARP_MINOR},
                {Key.DFlatMajor,ingestionAlbumTracksClassical_trackKey.D_FLAT_MAJOR},
                {Key.DMajor,ingestionAlbumTracksClassical_trackKey.D_MAJOR},
                {Key.DMinor,ingestionAlbumTracksClassical_trackKey.D_MINOR},
                {Key.DSharpMinor,ingestionAlbumTracksClassical_trackKey.D_SHARP_MINOR},
                {Key.EFlatMajor,ingestionAlbumTracksClassical_trackKey.E_FLAT_MAJOR},
                {Key.EFlatMinor,ingestionAlbumTracksClassical_trackKey.E_FLAT_MINOR},
                {Key.EMajor,ingestionAlbumTracksClassical_trackKey.E_MAJOR},
                {Key.EMinor,ingestionAlbumTracksClassical_trackKey.E_MINOR},
                {Key.FMajor,ingestionAlbumTracksClassical_trackKey.F_MAJOR},
                {Key.FMinor,ingestionAlbumTracksClassical_trackKey.F_MINOR},
                {Key.FSharpMajor,ingestionAlbumTracksClassical_trackKey.F_SHARP_MAJOR},
                {Key.FSharpMinor,ingestionAlbumTracksClassical_trackKey.F_SHARP_MINOR},
                {Key.GFlatMajor,ingestionAlbumTracksClassical_trackKey.G_FLAT_MAJOR},
                {Key.GMajor,ingestionAlbumTracksClassical_trackKey.G_MAJOR},
                {Key.GMinor,ingestionAlbumTracksClassical_trackKey.G_MINOR},
                {Key.GSharpMinor,ingestionAlbumTracksClassical_trackKey.G_SHARP_MINOR},
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

            // TODO i.album.additional_artists
            // TODO i.album.album_notes
            // TODO i.album.alternate_genre
            // TODO i.album.alternate_genreSpecified
            // TODO i.album.alternate_subgenre
            // TODO i.album.attachments
            // TODO i.album.c_line_text
            // TODO i.album.c_line_year
            // TODO i.album.catalog_number
            // TODO i.album.catalog_tier
            // TODO i.album.catalog_tierSpecified
            // TODO i.album.consumer_release_date
            // TODO i.album.cover_art
            // TODO i.album.display_artist
            // TODO i.album.extra1
            // TODO i.album.extra2
            // TODO i.album.extra3
            // TODO i.album.extra4
            // TODO i.album.extra5
            // TODO i.album.extra6
            // TODO i.album.extra7
            // TODO i.album.extra8
            // TODO i.album.extra9
            // TODO i.album.extra9Specified
            // TODO i.album.extra10
            // TODO i.album.extra10Specified
            // TODO i.album.fuga_id
            // TODO i.album.label

            i.album.language = ingestionAlbumLanguage.EN; // TODO

            // TODO i.album.languageSpecified
            // TODO i.album.main_genre
            // TODO i.album.main_subgenre
            // TODO i.album.name
            // TODO i.album.original_release_date
            // TODO i.album.original_release_dateSpecified
            // TODO i.album.p_line_text
            // TODO i.album.p_line_year

            i.album.parental_advisory = parental_advisory.@false; // TODO add setting

            // TODO i.album.parental_advisorySpecified
            // TODO i.album.pricing_intervals
            // TODO i.album.pricings
            // TODO i.album.primary_artist
            // TODO i.album.recording_location
            // TODO i.album.recording_year
            // TODO i.album.redeliveries
            // TODO i.album.release_format_type
            // TODO i.album.release_version
            // TODO i.album.schedule
            // TODO i.album.supplier
            // TODO i.album.territories
            // TODO i.album.total_discs

            i.album.upc_code = album.Ean.ToString();

            // TODO i.album.usage_rights

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
                    Artist artist;
                    if (artistsBuffer.Exists(a => a.Id == workContributor.Key))
                    {
                        // Present in buffer
                        artist = artistsBuffer.FirstOrDefault(a => a.Id == workContributor.Key);
                    }
                    else
                    {
                        // Find then add to buffer
                        artist = CatalogContext.Instance.Artists.FirstOrDefault(a => a.Id == workContributor.Key);
                        artistsBuffer.Add(artist);
                    }
                    Role role = CatalogContext.Instance.Roles.FirstOrDefault(r => r.Name.CompareTo(workContributor.Value.Name) == 0);
                    contributorRole cRole = (_roleConverter.ContainsKey(role.Reference)) ? _roleConverter[role.Reference] : contributorRole.ContributingArtist;

                    if (artist.LastName.ContainsKey(CatalogContext.Instance.DefaultLang))
                    {
                        asset.contributors.Add(new contributor()
                        {
                            name = (artist.FirstName[CatalogContext.Instance.DefaultLang] + " " + artist.LastName[CatalogContext.Instance.DefaultLang]).Trim(),
                            role = cRole,
                        });
                    }
                }

                // TODO asset.country_of_commissioning
                // TODO asset.country_of_recording

                if  (
                        childWork.Title.ContainsKey(CatalogContext.Instance.DefaultLang)
                        &&  (
                                parentWork == null
                                || parentWork.Title.ContainsKey(CatalogContext.Instance.DefaultLang)
                            )
                    )
                {
                    asset.display_title = (parentWork == null)
                        ? childWork.Title[CatalogContext.Instance.DefaultLang]
                        : parentWork.Title[CatalogContext.Instance.DefaultLang] + Syntax.HierarchicalSeparator(CatalogContext.Instance.DefaultLang) + childWork.Title[CatalogContext.Instance.DefaultLang];
                }

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

                asset.keySpecified = (parentWork == null && childWork.Tonality != null) || (parentWork != null && parentWork.Tonality != null);

                if (asset.keySpecified)
                {
                    asset.key = (parentWork == null)
                        ? _keyConverter[(Key)(childWork.Tonality)]
                        : _keyConverter[(Key)(parentWork.Tonality)];
                }

                // TODO asset.lyrics

                if (album.Subgenre != null)
                {
                    asset.main_subgenre = album.Subgenre.Name; // TODO implement trackwise field
                }

                if  (
                        parentWork != null 
                        && childWork.MovementTitle.ContainsKey(CatalogContext.Instance.DefaultLang)
                        && !String.IsNullOrEmpty(childWork.MovementTitle[CatalogContext.Instance.DefaultLang])
                    )
                {
                    asset.movement = childWork.MovementTitle[CatalogContext.Instance.DefaultLang];
                }

                if (parentWork != null && childWork.MovementNumber > 0)
                {
                    asset.movement_number = childWork.MovementNumber.ToString();
                }

                asset.on_disc = volumeIndex.ToString();

                // TODO asset.p_line_text
                // TODO asset.p_line_year

                asset.parental_advisory = parental_advisory.@false; // TODO add seting

                // TODO asset.preorder_type
                // TODO asset.preorder_typeSpecified

                asset.preview_length = "30"; // TODO add setting
                
                asset.preview_start = "0";

                Artist primaryArtist = CatalogContext.Instance.Artists.FirstOrDefault(e =>
                    e.Id == (isrc.Contributors.Keys.ToArray())[0]);
                if (primaryArtist.LastName.ContainsKey(CatalogContext.Instance.DefaultLang))
                {
                    asset.primary_artist = new primary_artist();
                    asset.primary_artist.name = (primaryArtist.FirstName[CatalogContext.Instance.DefaultLang] + " " + primaryArtist.LastName[CatalogContext.Instance.DefaultLang]).Trim();
                }

                // TODO asset.publishers
                // TODO asset.recording_location
                // TODO asset.recording_year
                // TODO asset.redeliveries_of_associated
                // TODO asset.resources
                // TODO asset.rights_contract_begin_date
                // TODO asset.rights_contract_begin_dateSpecified
                // TODO asset.rights_holder_name
                // TODO asset.rights_ownership_name

                asset.sequence_number = trackIndex.ToString();

                // TODO asset.track_notes
                // TODO asset.track_version
                // TODO asset.usage_rights
                
                if (parentWork != null && parentWork.Title.ContainsKey(CatalogContext.Instance.DefaultLang))
                {
                    asset.work = parentWork.Title[CatalogContext.Instance.DefaultLang];
                }

                // Add asset
                i.album.tracks.Items.Add(asset);
            }
        }
    }
}
