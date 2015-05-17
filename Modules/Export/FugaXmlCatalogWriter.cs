﻿/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using BabelMeta.Helpers;
using BabelMeta.Model;
using BabelMeta.Modules.Export.FugaXml;
using BabelMeta.AppConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BabelMeta.Model.Config;

namespace BabelMeta.Modules.Export
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

        private readonly Dictionary<CatalogTier, ingestionAlbumCatalog_tier> _albumTierConverter =
            new Dictionary<CatalogTier, ingestionAlbumCatalog_tier>
            {
                {CatalogTier.Back, ingestionAlbumCatalog_tier.BACK},
                {CatalogTier.Budget, ingestionAlbumCatalog_tier.BUDGET},
                {CatalogTier.Front, ingestionAlbumCatalog_tier.FRONT},
                {CatalogTier.Mid, ingestionAlbumCatalog_tier.MID},
                {CatalogTier.Premium, ingestionAlbumCatalog_tier.PREMIUM},
            };

        /// <summary>
        /// Not a one-to-one Dictionary. Check business rules.
        /// </summary>
        private readonly Dictionary<CatalogTier, ingestionAlbumTracksClassical_trackCatalog_tier> _isrcTierConverter =
            new Dictionary<CatalogTier, ingestionAlbumTracksClassical_trackCatalog_tier>
            {
                {CatalogTier.Back, ingestionAlbumTracksClassical_trackCatalog_tier.BACK},
                {CatalogTier.Budget, ingestionAlbumTracksClassical_trackCatalog_tier.BACK},
                {CatalogTier.Free, ingestionAlbumTracksClassical_trackCatalog_tier.FREE},
                {CatalogTier.Front, ingestionAlbumTracksClassical_trackCatalog_tier.FRONT},
                {CatalogTier.Mid, ingestionAlbumTracksClassical_trackCatalog_tier.MID},
                {CatalogTier.Premium, ingestionAlbumTracksClassical_trackCatalog_tier.FRONT},
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

                // There MUST be a subfolder for each album
                if (String.IsNullOrEmpty(album.Ean.ToString()))
                {
                    continue;
                }
                String subfolder = rootFolder + album.Ean;
                if (!Directory.Exists(subfolder))
                {
                    Directory.CreateDirectory(subfolder);
                }
                subfolder += "\\";

                string[] files = Directory.GetFiles(subfolder, "*.*");

                GenerateAlbumWiseData(album, i, files);

                foreach (KeyValuePair<Int16, Dictionary<Int16, String>> volume in album.Assets)
                {
                    GenerateTrackWiseData(album, volume, i, files);
                }

                TextWriter tw = new StreamWriter(subfolder + i.album.upc_code + ".xml", false, Encoding.UTF8);
                tw.Write(i.Serialize());
                tw.Close();

            }      

            return ReturnCodes.Ok;
        }

        /// <summary>
        /// Returns the file name, if found, of the file corresponding to criteria
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private String GetFilename(string[] files, FugaIngestionFileType fileType, object parameter = null)
        {
            if (files == null || files.Length == 0)
            {
                return String.Empty;
            }
            var filesList = files.ToList();

            switch (fileType)
            {
                case FugaIngestionFileType.Attachment:
                    foreach (string file in filesList)
                    {
                        var nameExtension = file.Split('.').Last().ToLower();
                        if (!String.IsNullOrEmpty(nameExtension) && nameExtension.CompareTo("pdf") == 0)
                        {
                            return file;
                        }
                    }
                    break;

                case FugaIngestionFileType.AudioTrack:
                    if (parameter != null)
                    {
                        var volumeIndex = ((KeyValuePair<int, int>)parameter).Key;
                        var trackIndex = ((KeyValuePair<int, int>)parameter).Value;
                        foreach (string file in filesList)
                        {
                            var nameExtension = file.Split('.').Last().ToLower();
                            if (
                                    !String.IsNullOrEmpty(nameExtension) &&
                                    (nameExtension.CompareTo("wav") == 0 || nameExtension.CompareTo("aiff") == 0)
                                )
                            {
                                var nameLeft = file.Substring(0, file.Length - 2 - nameExtension.Length);
                                var nameElements = nameLeft.Split('-');
                                var nameElementsCount = nameElements.ToList().Count();
                                if (nameElementsCount < 3)
                                {
                                    continue;
                                }
                                int tryVolumeIndex = 0;
                                int tryTrackIndex = 0;
                                var convertVolume = int.TryParse(nameElements[nameElementsCount - 2], out tryVolumeIndex);
                                var convertTrack = int.TryParse(nameElements[nameElementsCount - 1], out tryTrackIndex);
                                if (
                                        convertVolume 
                                        && convertTrack
                                        && tryVolumeIndex == volumeIndex
                                        && tryTrackIndex == trackIndex
                                    )
                                {
                                    // Found !
                                    return file;
                                }
                            }                            
                        }
                    }
                    break;

                case FugaIngestionFileType.Cover:
                    foreach (string file in filesList)
                    {
                        var nameExtension = file.Split('.').Last().ToLower();
                        if (
                                !String.IsNullOrEmpty(nameExtension) &&
                                (nameExtension.CompareTo("jpg") == 0 || nameExtension.CompareTo("png") == 0)
                            )
                        {
                            return file;
                        }
                    }
                    break;
            }

            return String.Empty;
        }

        private void GenerateAlbumWiseData(Album album, ingestion i, string[] files)
        {
            if (album == null || i == null)
            {
                return;
            }

            // TODO i.album.additional_artists
            // TODO i.album.album_notes
            // TODO i.album.alternate_genre

            i.album.alternate_genreSpecified = false;

            // TODO i.album.alternate_subgenre

            string attachmentFile = GetFilename(files, FugaIngestionFileType.Attachment);
            if (!String.IsNullOrEmpty(attachmentFile))
            {
                i.album.attachments = new List<attachment_type>();
                i.album.attachments.Add(new attachment_type
                    {
                        name = "Booklet",
                        description = "Booklet",
                        file = new file_type { name = attachmentFile },
                    });
            }

            i.album.c_line_text = (!String.IsNullOrEmpty(album.CName)) ? album.CName : CatalogContext.Instance.Settings.COwnerDefault;

            i.album.c_line_year = album.CYear.ToString();

            i.album.catalog_number = album.CatalogReference;

            i.album.catalog_tier = _albumTierConverter[album.Tier];

            i.album.catalog_tierSpecified = true;

            i.album.consumer_release_date = album.ConsumerReleaseDate;

            var coverFile = GetFilename(files, FugaIngestionFileType.Cover);
            if (!String.IsNullOrEmpty(coverFile))
            {
                i.album.cover_art = new ingestionAlbumCover_art();
                i.album.cover_art.image = new ingestionAlbumCover_artImage
                {
                    file = new file_type
                    {
                        name = coverFile,
                    },
                };

            }

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

            i.album.label = album.Owner;

            i.album.language = ingestionAlbumLanguage.EN; // TODO

            i.album.languageSpecified = true;

            i.album.main_genre = genre_type.Classical; // TODO add a converter

            i.album.main_subgenre = album.Subgenre.Name;

            i.album.name = album.Title[CatalogContext.Instance.DefaultLang];

            // TODO i.album.original_release_date
            // TODO i.album.original_release_dateSpecified

            i.album.p_line_text = (!String.IsNullOrEmpty(album.PName)) ? album.PName : CatalogContext.Instance.Settings.POwnerDefault;

            i.album.p_line_year = album.PYear.ToString();

            i.album.parental_advisory = parental_advisory.@false; // TODO add setting

            i.album.parental_advisorySpecified = true;

            // TODO i.album.pricing_intervals
            // TODO i.album.pricings

            if (album.PrimaryArtistId > 0)
            {
                Artist primaryArtist = CatalogContext.Instance.Artists.FirstOrDefault(e => e.Id == (Int32)album.PrimaryArtistId);
                if (primaryArtist.LastName.ContainsKey(CatalogContext.Instance.DefaultLang))
                {
                    i.album.primary_artist = new primary_artist
                    {
                        name = (primaryArtist.FirstName[CatalogContext.Instance.DefaultLang] + " " + primaryArtist.LastName[CatalogContext.Instance.DefaultLang]).Trim(),
                    };
                }
            }

            i.album.recording_location = album.RecordingLocation;

            i.album.recording_year = album.RecordingYear.ToString();

            i.album.redeliveries = new redeliveries_type
            {
                redeliver = false,
            };


            i.album.release_format_type = ingestionAlbumRelease_format_type.ALBUM; // TODO translater

            // TODO i.album.release_version
            // TODO i.album.schedule

            i.album.supplier = CatalogContext.Instance.Settings.SupplierDefault;

            i.album.territories = new List<territory_code>();
            i.album.territories.Add(territory_code.WORLD);

            i.album.total_discs = album.TotalDiscs.ToString();
            
            i.album.upc_code = album.Ean.ToString();

            // TODO i.album.usage_rights

        }

        private void GenerateTrackWiseData(Album album, KeyValuePair<Int16, Dictionary<Int16, String>> volume, ingestion i, string[] files)
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
                    continue;
                }
                // The asset's work is either standalone or a child work.
                Work currentWork = CatalogContext.Instance.Works.FirstOrDefault(e => e.Id == isrc.Work);
                if (currentWork == null)
                {
                    continue;
                }
                Work parentWork = (currentWork.Parent > 0)
                    ? CatalogContext.Instance.Works.FirstOrDefault(w => w.Id == currentWork.Parent)
                    : null;
                var isrcPerformersKeys = isrc.Contributors.Keys.ToArray();
                if (isrcPerformersKeys == null || !(isrcPerformersKeys.Length > 0))
                {
                    continue;
                }

                ingestionAlbumTracksClassical_track asset = new ingestionAlbumTracksClassical_track();

                // Additional artists, if any, are from the second performer.
                if (isrcPerformersKeys.Length > 1)
                {
                    asset.additional_artists = new List<artist>();
                }
                for (var j = 1; j < isrcPerformersKeys.Length; j++)
                {
                    Artist additionalArtist = CatalogContext.Instance.Artists.FirstOrDefault(e =>
                        e.Id == isrcPerformersKeys[j]);
                    if (additionalArtist.LastName.ContainsKey(CatalogContext.Instance.DefaultLang))
                    {
                        asset.additional_artists.Add(new artist
                        {
                            name = (additionalArtist.FirstName[CatalogContext.Instance.DefaultLang] + " " + additionalArtist.LastName[CatalogContext.Instance.DefaultLang]).Trim(),
                            primary_artist = true, // TODO manage primary character for additional artists
                        });
                    }
                }

                asset.allow_preorder_preview = false;

                asset.allow_preorder_previewSpecified = true;

                // TODO asset.alternate_genre

                asset.alternate_genreSpecified = false;

                // TODO asset.alternate_subgenre

                asset.always_send_display_title = true;

                asset.always_send_display_titleSpecified = true;

                asset.available_separately = isrc.AvailableSeparately;

                asset.catalog_tier = _isrcTierConverter[(CatalogTier)isrc.Tier];

                asset.catalog_tierSpecified = true;

                asset.classical_catalog = (parentWork == null) ? currentWork.ClassicalCatalog : parentWork.ClassicalCatalog;

                asset.contributors = new List<contributor>(); // These represent work-related contributors like typically Composer, Arranger, etc.
                foreach (KeyValuePair<Int32, Role> workContributor in currentWork.Contributors)
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
                    contributorRole cRole = (_roleConverter.ContainsKey((BabelMeta.Model.Role.QualifiedName)role.Reference))
                        ? _roleConverter[(BabelMeta.Model.Role.QualifiedName)role.Reference] 
                        : contributorRole.ContributingArtist;

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
                        currentWork.Title.ContainsKey(CatalogContext.Instance.DefaultLang)
                        &&  (
                                parentWork == null
                                || parentWork.Title.ContainsKey(CatalogContext.Instance.DefaultLang)
                            )
                    )
                {
                    asset.display_title = (parentWork == null)
                        ? currentWork.Title[CatalogContext.Instance.DefaultLang]
                        : parentWork.Title[CatalogContext.Instance.DefaultLang] + Syntax.HierarchicalSeparator(CatalogContext.Instance.DefaultLang) + currentWork.Title[CatalogContext.Instance.DefaultLang];
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

                asset.keySpecified = (parentWork == null && currentWork.Tonality != null) || (parentWork != null && parentWork.Tonality != null);

                if (asset.keySpecified)
                {
                    asset.key = (parentWork == null)
                        ? _keyConverter[(Key)(currentWork.Tonality)]
                        : _keyConverter[(Key)(parentWork.Tonality)];
                }

                // TODO asset.lyrics

                if (album.Subgenre != null)
                {
                    asset.main_subgenre = album.Subgenre.Name; // TODO implement trackwise field
                }

                if  (
                        parentWork != null 
                        && currentWork.MovementTitle.ContainsKey(CatalogContext.Instance.DefaultLang)
                        && !String.IsNullOrEmpty(currentWork.MovementTitle[CatalogContext.Instance.DefaultLang])
                    )
                {
                    asset.movement = currentWork.MovementTitle[CatalogContext.Instance.DefaultLang];
                }

                if (parentWork != null && currentWork.MovementNumber > 0)
                {
                    asset.movement_number = currentWork.MovementNumber.ToString();
                }

                asset.on_disc = volumeIndex.ToString();

                asset.p_line_text = isrc.PName;

                asset.p_line_year = isrc.PYear.ToString();

                asset.parental_advisory = parental_advisory.@false; // TODO add seting

                // TODO asset.preorder_type

                asset.preorder_typeSpecified = false;

                asset.preview_length = "30"; // TODO add setting
                
                asset.preview_start = "0";

                // Primary artists is performer 1 (isrc contributor 1)
                Artist primaryArtist = CatalogContext.Instance.Artists.FirstOrDefault(e =>
                    e.Id == isrcPerformersKeys[0]);
                if (primaryArtist.LastName.ContainsKey(CatalogContext.Instance.DefaultLang))
                {
                    asset.primary_artist = new primary_artist()
                    {
                        name = (primaryArtist.FirstName[CatalogContext.Instance.DefaultLang] + " " + primaryArtist.LastName[CatalogContext.Instance.DefaultLang]).Trim(),
                    };
                }

                // TODO asset.publishers

                asset.recording_location = isrc.RecordingLocation;

                asset.recording_year = isrc.RecordingYear.ToString();

                // TODO asset.redeliveries_of_associated
                var audioFilename = GetFilename(files, FugaIngestionFileType.AudioTrack, new KeyValuePair<int, int>(volumeIndex, trackIndex));
                if (!String.IsNullOrEmpty(audioFilename))
                {
                    asset.resources = new List<resourcesAudio>();
                    asset.resources.Add(new resourcesAudio 
                    {
                        file = new file_type
                        {
                            name = audioFilename,
                        }
                    });
                }

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
