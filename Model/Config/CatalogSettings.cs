/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BabelMeta.Model.Config
{
    /// <summary>
    /// Default values
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
