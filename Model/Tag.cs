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
            Name = String.Empty;
        }

        public Tag(SerializationInfo info, StreamingContext ctxt)
        {
            Name = (String)info.GetValue("BabelMeta.Model.Tag.Name", typeof(String));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Tag.Name", Name);
        }

        public String Name { get; set; }
    }
}
