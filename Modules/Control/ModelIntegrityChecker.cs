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
using System;
using System.Diagnostics;
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
            get { return _instance ?? (_instance = new ModelIntgrityChecker()); }
        }

        public bool CheckRedundantKeys()
        {
            if (CatalogContext.Instance == null || CatalogContext.Instance.Initialized == false)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckRedundantKeys, Instance null or not initialized");
                return false;
            }

            // Langs
            if (CatalogContext.Instance.Langs != null && CatalogContext.Instance.Langs.Count > 0 && CatalogContext.Instance.Langs.GroupBy(e => e.ShortName).Max(g => g.Count()) > 1)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckRedundantKeys, Lang");
                return false;
            }

            // Tags
            if (CatalogContext.Instance.Tags != null && CatalogContext.Instance.Tags.Count > 0 && CatalogContext.Instance.Tags.GroupBy(e => e.Name).Max(g => g.Count()) > 1)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckRedundantKeys, Tags");
                return false;
            }

            // Roles
            if (CatalogContext.Instance.Roles != null && CatalogContext.Instance.Roles.Count > 0 && CatalogContext.Instance.Roles.GroupBy(e => e.Name).Max(g => g.Count()) > 1)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckRedundantKeys, Roles");
                return false;
            }

            // Qualities
            if (CatalogContext.Instance.Qualities != null && CatalogContext.Instance.Qualities.Count > 0 && CatalogContext.Instance.Qualities.GroupBy(e => e.Name).Max(g => g.Count()) > 1)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckRedundantKeys, Qualities");
                return false;
            }

            // Artists
            if (CatalogContext.Instance.Artists != null && CatalogContext.Instance.Artists.Count > 0 && CatalogContext.Instance.Artists.GroupBy(e => e.Id).Max(g => g.Count()) > 1)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckRedundantKeys, Artists");
                return false;
            }

            // Works
            if (CatalogContext.Instance.Works != null && CatalogContext.Instance.Works.Count > 0 && CatalogContext.Instance.Works.GroupBy(e => e.Id).Max(g => g.Count()) > 1)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckRedundantKeys, Works");
                return false;
            }

            // Assets
            if (CatalogContext.Instance.Assets != null && CatalogContext.Instance.Assets.Count > 0 && CatalogContext.Instance.Assets.GroupBy(e => e.Id).Max(g => g.Count()) > 1)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckRedundantKeys, Assets");
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
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckRedundantKeys, Albums");
                return false;
            }
            
            // Tracks
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
            try
            {
                Work work;
                if (
                        CatalogContext.Instance.Works != null
                        && CatalogContext.Instance.Works.Count > 0
                        &&
                        (
                            work = CatalogContext.Instance.Works.FirstOrDefault(w =>
                                w.Contributors.Keys.ToList().Exists(c =>
                                    !CatalogContext.Instance.Artists.Exists(e =>
                                        e.Id == c
                                    )
                                )
                            )
                        ) != null
                    )
                {
                    Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Work->Artist, corrupted work = " + work.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Work->Artist, exception=" + ex.Message);
                return false;
            }

            // Work -> Role
            try
            {
                Work work;
                if (
                        CatalogContext.Instance.Works != null
                        && CatalogContext.Instance.Works.Count > 0
                        && 
                        (
                            work = CatalogContext.Instance.Works.FirstOrDefault(w =>
                                w.Contributors.Values.ToList().Exists(r =>
                                    !CatalogContext.Instance.Roles.Exists(e =>
                                        String.Compare(e.Name, r, StringComparison.Ordinal) == 0
                                    )
                                )
                            )
                        ) != null
                    )
                {
                    Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Work->Role, corrupted work = " + work.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Work->Role, exception=" + ex.Message);
                return false;
            }

            // Work -> Work, orphan
            try
            {
                Work work;
                if (
                        CatalogContext.Instance.Works != null
                        && CatalogContext.Instance.Works.Count > 0
                        &&
                        (
                            work = CatalogContext.Instance.Works.Where(w => w.Parent > 0).FirstOrDefault(w =>
                                !CatalogContext.Instance.Works.Exists(e =>
                                    e.Id == w.Parent
                                )
                            )
                        ) != null
                    )
                {
                    Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Work->Work, orphan, corrupted work = " + work.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Work->Work, exception=" + ex.Message);
                return false;
            }

            // Work -> Work, loop
            try
            {
                Work work;
                if (
                        CatalogContext.Instance.Works != null
                        && CatalogContext.Instance.Works.Count > 0
                        &&
                        (
                            work = CatalogContext.Instance.Works.Where(w => w.Parent > 0).FirstOrDefault(w => w.Id == w.Parent)
                        ) != null
                    )
                {
                    Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Work->Work, loop, corrupted work = " + work.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Work->Work, exception=" + ex.Message);
                return false;
            }

            // Asset -> Work
            try
            {
                Asset asset;
                if (
                        CatalogContext.Instance.Assets != null
                        && CatalogContext.Instance.Assets.Count > 0
                        &&
                        (
                            asset = CatalogContext.Instance.Assets.FirstOrDefault(i =>
                                !CatalogContext.Instance.Works.Exists(e =>
                                    e.Id == i.Work
                                )
                            )
                        ) != null
                    )
                {
                    Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Asset->Work, corrupted asset = " + asset.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Asset->Work, exception=" + ex.Message);
                return false;
            }

            // Asset -> Contributor
            try
            {
                Asset asset;
                if (
                        CatalogContext.Instance.Assets != null
                        && CatalogContext.Instance.Assets.Count > 0
                        && 
                        (
                            asset = CatalogContext.Instance.Assets.FirstOrDefault(i =>
                                i.Contributors.Keys.ToList().Exists(c =>
                                    !CatalogContext.Instance.Artists.Exists(e =>
                                        e.Id == c
                                    )
                                )
                            )
                        ) != null
                    )
                {
                    Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Asset->Contributor, corrupted asset = " + asset.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Asset->Contributor, exception=" + ex.Message);
                return false;
            }

            // Asset -> Role
            try
            {
                Asset asset;
                if (
                        CatalogContext.Instance.Assets != null
                        && CatalogContext.Instance.Assets.Count > 0
                        && 
                        (
                            asset = CatalogContext.Instance.Assets.FirstOrDefault(i =>
                                i.Contributors.Values.ToList().Exists(rq =>
                                        rq.Keys.ToList().Exists(r =>
                                            !CatalogContext.Instance.Roles.Exists(e =>
                                                String.Compare(e.Name, r, StringComparison.Ordinal) == 0
                                        )
                                    )
                                )
                            )
                        ) != null
                    )
                {
                    Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Asset->Role, corrupted asset = " + asset.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Asset->Role, exception=" + ex.Message);
                return false;
            }

            // Asset -> Quality
            try
            {
                Asset asset;
                if (
                        CatalogContext.Instance.Assets != null
                        && CatalogContext.Instance.Assets.Count > 0
                        &&
                        (
                            asset = CatalogContext.Instance.Assets.FirstOrDefault(i =>
                                i.Contributors.Values.ToList().Exists(rq =>
                                        rq.Values.ToList().Exists(q =>
                                            !CatalogContext.Instance.Qualities.Exists(e =>
                                                String.Compare(e.Name, q, StringComparison.Ordinal) == 0
                                        )
                                    )
                                )
                            )
                        ) != null
                    )
                {
                    Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Asset->Quality, corrupted asset = " + asset.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Asset->Quality, exception=" + ex.Message);
                return false;
            }

            // Album -> Asset
            try
            {
                Album album;
                if (
                        CatalogContext.Instance.Albums != null
                        && CatalogContext.Instance.Albums.Count > 0
                        && 
                        (
                            album = CatalogContext.Instance.Albums.FirstOrDefault(a =>
                                a.Tracks.Values.ToList().Exists(t =>
                                    t.Values.ToList().Exists(i =>
                                        !CatalogContext.Instance.Assets.Exists(e =>
                                            String.Compare(e.Id, i, StringComparison.Ordinal) == 0
                                        )
                                    )
                                )
                            )
                        ) != null
                    )
                {
                    Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Album->Asset, corrupted album = " + album.CatalogReference);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, "ModelIntegrityChecker.CheckReferentialIntegrity, Album->Asset, exception=" + ex.Message);
                return false;
            }

            // Default lang
            return CatalogContext.Instance.DefaultLang != null;
        }
    }
}
