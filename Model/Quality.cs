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
    public class Quality : ISerializable
    {
        public Quality()
        {
            Name = String.Empty;
        }

        public Quality(SerializationInfo info, StreamingContext ctxt)
        {
            Name = (String)info.GetValue("BabelMeta.Model.Quality.Name", typeof(String));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Quality.Name", Name);
        }

        public String Name { get; set; }
    }
}
