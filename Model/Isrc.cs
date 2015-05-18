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
    public class Isrc : ISerializable
    {
        public Isrc()
        {
            Id = string.Empty;
            Work = 0;
            Contributors = null;
            CName = string.Empty;
            CYear = null;
            PName = string.Empty;
            PYear = null;
            RecordingLocation = string.Empty;
            RecordingYear = null;
            AvailableSeparately = true;
            Tier = null;
        }

        public Isrc(SerializationInfo info, StreamingContext ctxt)
        {
            Id = (string)info.GetValue("BabelMeta.Model.Isrc.Id", typeof(string));
            Work = (Int32)info.GetValue("BabelMeta.Model.Isrc.Work", typeof(Int32));
            Contributors = (Dictionary<Int32, Dictionary<Role, Quality>>)info.GetValue("BabelMeta.Model.Isrc.Contributors", typeof(Dictionary<Int32, Dictionary<Role, Quality>>));
            CName = (string)info.GetValue("BabelMeta.Model.Isrc.CName", typeof(string));
            CYear = (Int16?)info.GetValue("BabelMeta.Model.Isrc.CYear", typeof(Int16?));
            PName = (string)info.GetValue("BabelMeta.Model.Isrc.PName", typeof(string));
            PYear = (Int16?)info.GetValue("BabelMeta.Model.Isrc.PYear", typeof(Int16?));
            RecordingLocation = (string)info.GetValue("BabelMeta.Model.Isrc.RecordingLocation", typeof(string));
            RecordingYear = (Int16?)info.GetValue("BabelMeta.Model.Isrc.RecordingYear", typeof(Int16?));
            AvailableSeparately = (bool)info.GetValue("BabelMeta.Model.Isrc.AvailableSeparately", typeof(bool));
            Tier = (CatalogTier?)info.GetValue("BabelMeta.Model.Isrc.Tier", typeof(CatalogTier?));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Isrc.Id", Id);
            info.AddValue("BabelMeta.Model.Isrc.Work", Work);
            info.AddValue("BabelMeta.Model.Isrc.Contributors", Contributors);
            info.AddValue("BabelMeta.Model.Isrc.CName", CName);
            info.AddValue("BabelMeta.Model.Isrc.CYear", CYear);
            info.AddValue("BabelMeta.Model.Isrc.PName", PName);
            info.AddValue("BabelMeta.Model.Isrc.PYear", PYear);
            info.AddValue("BabelMeta.Model.Isrc.RecordingLocation", RecordingLocation);
            info.AddValue("BabelMeta.Model.Isrc.RecordingYear", RecordingYear);
            info.AddValue("BabelMeta.Model.Isrc.AvailableSeparately", AvailableSeparately);
            info.AddValue("BabelMeta.Model.Isrc.Tier", Tier);
        }

        public string Id { get; set; }

        public Int32 Work { get; set; }

        /// <summary>
        /// Isrc Contributor is e.g. the Performer or Musical Director or Sound Engineer
        /// </summary>
        public Dictionary<Int32, Dictionary<Role, Quality>> Contributors { get; set; }

        public string CName { get; set; }

        public Int16? CYear { get; set; }

        public string PName { get; set; }

        public Int16? PYear { get; set; }

        public string RecordingLocation { get; set; }

        public Int16? RecordingYear { get; set; }

        public bool AvailableSeparately { get; set; }

        public CatalogTier? Tier { get; set; }
    }
}
