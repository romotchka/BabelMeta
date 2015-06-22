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
    /// Isrc class represents the Isrc unique Id, per se, and the related information (Work and work Performers).
    /// Having a dedicated Isrc object permits to re-use it in different Albums (compilations, bundles, licensing...)
    /// </summary>
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
