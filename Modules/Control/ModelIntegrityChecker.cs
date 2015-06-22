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

using BabelMeta.Model;
using System.Linq;

namespace BabelMeta.Modules.Control
{
    /// <summary>
    /// Implementation of integrity checker for the core interchange format CatalogContext.
    /// </summary>
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
            if (CatalogContext.Instance.Langs != null && CatalogContext.Instance.Langs.Count > 0 && CatalogContext.Instance.Langs.GroupBy(e => e.ShortName).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Tags
            if (CatalogContext.Instance.Tags != null && CatalogContext.Instance.Tags.Count > 0 && CatalogContext.Instance.Tags.GroupBy(e => e.Name).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Roles
            if (CatalogContext.Instance.Roles != null && CatalogContext.Instance.Roles.Count > 0 && CatalogContext.Instance.Roles.GroupBy(e => e.Name).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Qualities
            if (CatalogContext.Instance.Qualities != null && CatalogContext.Instance.Qualities.Count > 0 && CatalogContext.Instance.Qualities.GroupBy(e => e.Name).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Artists
            if (CatalogContext.Instance.Artists != null && CatalogContext.Instance.Artists.Count > 0 && CatalogContext.Instance.Artists.GroupBy(e => e.Id).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Works
            if (CatalogContext.Instance.Works != null && CatalogContext.Instance.Works.Count > 0 && CatalogContext.Instance.Works.GroupBy(e => e.Id).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Isrcs
            if (CatalogContext.Instance.Isrcs != null && CatalogContext.Instance.Isrcs.Count > 0 && CatalogContext.Instance.Isrcs.GroupBy(e => e.Id).Max(g => g.Count()) > 1)
            {
                return false;
            }

            // Albums
            if  (
                    CatalogContext.Instance.Albums != null && CatalogContext.Instance.Albums.Count > 0 && 
                    (
                        CatalogContext.Instance.Albums.GroupBy(e => e.Id).Max(g => g.Count()) > 1
                        || CatalogContext.Instance.Albums.Where(e => e.Ean != null && e.Ean > 0).GroupBy(e => e.Ean).Max(g => g.Count()) > 1
                    )
                )
            {
                return false;
            }
            
            // Assets
            // Structurally not exposed to redundancy

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
                    CatalogContext.Instance.Works != null && CatalogContext.Instance.Works.Count > 0 &&
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
                    CatalogContext.Instance.Works != null && CatalogContext.Instance.Works.Count > 0 &&
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
                    CatalogContext.Instance.Works != null && CatalogContext.Instance.Works.Count > 0 &&
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
                    CatalogContext.Instance.Isrcs != null && CatalogContext.Instance.Isrcs.Count > 0 &&
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
                    CatalogContext.Instance.Isrcs != null && CatalogContext.Instance.Isrcs.Count > 0 &&
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
                    CatalogContext.Instance.Isrcs != null && CatalogContext.Instance.Isrcs.Count > 0 &&
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
                    CatalogContext.Instance.Isrcs != null && CatalogContext.Instance.Isrcs.Count > 0 &&
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
                    CatalogContext.Instance.Albums != null && CatalogContext.Instance.Albums.Count > 0 &&
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
