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

namespace BabelMeta.Model
{
    [Serializable()]
    public class Lang : ISerializable
    {
        public Lang()
        {
            LongName = string.Empty;
            ShortName = string.Empty;
            IsDefault = false;
        }

        public Lang(SerializationInfo info, StreamingContext ctxt)
        {
            LongName = (string)info.GetValue("BabelMeta.Model.Lang.LongName", typeof(string));
            ShortName = (string)info.GetValue("BabelMeta.Model.Lang.ShortName", typeof(string));
            IsDefault = (bool)info.GetValue("BabelMeta.Model.Lang.IsDefault", typeof(bool));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Lang.LongName", LongName);
            info.AddValue("BabelMeta.Model.Lang.ShortName", ShortName);
            info.AddValue("BabelMeta.Model.Lang.IsDefault", IsDefault);
        }

        public string LongName { get; set; }

        public string ShortName { get; set; }

        public bool IsDefault { get; set; }
    }
}
