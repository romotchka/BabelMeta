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
    public class Tag : ISerializable
    {
        public Tag()
        {
            Name = string.Empty;
        }

        public Tag(SerializationInfo info, StreamingContext ctxt)
        {
            Name = (string)info.GetValue("BabelMeta.Model.Tag.Name", typeof(string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Tag.Name", Name);
        }

        public string Name { get; set; }
    }
}
