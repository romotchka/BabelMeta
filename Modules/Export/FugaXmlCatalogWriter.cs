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

using System.Windows.Forms;
using BabelMeta.AppConfig;
using BabelMeta.Helpers;
using BabelMeta.Model;
using BabelMeta.Model.Config;
using BabelMeta.Modules.Export.FugaXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BabelMeta.Modules.Export
{
    /// <summary>
    /// Implementation of ICatalogWriter interface generating Fuga (http://www.fuga.com) ingestion files.
    /// If the target repertoire matches the tree structure expected (one subfolder named according to the UPC/EAN of each album),
    /// the Writer will scan audio files and attachments so as to include them in metadata.
    /// </summary>
    public class FugaXmlCatalogWriter : ICatalogWriter
    {
        // Translation of standardized roles in platform-specific ones
        // TODO: Complete.
        private readonly Dictionary<Album.ActionType, ingestionAction> _actionConverter =
            new Dictionary<Album.ActionType, ingestionAction>
            {
                {Album.ActionType.Insert, ingestionAction.INSERT},
                {Album.ActionType.Update, ingestionAction.UPDATE},
            };

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

        MainFormViewModel _mainFormViewModel = null;

        private static FugaXmlCatalogWriter _instance;

        private FugaXmlCatalogWriter()
        {

        }

        public static FugaXmlCatalogWriter Instance
        {
            get { return _instance ?? (_instance = new FugaXmlCatalogWriter()); }
        }

        #region Generate
        /// <summary>
        /// Main generation method.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public ReturnCode Generate(object context, MainFormViewModel viewModel = null)
        {
            var rootFolder = (String)context;
            if (String.IsNullOrEmpty(rootFolder))
            {
                return ReturnCode.ModulesExportFugaXmlGenerateNullFolderName;
            }

            if ((rootFolder.ToCharArray())[rootFolder.Length-1] != '\\')
            {
                rootFolder += "\\";
            }

            _mainFormViewModel = viewModel;

            if (!CatalogContext.Instance.Initialized)
            {
                return ReturnCode.ModulesExportCatalogContextNotInitialized;
            }

            foreach (var album in CatalogContext.Instance.Albums)
            {
                var i = new ingestion
                {
                    album = new ingestionAlbum
                    {
                        tracks = new ingestionAlbumTracks(),
                    },
                };

                // There MUST be a subfolder for each album
                if (String.IsNullOrEmpty(album.Ean.ToString()))
                {
                    Notify(String.Format("Album [{0}]: Ean is empty.", album.CatalogReference));
                    continue;
                }

                var subfolder = rootFolder + album.Ean;
                if (!Directory.Exists(subfolder))
                {
                    Directory.CreateDirectory(subfolder);
                }
                subfolder += "\\";

                var files = Directory.GetFiles(subfolder, "*.*");

                GenerateAlbumWiseData(album, i, files);

                if (album.Tracks != null)
                {
                    foreach (var volume in album.Tracks)
                    {
                        GenerateTrackWiseData(album, volume, i, files);
                    }
                }

                var tw = new StreamWriter(subfolder + i.album.upc_code + ".xml", false, Encoding.UTF8);
                tw.Write(i.Serialize().WithFugaXmlHeader());
                tw.Close();

                // Generate the empty '.complete' file as requested
                var complete = new StreamWriter(subfolder + album.Ean + ".complete", false, Encoding.ASCII); // ASCII encoding is to ensure strict 0Kb on disk
                complete.Close();
            }

            return ReturnCode.Ok;
        }

        private void GenerateAlbumWiseData(Album album, ingestion i, String[] files)
        {
            if (album == null || i == null)
            {
                return;
            }

            // Action
            try
            {
                i.action = _actionConverter[(Album.ActionType)album.ActionTypeValue];
            }
            catch (Exception ex)
            {
                Notify(String.Format("Album [{0}]: Invalid action.", album.CatalogReference));
                Debug.Write(this, "FugaXmlCatalogWriter.GenerateAlbumWiseData, exception=" + ex.Message);
            }

            // TODO i.album.additional_artists
            // TODO i.album.album_notes
            // TODO i.album.alternate_genre

            i.album.alternate_genreSpecified = false;

            // TODO i.album.alternate_subgenre

            var attachmentFile = SearchFilename(files, FugaIngestionFileType.Attachment);
            if (!String.IsNullOrEmpty(attachmentFile))
            {
                var f = new FileInfo(attachmentFile);
                i.album.attachments = new List<attachment_type>
                {
                    new attachment_type
                    {
                        name = "Booklet",
                        description = "Booklet",
                        file = new file_type
                        {
                            name = attachmentFile.GetFileNameFromFullPath(),
                            size = (ulong) f.Length,
                        },
                    },
                };
            }

            i.album.c_line_text = (!String.IsNullOrEmpty(album.CName)) ? album.CName.Typo() : CatalogContext.Instance.Settings.COwnerDefault.Typo();

            if (album.CYear != null)
            {
                i.album.c_line_year = album.CYear.ToString();
            }

            i.album.catalog_number = album.CatalogReference.Typo();

            i.album.catalog_tier = _albumTierConverter[album.Tier];

            i.album.catalog_tierSpecified = true;

            i.album.consumer_release_date = album.ConsumerReleaseDate;

            var coverFile = SearchFilename(files, FugaIngestionFileType.Cover);
            if (!String.IsNullOrEmpty(coverFile))
            {
                var f = new FileInfo(coverFile);
                i.album.cover_art = new ingestionAlbumCover_art
                {
                    image = new ingestionAlbumCover_artImage
                    {
                        file = new file_type
                        {
                            name = coverFile.GetFileNameFromFullPath(),
                            size = (ulong)f.Length,
                        },
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

            i.album.label = album.Owner.Typo();

            i.album.language = ingestionAlbumLanguage.EN; // TODO

            i.album.languageSpecified = true;

            i.album.main_genre = genre_type.Classical; // TODO add a converter

            if (album.Subgenre != null && !String.IsNullOrEmpty(album.Subgenre.Name))
            {
                i.album.main_subgenre = album.Subgenre.Name.Typo();
            }
            else
            {
                Notify(String.Format("Album [{0}]: Missing subgenre.", album.CatalogReference));
            }

            if (album.Title != null && album.Title.ContainsKey(CatalogContext.Instance.DefaultLang.ShortName))
            {
                i.album.name = album.Title[CatalogContext.Instance.DefaultLang.ShortName].Typo();
            }

            // TODO i.album.original_release_date
            // TODO i.album.original_release_dateSpecified

            i.album.p_line_text = (!String.IsNullOrEmpty(album.PName)) ? album.PName.Typo() : CatalogContext.Instance.Settings.POwnerDefault.Typo();

            if (album.PYear != null)
            {
                i.album.p_line_year = album.PYear.ToString();
            }

            i.album.parental_advisory = parental_advisory.@false; // TODO add setting

            i.album.parental_advisorySpecified = true;

            // TODO i.album.pricing_intervals
            // TODO i.album.pricings

            if (album.PrimaryArtistId > 0)
            {
                var primaryArtist = CatalogContext.Instance.Artists.FirstOrDefault(e => e.Id.Equals(album.PrimaryArtistId));
                if (primaryArtist != null && primaryArtist.LastName.ContainsKey(CatalogContext.Instance.DefaultLang.ShortName))
                {
                    i.album.primary_artist = new primary_artist
                    {
                        name = (primaryArtist.FirstName[CatalogContext.Instance.DefaultLang.ShortName].Typo() + " " + primaryArtist.LastName[CatalogContext.Instance.DefaultLang.ShortName].Typo()).Trim(),
                    };
                }
            }

            i.album.recording_location = album.RecordingLocation.Typo();

            if (album.RecordingYear > 0)
            {
                i.album.recording_year = album.RecordingYear.ToString();
            }

            i.album.redeliveries = new redeliveries_type
            {
                redeliver = false,
            };


            i.album.release_format_type = ingestionAlbumRelease_format_type.ALBUM; // TODO translater

            // TODO i.album.release_version
            // TODO i.album.schedule

            i.album.supplier = CatalogContext.Instance.Settings.SupplierDefault.Typo();

            i.album.territories = new List<territory_code>
            {
                territory_code.WORLD,
            };

            if (album.TotalDiscs != null)
            {
                i.album.total_discs = album.TotalDiscs.ToString();
            }
            else
            {
                Notify(String.Format("Album [{0}]: Missing total discs.", album.CatalogReference));
            }

            if (album.Ean != null)
            {
                i.album.upc_code = album.Ean.ToString();
            }
            else
            {
                Notify(String.Format("Album [{0}]: Missing UPC/EAN.", album.CatalogReference));
            }

            // TODO i.album.usage_rights

        }

        private void GenerateTrackWiseData(Album album, KeyValuePair<short, Dictionary<short, String>> volume, ingestion i, String[] files)
        {
            if (album == null || i == null)
            {
                return;
            }
            var volumeIndex = volume.Key;
            var volumeTracks = volume.Value;

            // Artists buffer for performance improvement
            var artistsBuffer = new List<Artist>();

            foreach (var volumeTrack in volumeTracks)
            {
                var trackIndex = volumeTrack.Key;
                var assetId = volumeTrack.Value;
                var asset = CatalogContext.Instance.Assets.FirstOrDefault(e => String.Compare(e.Id, assetId, StringComparison.Ordinal) == 0);
                if (asset == null)
                {
                    Notify(String.Format("Album [{0}] [{1},{2}]: Asset key reference not found.", album.CatalogReference, volumeIndex, trackIndex));
                    continue;
                }

                // The asset's work is either standalone or a child work.
                var currentWork = CatalogContext.Instance.Works.FirstOrDefault(e => e.Id.Equals(asset.Work));
                if (currentWork == null)
                {
                    Notify(String.Format("Album [{0}] [{1},{2}]: Work not found for asset {3}.", album.CatalogReference, volumeIndex, trackIndex, asset.Id));
                    continue;
                }
                var parentWork = (currentWork.Parent > 0)
                    ? CatalogContext.Instance.Works.FirstOrDefault(w => w.Id.Equals(currentWork.Parent))
                    : null;
                var assetPerformersKeys = asset.Contributors.Keys.ToArray();
                if (assetPerformersKeys == null || !(assetPerformersKeys.Length > 0))
                {
                    Notify(String.Format("Album [{0}] [{1},{2}]: No performers.", album.CatalogReference, volumeIndex, trackIndex));
                    continue;
                }

                var ingestionTrack = new ingestionAlbumTracksClassical_track();

                // Additional artists, if any, are from the second performer.
                if (assetPerformersKeys.Length > 1)
                {
                    ingestionTrack.additional_artists = new List<artist>();
                }
                for (var j = 1; j < assetPerformersKeys.Length; j++)
                {
                    var additionalArtist = CatalogContext.Instance.Artists.FirstOrDefault(e =>
                        e.Id.Equals(assetPerformersKeys[j]));
                    if (additionalArtist.LastName.ContainsKey(CatalogContext.Instance.DefaultLang.ShortName))
                    {
                        ingestionTrack.additional_artists.Add(new artist
                        {
                            name = (additionalArtist.FirstName[CatalogContext.Instance.DefaultLang.ShortName].Typo() + " " + additionalArtist.LastName[CatalogContext.Instance.DefaultLang.ShortName].Typo()).Trim(),
                            primary_artist = true, // TODO manage primary character for additional artists
                        });
                    }
                }

                ingestionTrack.allow_preorder_preview = false;

                ingestionTrack.allow_preorder_previewSpecified = true;

                // TODO asset.alternate_genre

                ingestionTrack.alternate_genreSpecified = false;

                // TODO asset.alternate_subgenre

                ingestionTrack.always_send_display_title = true;

                ingestionTrack.always_send_display_titleSpecified = true;

                ingestionTrack.available_separately = asset.AvailableSeparately;

                ingestionTrack.catalog_tier = _isrcTierConverter[(CatalogTier)asset.Tier];

                ingestionTrack.catalog_tierSpecified = true;

                ingestionTrack.classical_catalog = (parentWork == null) ? currentWork.ClassicalCatalog : parentWork.ClassicalCatalog;

                ingestionTrack.contributors = new List<contributor>(); // These represent work-related contributors like typically Composer, Arranger, etc.
                foreach (var workContributor in currentWork.Contributors)
                {
                    Artist artist;
                    if (artistsBuffer.Exists(a => a.Id.Equals(workContributor.Key)))
                    {
                        // Present in buffer
                        artist = artistsBuffer.FirstOrDefault(a => a.Id.Equals(workContributor.Key));
                    }
                    else
                    {
                        // Find then add to buffer
                        artist = CatalogContext.Instance.Artists.FirstOrDefault(a => a.Id.Equals(workContributor.Key));
                        artistsBuffer.Add(artist);
                    }
                    var role = CatalogContext.Instance.Roles.FirstOrDefault(r => String.Compare(r.Name, workContributor.Value, StringComparison.Ordinal) == 0);
                    var cRole = (_roleConverter.ContainsKey(role.Reference))
                        ? _roleConverter[role.Reference]
                        : contributorRole.ContributingArtist;

                    if (artist.LastName.ContainsKey(CatalogContext.Instance.DefaultLang.ShortName))
                    {
                        ingestionTrack.contributors.Add(new contributor
                        {
                            name = (artist.FirstName[CatalogContext.Instance.DefaultLang.ShortName].Typo() + " " + artist.LastName[CatalogContext.Instance.DefaultLang.ShortName].Typo()).Trim(),
                            role = cRole,
                        });
                    }
                }

                // TODO asset.country_of_commissioning
                // TODO asset.country_of_recording

                if  (
                        currentWork.Title.ContainsKey(CatalogContext.Instance.DefaultLang.ShortName)
                        && (
                                parentWork == null
                                || parentWork.Title.ContainsKey(CatalogContext.Instance.DefaultLang.ShortName)
                            )
                    )
                {
                    ingestionTrack.display_title = (parentWork == null)
                        ? currentWork.Title[CatalogContext.Instance.DefaultLang.ShortName].Typo()
                        : parentWork.Title[CatalogContext.Instance.DefaultLang.ShortName].Typo() + StringHelper.HierarchicalSeparator(CatalogContext.Instance.DefaultLang) + currentWork.Title[CatalogContext.Instance.DefaultLang.ShortName].Typo();
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

                ingestionTrack.isrc_code = assetId;

                ingestionTrack.keySpecified = (parentWork == null && currentWork.Tonality != null) || (parentWork != null && parentWork.Tonality != null);

                if (ingestionTrack.keySpecified)
                {
                    ingestionTrack.key = (parentWork == null)
                        ? _keyConverter[(Key)(currentWork.Tonality)]
                        : _keyConverter[(Key)(parentWork.Tonality)];
                }

                // TODO asset.lyrics

                if (album.Subgenre != null)
                {
                    ingestionTrack.main_subgenre = album.Subgenre.Name.Typo(); // TODO implement trackwise field
                }

                if  (
                        parentWork != null
                        && currentWork.MovementTitle.ContainsKey(CatalogContext.Instance.DefaultLang.ShortName)
                        && !String.IsNullOrEmpty(currentWork.MovementTitle[CatalogContext.Instance.DefaultLang.ShortName])
                    )
                {
                    ingestionTrack.movement = currentWork.MovementTitle[CatalogContext.Instance.DefaultLang.ShortName].Typo();
                }

                if (parentWork != null && currentWork.MovementNumber > 0)
                {
                    ingestionTrack.movement_number = currentWork.MovementNumber.ToString();
                }

                ingestionTrack.on_disc = volumeIndex.ToString(CultureInfo.InvariantCulture);

                ingestionTrack.p_line_text = asset.PName.Typo();

                ingestionTrack.p_line_year = asset.PYear.ToString();

                ingestionTrack.parental_advisory = parental_advisory.@false; // TODO add seting

                // TODO asset.preorder_type

                ingestionTrack.preorder_typeSpecified = false;

                ingestionTrack.preview_length = "30"; // TODO add setting

                ingestionTrack.preview_start = "0";

                // Primary artists is performer 1 (asset contributor 1)
                var primaryArtist = CatalogContext.Instance.Artists.FirstOrDefault(e =>
                    e.Id.Equals(assetPerformersKeys[0]));
                if  (
                        primaryArtist != null 
                        && primaryArtist.LastName != null 
                        && primaryArtist.LastName.ContainsKey(CatalogContext.Instance.DefaultLang.ShortName)
                    )
                {
                    ingestionTrack.primary_artist = new primary_artist
                    {
                        name = (primaryArtist.FirstName[CatalogContext.Instance.DefaultLang.ShortName].Typo() + " " + primaryArtist.LastName[CatalogContext.Instance.DefaultLang.ShortName].Typo()).Trim(),
                    };
                }

                // TODO asset.publishers

                ingestionTrack.recording_location = asset.RecordingLocation.Typo();

                if (asset.RecordingYear != null)
                {
                    ingestionTrack.recording_year = asset.RecordingYear.ToString();
                }

                // TODO asset.redeliveries_of_associated
                var audioFilename = SearchFilename(files, FugaIngestionFileType.AudioTrack, new KeyValuePair<int, int>(volumeIndex, trackIndex));
                if (!String.IsNullOrEmpty(audioFilename))
                {
                    var f = new FileInfo(audioFilename);
                    ingestionTrack.resources = new List<resourcesAudio>
                    {
                        new resourcesAudio
                        {
                            file = new file_type
                            {
                                name = audioFilename.GetFileNameFromFullPath(),
                                size = (ulong) f.Length,
                            }
                        },
                    };
                }

                // TODO asset.rights_contract_begin_date
                // TODO asset.rights_contract_begin_dateSpecified
                // TODO asset.rights_holder_name
                // TODO asset.rights_ownership_name

                ingestionTrack.sequence_number = trackIndex.ToString(CultureInfo.InvariantCulture);

                // TODO asset.track_notes
                // TODO asset.track_version
                // TODO asset.usage_rights

                if  (
                        parentWork != null 
                        && parentWork.Title != null 
                        && parentWork.Title.ContainsKey(CatalogContext.Instance.DefaultLang.ShortName)
                    )
                {
                    ingestionTrack.work = parentWork.Title[CatalogContext.Instance.DefaultLang.ShortName].Typo();
                }

                // Add asset
                i.album.tracks.Items.Add(ingestionTrack);
            }
        }
        #endregion

        public void Notify(String message)
        {
            if (_mainFormViewModel == null || _mainFormViewModel.MainFormDispatcher == null || String.IsNullOrEmpty(message))
            {
                Debug.Write("FugaXmlCatalogWriter.Notify, wrong view model or empty message");
                return;
            }
            var methodInvoker = new MethodInvoker(() =>
            {
                _mainFormViewModel.Notification = message;
            });

            _mainFormViewModel.MainFormDispatcher.Invoke(methodInvoker);
        }

        /// <summary>
        /// Returns the file name, if found, of the file corresponding to criteria
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static String SearchFilename(String[] files, FugaIngestionFileType fileType, object parameter = null)
        {
            if (files == null || files.Length == 0)
            {
                return String.Empty;
            }
            var filesList = files.ToList();

            switch (fileType)
            {
                case FugaIngestionFileType.Attachment:
                    foreach (var file in filesList)
                    {
                        var nameExtension = file.Split('.').Last().ToLower();
                        if  (
                                !String.IsNullOrEmpty(nameExtension) 
                                && String.Compare(nameExtension, "pdf", StringComparison.Ordinal) == 0
                            )
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
                        foreach (var file in filesList)
                        {
                            var nameExtension = file.Split('.').Last().ToLower();
                            if  (
                                    String.IsNullOrEmpty(nameExtension) 
                                    || 
                                    (
                                        String.Compare(nameExtension, "wav", StringComparison.Ordinal) != 0 
                                        && String.Compare(nameExtension, "aif", StringComparison.Ordinal) != 0 
                                        && String.Compare(nameExtension, "aiff", StringComparison.Ordinal) != 0
                                    )
                                )
                            {
                                continue;
                            }
                            var nameLeft = file.Substring(0, file.Length - nameExtension.Length - 1);
                            var nameElements = nameLeft.Split('-');
                            var nameElementsCount = nameElements.ToList().Count();
                            if (nameElementsCount < 3)
                            {
                                continue;
                            }
                            var tryVolumeIndex = 0;
                            var tryTrackIndex = 0;
                            var convertVolume = int.TryParse(nameElements[nameElementsCount - 2], out tryVolumeIndex);
                            var convertTrack = int.TryParse(nameElements[nameElementsCount - 1], out tryTrackIndex);
                            if  (
                                    convertVolume 
                                    && convertTrack
                                    && tryVolumeIndex.Equals(volumeIndex)
                                    && tryTrackIndex.Equals(trackIndex)
                                )
                            {
                                // Found !
                                return file;
                            }
                        }
                    }
                    break;

                case FugaIngestionFileType.Cover:
                    foreach (var file in filesList)
                    {
                        var nameExtension = file.Split('.').Last().ToLower();
                        if  (
                                !String.IsNullOrEmpty(nameExtension) 
                                &&
                                (
                                    String.Compare(nameExtension, "jpg", StringComparison.Ordinal) == 0 
                                    || String.Compare(nameExtension, "jpeg", StringComparison.Ordinal) == 0 
                                    || String.Compare(nameExtension, "png", StringComparison.Ordinal) == 0
                                )
                            )
                        {
                            return file;
                        }
                    }
                    break;
            }
            return String.Empty;
        }
    }
}
