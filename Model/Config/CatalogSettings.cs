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

using System;
using System.Runtime.Serialization;

namespace BabelMeta.Model.Config
{
    /// <summary>
    /// CatalogSettings provides with a series of default values.
    /// </summary>
    [Serializable()]
    public class CatalogSettings : ISerializable
    {
        public CatalogSettings()
        {
            SupplierDefault = string.Empty;
            LabelDefault = string.Empty;
            COwnerDefault = string.Empty;
            POwnerDefault = string.Empty;
            CatalogTierDefault = CatalogTier.Front;
            MainGenreDefault = string.Empty;
            FormatDefault = ProductFormat.Album;
            AvailableSeparatelyDefault = true;
        }

        public CatalogSettings(SerializationInfo info, StreamingContext ctxt)
        {
            SupplierDefault = (string)info.GetValue("BabelMeta.Model.CatalogSettings.SupplierDefault", typeof(string));
            LabelDefault = (string)info.GetValue("BabelMeta.Model.CatalogSettings.LabelDefault", typeof(string));
            COwnerDefault = (string)info.GetValue("BabelMeta.Model.CatalogSettings.COwnerDefault", typeof(string));
            POwnerDefault = (string)info.GetValue("BabelMeta.Model.CatalogSettings.POwnerDefault", typeof(string));
            CatalogTierDefault = (CatalogTier)info.GetValue("BabelMeta.Model.CatalogSettings.CatalogTierDefault", typeof(CatalogTier));
            MainGenreDefault = (string)info.GetValue("BabelMeta.Model.CatalogSettings.MainGenreDefault", typeof(string));
            FormatDefault = (ProductFormat)info.GetValue("BabelMeta.Model.CatalogSettings.FormatDefault", typeof(ProductFormat));
            AvailableSeparatelyDefault = (bool)info.GetValue("BabelMeta.Model.CatalogSettings.AvailableSeparatelyDefault", typeof(bool));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.CatalogSettings.SupplierDefault", SupplierDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.LabelDefault", LabelDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.COwnerDefault", COwnerDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.POwnerDefault", POwnerDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.CatalogTierDefault", CatalogTierDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.MainGenreDefault", MainGenreDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.FormatDefault", FormatDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.AvailableSeparatelyDefault", AvailableSeparatelyDefault);
        }

        public string SupplierDefault { get; set; }

        public string LabelDefault { get; set; }

        public string COwnerDefault { get; set; }

        public string POwnerDefault { get; set; }

        public CatalogTier CatalogTierDefault { get; set; }

        public string MainGenreDefault { get; set; }

        public ProductFormat FormatDefault { get; set; }

        public bool AvailableSeparatelyDefault { get; set; }

        // TODO: Complete with default behaviours
    }
}
