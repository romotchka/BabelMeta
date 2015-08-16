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

using BabelMeta.Model.Config;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BabelMeta.Model
{
    /// <summary>
    /// Core model Album.
    /// </summary>
    [Serializable()]
    public class Album : ISerializable
    {
        public Album()
        {
            Id = 0;
            ActionTypeValue = ActionType.Insert;
            CName = String.Empty;
            CYear = null;
            PName = String.Empty;
            PYear = null;
            Tier = CatalogTier.Front;
            ConsumerReleaseDate = DateTime.Now;
            OriginalReleaseDate = null;
            Ean = null;
            Title = null;
            Genre = null;
            Subgenre = null;
            Owner = String.Empty;
            CatalogReference = String.Empty;
            RecordingLocation = String.Empty;
            RecordingYear = null;
            Redeliver = false;
        }

        public Album(SerializationInfo info, StreamingContext ctxt)
        {
            Id = (int)info.GetValue("BabelMeta.Model.Album.Id", typeof(int));
            ActionTypeValue = (ActionType?)info.GetValue("BabelMeta.Model.Album.ActionTypeValue", typeof(ActionType?));
            CName = (String)info.GetValue("BabelMeta.Model.Album.CName", typeof(String));
            CYear = (short?)info.GetValue("BabelMeta.Model.Album.CYear", typeof(short?));
            PName = (String)info.GetValue("BabelMeta.Model.Album.PName", typeof(String));
            PYear = (short?)info.GetValue("BabelMeta.Model.Album.PYear", typeof(short?));
            Tier = (CatalogTier)info.GetValue("BabelMeta.Model.Album.Tier", typeof(CatalogTier));
            ConsumerReleaseDate = (DateTime)info.GetValue("BabelMeta.Model.Album.ConsumerReleaseDate", typeof(DateTime));
            OriginalReleaseDate = (DateTime?)info.GetValue("BabelMeta.Model.Album.OriginalReleaseDate", typeof(DateTime?));
            Ean = (long?)info.GetValue("BabelMeta.Model.Album.Ean", typeof(long?));
            Title = (Dictionary<String, String>)info.GetValue("BabelMeta.Model.Album.Title", typeof(Dictionary<String, String>));
            Genre = (Tag)info.GetValue("BabelMeta.Model.Album.Genre", typeof(Tag));
            Subgenre = (Tag)info.GetValue("BabelMeta.Model.Album.Subgenre", typeof(Tag));
            Owner = (String)info.GetValue("BabelMeta.Model.Album.Owner", typeof(String));
            CatalogReference = (String)info.GetValue("BabelMeta.Model.Album.CatalogReference", typeof(String));
            RecordingLocation = (String)info.GetValue("BabelMeta.Model.Album.RecordingLocation", typeof(String));
            RecordingYear = (short?)info.GetValue("BabelMeta.Model.Album.RecordingYear", typeof(short?));
            Redeliver = (bool)info.GetValue("BabelMeta.Model.Album.Redeliver", typeof(bool));
            Tracks = (Dictionary<short, Dictionary<short, String>>)info.GetValue("BabelMeta.Model.Album.Tracks", typeof(Dictionary<short, Dictionary<short, String>>));
            PrimaryArtistId = (int?)info.GetValue("BabelMeta.Model.Album.PrimaryArtistId", typeof(int?));
            TotalDiscs = (short?)info.GetValue("BabelMeta.Model.Album.TotalDiscs", typeof(short?));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Album.Id", Id);
            info.AddValue("BabelMeta.Model.Album.ActionTypeValue", ActionTypeValue);
            info.AddValue("BabelMeta.Model.Album.CName", CName);
            info.AddValue("BabelMeta.Model.Album.CYear", CYear);
            info.AddValue("BabelMeta.Model.Album.PName", PName);
            info.AddValue("BabelMeta.Model.Album.PYear", PYear);
            info.AddValue("BabelMeta.Model.Album.Tier", Tier);
            info.AddValue("BabelMeta.Model.Album.ConsumerReleaseDate", ConsumerReleaseDate);
            info.AddValue("BabelMeta.Model.Album.OriginalReleaseDate", OriginalReleaseDate);
            info.AddValue("BabelMeta.Model.Album.Ean", Ean);
            info.AddValue("BabelMeta.Model.Album.Title", Title);
            info.AddValue("BabelMeta.Model.Album.Genre", Genre);
            info.AddValue("BabelMeta.Model.Album.Subgenre", Subgenre);
            info.AddValue("BabelMeta.Model.Album.Owner", Owner);
            info.AddValue("BabelMeta.Model.Album.CatalogReference", CatalogReference);
            info.AddValue("BabelMeta.Model.Album.RecordingLocation", RecordingLocation);
            info.AddValue("BabelMeta.Model.Album.RecordingYear", RecordingYear);
            info.AddValue("BabelMeta.Model.Album.Redeliver", Redeliver);
            info.AddValue("BabelMeta.Model.Album.Tracks", Tracks);
            info.AddValue("BabelMeta.Model.Album.PrimaryArtistId", PrimaryArtistId);
            info.AddValue("BabelMeta.Model.Album.TotalDiscs", TotalDiscs);
        }

        /// <summary>
        /// Album internal Id.
        /// </summary>
        public int Id { get; set; }

        public ActionType? ActionTypeValue { get; set; }

        public String CName { get; set; }

        public short? CYear { get; set; }

        public String PName { get; set; }

        public short? PYear { get; set; }

        public CatalogTier Tier { get; set; }

        public DateTime ConsumerReleaseDate { get; set; }

        public DateTime? OriginalReleaseDate { get; set; }

        /// <summary>
        /// UPC/EAN
        /// </summary>
        public long? Ean { get; set; }

        /// <summary>
        /// The field name is singular because it represents the same entry in different languages available
        /// </summary>
        public Dictionary<String, String> Title { get; set; }

        public Tag Genre { get; set; }

        public Tag Subgenre { get; set; }

        public String Owner { get; set; }

        public String CatalogReference { get; set; }

        public String RecordingLocation { get; set; }

        public short? RecordingYear { get; set; }

        public bool Redeliver { get; set; }

        /// <summary>
        /// Nested Dictionaries correspond to Volume index, then Track index.
        /// The String value is the Isrc Id.
        /// E.g. in a 2-volume album, Tracks[2][5] would represent Track 5 of Volume 2.
        /// The value itself is the Isrc key of the corresponding asset.
        /// </summary>
        public Dictionary<short, Dictionary<short, String>> Tracks { get; set; }

        // Deduced field (from number of occurrences throughout Album)
        public int? PrimaryArtistId { get; set; }

        // Deduced field (from Tracks)
        public short? TotalDiscs { get; set; }

        public enum ActionType
        {
            Insert,
            Update,
        } 
    }
}
