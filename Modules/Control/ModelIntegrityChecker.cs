/*
 * Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using MetadataConverter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataConverter.Modules.Control
{
    public class ModelIntgrityChecker : IIntegrityChecker
    {

        private static ModelIntgrityChecker _instance;

        private ModelIntgrityChecker()
        {

        }

        public static ModelIntgrityChecker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ModelIntgrityChecker();

                }
                return _instance;
            }
        }

        public bool CheckRedundantKeys()
        {
            if (CatalogContext.Instance == null || CatalogContext.Instance.Initialized == false)
            {
                return false;
            }

            // Langs
            if (CatalogContext.Instance.Langs.GroupBy(e => e.ShortName).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Tags
            if (CatalogContext.Instance.Tags.GroupBy(e => e.Name).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Roles
            if (CatalogContext.Instance.Roles.GroupBy(e => e.Name).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Qualities
            if (CatalogContext.Instance.Qualities.GroupBy(e => e.Name).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Artists
            if (CatalogContext.Instance.Artists.GroupBy(e => e.Id).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Works
            if (CatalogContext.Instance.Works.GroupBy(e => e.Id).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Isrcs
            if (CatalogContext.Instance.Isrcs.GroupBy(e => e.Id).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Albums
            if  (
                    CatalogContext.Instance.Albums.GroupBy(e => e.Id).Max(g => g.Count()) > 1
                    || CatalogContext.Instance.Albums.Where(e => e.Ean != null && e.Ean > 0).GroupBy(e => e.Ean).Max(g => g.Count()) > 1
                )
            {
                return false;
            }
            
            // Assets
            // Structurally not eligible to redundancy

            return true;
        }

        public bool CheckReferentialIntegrity()
        {
            if (CatalogContext.Instance == null || CatalogContext.Instance.Initialized == false)
            {
                return false;
            }

            // Works -> Artists
            if  (
                    CatalogContext.Instance.Works.Exists(w => 
                        w.Contributors.Keys.ToList().Exists(c => 
                            !CatalogContext.Instance.Artists.Exists(a => 
                                a.Id == c
                            )
                        )
                    )
                )
            {
                return false;
            }

            return true;
        }
    }
}
