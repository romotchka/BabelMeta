/*
 * Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataConverter.Model
{
    /// <summary>
    /// Singleton object representing the whole catalog in interchange format between Input/Output
    /// </summary>
    public class CatalogContext
    {
        private static CatalogContext _instance;

        private CatalogContext() 
        {
        }

        public static CatalogContext Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new CatalogContext();

                    _instance.Langs = new List<Lang>();
                    _instance.Roles = new List<Role>();
                    _instance.Qualities = new List<Quality>();
                    _instance.Tags = new List<Tag>();
                    _instance.Artists = new List<Artist>();
                    _instance.Works = new List<Work>();
                    _instance.Isrcs = new List<Isrc>();
                    _instance.Albums = new List<Album>();
                }
                return _instance;
            }
        }

        internal bool Initialized = false;

        internal bool RedundantKeysChecked = false;

        internal bool ReferentialIntegrityChecked = false;

        internal List<Lang> Langs;

        internal List<Role> Roles;

        internal List<Quality> Qualities;

        internal List<Tag> Tags;

        internal List<Artist> Artists;

        internal List<Work> Works;

        internal List<Isrc> Isrcs;

        internal List<Album> Albums;

        public void Init()
        {
            _instance.Langs.Clear();
            _instance.Roles.Clear();
            _instance.Qualities.Clear();
            _instance.Tags.Clear();
            _instance.Artists.Clear();
            _instance.Works.Clear();
            _instance.Isrcs.Clear();
            _instance.Albums.Clear();
        }
    }
}
