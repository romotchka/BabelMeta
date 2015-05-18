/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using BabelMeta.Model.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BabelMeta.Model
{
    [Serializable()]
    public class Album : ISerializable
    {
        public Album()
        {
            Id = 0;
            CName = string.Empty;
            CYear = null;
            PName = string.Empty;
            PYear = null;
            Tier = CatalogTier.Front;
            ConsumerReleaseDate = DateTime.Now;
            OriginalReleaseDate = null;
            Ean = null;
            Title = null;
            Genre = null;
            Subgenre = null;
            Owner = string.Empty;
            CatalogReference = string.Empty;
            RecordingLocation = string.Empty;
            RecordingYear = null;
            Redeliver = false;
        }

        public Album(SerializationInfo info, StreamingContext ctxt)
        {
            Id = (Int32)info.GetValue("BabelMeta.Model.Album.Id", typeof(Int32));
            CName = (string)info.GetValue("BabelMeta.Model.Album.CName", typeof(string));
            CYear = (Int16?)info.GetValue("BabelMeta.Model.Album.CYear", typeof(Int16?));
            PName = (string)info.GetValue("BabelMeta.Model.Album.PName", typeof(string));
            PYear = (Int16?)info.GetValue("BabelMeta.Model.Album.PYear", typeof(Int16?));
            Tier = (CatalogTier)info.GetValue("BabelMeta.Model.Album.Tier", typeof(CatalogTier));
            ConsumerReleaseDate = (DateTime)info.GetValue("BabelMeta.Model.Album.ConsumerReleaseDate", typeof(DateTime));
            OriginalReleaseDate = (DateTime?)info.GetValue("BabelMeta.Model.Album.OriginalReleaseDate", typeof(DateTime?));
            Ean = (Int64?)info.GetValue("BabelMeta.Model.Album.Ean", typeof(Int64?));
            Title = (Dictionary<string, string>)info.GetValue("BabelMeta.Model.Album.Title", typeof(Dictionary<string, string>));
            Genre = (Tag)info.GetValue("BabelMeta.Model.Album.Genre", typeof(Tag));
            Subgenre = (Tag)info.GetValue("BabelMeta.Model.Album.Subgenre", typeof(Tag));
            Owner = (string)info.GetValue("BabelMeta.Model.Album.Owner", typeof(string));
            CatalogReference = (string)info.GetValue("BabelMeta.Model.Album.CatalogReference", typeof(string));
            RecordingLocation = (string)info.GetValue("BabelMeta.Model.Album.RecordingLocation", typeof(string));
            RecordingYear = (Int16?)info.GetValue("BabelMeta.Model.Album.RecordingYear", typeof(Int16?));
            Redeliver = (bool)info.GetValue("BabelMeta.Model.Album.Redeliver", typeof(bool));
            Assets = (Dictionary<Int16, Dictionary<Int16, string>>)info.GetValue("BabelMeta.Model.Album.Assets", typeof(Dictionary<Int16, Dictionary<Int16, string>>));
            PrimaryArtistId = (Int32?)info.GetValue("BabelMeta.Model.Album.PrimaryArtistId", typeof(Int32?));
            TotalDiscs = (Int16?)info.GetValue("BabelMeta.Model.Album.TotalDiscs", typeof(Int16?));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Album.Id", Id);
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
            info.AddValue("BabelMeta.Model.Album.Assets", Assets);
            info.AddValue("BabelMeta.Model.Album.PrimaryArtistId", PrimaryArtistId);
            info.AddValue("BabelMeta.Model.Album.TotalDiscs", TotalDiscs);
        }

        public Int32 Id { get; set; }

        public string CName { get; set; }

        public Int16? CYear { get; set; }

        public string PName { get; set; }

        public Int16? PYear { get; set; }

        public CatalogTier Tier { get; set; }

        public DateTime ConsumerReleaseDate { get; set; }

        public DateTime? OriginalReleaseDate { get; set; }

        public Int64? Ean { get; set; }

        /// <summary>
        /// The field name is singular because it represents the same entry in different languages available
        /// </summary>
        public Dictionary<string, string> Title { get; set; }

        public Tag Genre { get; set; }

        public Tag Subgenre { get; set; }

        public string Owner { get; set; }

        public string CatalogReference { get; set; }

        public string RecordingLocation { get; set; }

        public Int16? RecordingYear { get; set; }

        public bool Redeliver { get; set; }

        /// <summary>
        /// Nested Dictionaries correspond to Volume index & Track index.
        /// The string value is an Isrc id.
        /// </summary>
        public Dictionary<Int16, Dictionary<Int16, string>> Assets { get; set; }

        // Deduced field
        public Int32? PrimaryArtistId { get; set; }

        // Deduced field
        public Int16? TotalDiscs { get; set; }
    }
}
