/*
 * Babel Meta
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

            // Work -> Artist
            if  (
                    CatalogContext.Instance.Works.Exists(w => 
                        w.Contributors.Keys.ToList().Exists(c => 
                            !CatalogContext.Instance.Artists.Exists(e => 
                                e.Id == c
                            )
                        )
                    )
                )
            {
                return false;
            }

            // Work -> Role
            if (
                    CatalogContext.Instance.Works.Exists(w =>
                        w.Contributors.Values.ToList().Exists(r =>
                            !CatalogContext.Instance.Roles.Exists(e =>
                                e.Name.CompareTo(r.Name) == 0
                            )
                        )
                    )
                )
            {
                return false;
            }

            // Work -> Work
            if (
                    CatalogContext.Instance.Works.Where(w => w.Parent > 0).ToList().Exists(w =>
                        !CatalogContext.Instance.Works.Exists(e =>
                            e.Id == w.Parent
                        )
                    )
                )
            {
                return false;
            }

            // Isrc -> Work
            if (
                    CatalogContext.Instance.Isrcs.Exists(i =>
                        !CatalogContext.Instance.Works.Exists(e =>
                            e.Id == i.Work
                        )
                    )
                )
            {
                return false;
            }

            // Isrc -> Contributor
            if (
                    CatalogContext.Instance.Isrcs.Exists(i =>
                        i.Contributors.Keys.ToList().Exists(c =>
                            !CatalogContext.Instance.Artists.Exists(e =>
                                e.Id == c
                            )
                        )
                    )
                )
            {
                return false;
            }

            // Isrc -> Role
            if (
                    CatalogContext.Instance.Isrcs.Exists(i =>
                        i.Contributors.Values.ToList().Exists(rq =>
                                rq.Keys.ToList().Exists(r =>
                                    !CatalogContext.Instance.Roles.Exists(e =>
                                        e.Name.CompareTo(r.Name) == 0                                
                                )
                            )
                        )
                    )
                )
            {
                return false;
            }

            // Isrc -> Quality
            if (
                    CatalogContext.Instance.Isrcs.Exists(i =>
                        i.Contributors.Values.ToList().Exists(rq =>
                                rq.Values.ToList().Exists(q =>
                                    !CatalogContext.Instance.Qualities.Exists(e =>
                                        e.Name.CompareTo(q.Name) == 0
                                )
                            )
                        )
                    )
                )
            {
                return false;
            }

            // Album -> Isrc
            if (
                    CatalogContext.Instance.Albums.Exists(a =>
                        a.Assets.Values.ToList().Exists(t =>
                            t.Values.ToList().Exists(i =>
                                !CatalogContext.Instance.Isrcs.Exists(e =>
                                    e.Id.CompareTo(i) == 0
                                )
                            )
                        )
                    )
                )
            {
                return false;
            }

            // Default lang
            if (CatalogContext.Instance.DefaultLang == null)
            {
                return false;
            }

            return true;
        }
    }
}
