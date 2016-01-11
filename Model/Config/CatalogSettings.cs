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
    [Serializable]
    public class CatalogSettings : ISerializable
    {
        public CatalogSettings()
        {
            SupplierDefault = String.Empty;
            LabelDefault = String.Empty;
            COwnerDefault = String.Empty;
            CYearDefault = (short)DateTime.Now.Year;
            POwnerDefault = String.Empty;
            PYearDefault = (short)DateTime.Now.Year;
            CatalogTierDefault = CatalogTier.Front;
            MainGenreDefault = String.Empty;
            FormatDefault = ProductFormat.Album;
            AvailableSeparatelyDefault = true;
        }

        public CatalogSettings(SerializationInfo info, StreamingContext context)
        {
            SupplierDefault = (String)info.GetValue("BabelMeta.Model.CatalogSettings.SupplierDefault", typeof(String));
            LabelDefault = (String)info.GetValue("BabelMeta.Model.CatalogSettings.LabelDefault", typeof(String));
            COwnerDefault = (String)info.GetValue("BabelMeta.Model.CatalogSettings.COwnerDefault", typeof(String));
            CYearDefault = (short)info.GetValue("BabelMeta.Model.CatalogSettings.CYearDefault", typeof(short));
            POwnerDefault = (String)info.GetValue("BabelMeta.Model.CatalogSettings.POwnerDefault", typeof(String));
            PYearDefault = (short)info.GetValue("BabelMeta.Model.CatalogSettings.PYearDefault", typeof(short));
            CatalogTierDefault = (CatalogTier)info.GetValue("BabelMeta.Model.CatalogSettings.CatalogTierDefault", typeof(CatalogTier));
            MainGenreDefault = (String)info.GetValue("BabelMeta.Model.CatalogSettings.MainGenreDefault", typeof(String));
            FormatDefault = (ProductFormat)info.GetValue("BabelMeta.Model.CatalogSettings.FormatDefault", typeof(ProductFormat));
            AvailableSeparatelyDefault = (bool)info.GetValue("BabelMeta.Model.CatalogSettings.AvailableSeparatelyDefault", typeof(bool));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.CatalogSettings.SupplierDefault", SupplierDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.LabelDefault", LabelDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.COwnerDefault", COwnerDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.CYearDefault", CYearDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.POwnerDefault", POwnerDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.PYearDefault", PYearDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.CatalogTierDefault", CatalogTierDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.MainGenreDefault", MainGenreDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.FormatDefault", FormatDefault);
            info.AddValue("BabelMeta.Model.CatalogSettings.AvailableSeparatelyDefault", AvailableSeparatelyDefault);
        }

        public String SupplierDefault { get; set; }

        public String LabelDefault { get; set; }

        public String COwnerDefault { get; set; }

        public short CYearDefault { get; set; }

        public String POwnerDefault { get; set; }

        public short PYearDefault { get; set; }

        public CatalogTier CatalogTierDefault { get; set; }

        public String MainGenreDefault { get; set; }

        public ProductFormat FormatDefault { get; set; }

        public bool AvailableSeparatelyDefault { get; set; }

        // TODO: Complete with default behaviours
    }
}
