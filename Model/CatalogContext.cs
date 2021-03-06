﻿/*
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

using BabelMeta.Model.Config;
using System.Collections.Generic;
using System.Linq;

namespace BabelMeta.Model
{
    /// <summary>
    /// Singleton object representing the whole catalog in interchange format (internal).
    /// This singleton should embark enough complexity to deal with any input or output metadata.
    /// </summary>
    public class CatalogContext
    {
        private static CatalogContext _instance;

        private CatalogContext()
        {
            ReferentialIntegrityChecked = false;
            RedundantKeysChecked = false;
        }

        public static CatalogContext Instance
        {
            get 
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = new CatalogContext
                {
                    Settings = new CatalogSettings(),
                    Langs = new List<Lang>(),
                    Roles = new List<Role>(),
                    Qualities = new List<Quality>(),
                    Tags = new List<Tag>(),
                    Artists = new List<Artist>(),
                    Works = new List<Work>(),
                    Assets = new List<Asset>(),
                    Albums = new List<Album>(),
                };

                return _instance;
            }
        }

        internal bool Initialized = false;

        /// <summary>
        /// Determines whether redundant keys checking is active.
        /// </summary>
        internal bool RedundantKeysChecked { get; set; }

        /// <summary>
        /// Determines whether referential integrity checking between linked entities (e.g. Artists/Works) is active.
        /// </summary>
        internal bool ReferentialIntegrityChecked { get; set; }

        public bool IntegrityChecked
        {
            get
            {
                return RedundantKeysChecked && ReferentialIntegrityChecked;
            }
        }

        /// <summary>
        /// Default language for output formats that request only 1 language.
        /// </summary>
        public Lang DefaultLang
        {
            get
            {
                if (Langs == null)
                {
                    return null;
                }
                return (Langs.Exists(l => l.IsDefault))
                    ? Langs.FirstOrDefault(l => l.IsDefault)
                    : null;
            }
        }

        public CatalogSettings Settings;

        public List<Lang> Langs;

        public List<Role> Roles;

        public List<Quality> Qualities;

        public List<Tag> Tags;

        public List<Artist> Artists;

        public List<Work> Works;

        public List<Asset> Assets;

        public List<Album> Albums;

        public void Init()
        {
            _instance.Settings = null;
            _instance.Langs.Clear();
            _instance.Roles.Clear();
            _instance.Qualities.Clear();
            _instance.Tags.Clear();
            _instance.Artists.Clear();
            _instance.Works.Clear();
            _instance.Assets.Clear();
            _instance.Albums.Clear();
        }

        /// <summary>
        /// Removes from Catalog the Artists not present in any album (and their works)
        /// </summary>
        public void FilterUnusedArtists()
        {
            if (Albums == null)
            {
                return;
            }

            Artists.RemoveAll(art =>
                !Albums.Exists(alb =>
                    alb.Tracks.Values.ToList().Exists(vol =>
                        vol.Values.ToList().Exists(ik =>
                            Assets.Exists(asset =>
                                (
                                    System.String.Compare(asset.Id, ik, System.StringComparison.Ordinal) == 0
                                    && Works.Exists(w =>
                                        w.Id == asset.Work
                                        && w.Contributors.Keys.ToList().Contains(art.Id)
                                    )
                                )
                            )
                        )
                    )
                )
            );

            FilterUnusedWorks();
        }

        /// <summary>
        /// Removes from Catalog Works not present in any album (and their sub-works)
        /// </summary>
        public void FilterUnusedWorks()
        {
            if (Albums == null)
            {
                return;
            }

            Works.RemoveAll(w =>
                !Albums.Exists(alb =>
                    alb.Tracks.Values.ToList().Exists(vol =>
                        vol.Values.ToList().Exists(ik =>
                            Assets.Exists(asset =>
                                System.String.Compare(asset.Id, ik, System.StringComparison.Ordinal) == 0
                                && asset.Work == w.Id
                            )
                        )
                    )
                )
            );
        }
    }
}
