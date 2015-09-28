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
using BabelMeta.Services.DbDriver;

namespace BabelMeta.Model
{
    /// <summary>
    /// Asset class represents the Isrc unique Id, per se, and the related information (Work and work Performers).
    /// Having a dedicated Isrc object permits to re-use it in different Albums (compilations, bundles, licensing...)
    /// </summary>
    [Serializable()]
    public class Asset : ISerializable
    {
        public Asset()
        {
            Id = String.Empty;
            Work = 0;
            Contributors = null;
            CName = String.Empty;
            CYear = null;
            PName = String.Empty;
            PYear = null;
            RecordingLocation = String.Empty;
            RecordingYear = null;
            AvailableSeparately = true;
            Tier = null;
        }

        public Asset(SerializationInfo info, StreamingContext context)
        {
            Id = (String)info.GetValue("BabelMeta.Model.Asset.Id", typeof(String));
            Work = (int)info.GetValue("BabelMeta.Model.Asset.Work", typeof(int));
            Contributors = (Dictionary<int, Dictionary<String, String>>)info.GetValue("BabelMeta.Model.Asset.Contributors", typeof(Dictionary<int, Dictionary<String, String>>));
            CName = (String)info.GetValue("BabelMeta.Model.Asset.CName", typeof(String));
            CYear = (short?)info.GetValue("BabelMeta.Model.Asset.CYear", typeof(short?));
            PName = (String)info.GetValue("BabelMeta.Model.Asset.PName", typeof(String));
            PYear = (short?)info.GetValue("BabelMeta.Model.Asset.PYear", typeof(short?));
            RecordingLocation = (String)info.GetValue("BabelMeta.Model.Asset.RecordingLocation", typeof(String));
            RecordingYear = (short?)info.GetValue("BabelMeta.Model.Asset.RecordingYear", typeof(short?));
            AvailableSeparately = (bool)info.GetValue("BabelMeta.Model.Asset.AvailableSeparately", typeof(bool));
            Tier = (CatalogTier?)info.GetValue("BabelMeta.Model.Asset.Tier", typeof(CatalogTier?));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Asset.Id", Id);
            info.AddValue("BabelMeta.Model.Asset.Work", Work);
            info.AddValue("BabelMeta.Model.Asset.Contributors", Contributors);
            info.AddValue("BabelMeta.Model.Asset.CName", CName);
            info.AddValue("BabelMeta.Model.Asset.CYear", CYear);
            info.AddValue("BabelMeta.Model.Asset.PName", PName);
            info.AddValue("BabelMeta.Model.Asset.PYear", PYear);
            info.AddValue("BabelMeta.Model.Asset.RecordingLocation", RecordingLocation);
            info.AddValue("BabelMeta.Model.Asset.RecordingYear", RecordingYear);
            info.AddValue("BabelMeta.Model.Asset.AvailableSeparately", AvailableSeparately);
            info.AddValue("BabelMeta.Model.Asset.Tier", Tier);
        }

        [DbField(MaxSize = 128)]
        public String Id { get; set; }

        public int Work { get; set; }

        /// <summary>
        /// Asset Contributor is e.g. the Performer or Musical Director or Sound Engineer
        /// The String key in the nested dictionary is a Role.Name
        /// The String field in the nested dictionary represents a Quality.Name
        /// </summary>
        public Dictionary<int, Dictionary<String, String>> Contributors { get; set; }

        [DbField(MaxSize = 128)]
        public String CName { get; set; }

        public short? CYear { get; set; }

        [DbField(MaxSize = 128)]
        public String PName { get; set; }

        public short? PYear { get; set; }

        [DbField(MaxSize = 128)]
        public String RecordingLocation { get; set; }

        public short? RecordingYear { get; set; }

        public bool AvailableSeparately { get; set; }

        [DbField(MaxSize = 16)]
        public CatalogTier? Tier { get; set; }
    }
}
