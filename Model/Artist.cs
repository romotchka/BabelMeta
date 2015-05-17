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
    public class Artist : ISerializable 
    {
        public Artist()
        {
            Id = 0;
            Birth = null;
            Death = null;
            FirstName = null;
            LastName = null;
        }

        public Artist(SerializationInfo info, StreamingContext ctxt)
        {
            Id = (Int32)info.GetValue("BabelMeta.Model.Artist.Id", typeof(Int32));
            Birth = (Int16?)info.GetValue("BabelMeta.Model.Artist.Birth", typeof(Int16?));
            Death = (Int16?)info.GetValue("BabelMeta.Model.Artist.Death", typeof(Int16?));
            FirstName = (Dictionary<Lang, String>)info.GetValue("BabelMeta.Model.Artist.FirstName", typeof(Dictionary<Lang, String>));
            LastName = (Dictionary<Lang, String>)info.GetValue("BabelMeta.Model.Artist.LastName", typeof(Dictionary<Lang, String>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Artist.Id", Id);
            info.AddValue("BabelMeta.Model.Artist.Birth", Birth);
            info.AddValue("BabelMeta.Model.Artist.Death", Death);
            info.AddValue("BabelMeta.Model.Artist.FirstName", FirstName);
            info.AddValue("BabelMeta.Model.Artist.LastName", LastName);
        }

        public Int32 Id { get; set; }

        public Int16? Birth { get; set; }

        public Int16? Death { get; set; }

        /// <summary>
        /// The field name is singular because it represents the same entry in different languages available
        /// </summary>
        public Dictionary<Lang, String> FirstName { get; set; }

        /// <summary>
        /// The field name is singular because it represents the same entry in different languages available
        /// </summary>
        public Dictionary<Lang, String> LastName { get; set; }
    }
}
