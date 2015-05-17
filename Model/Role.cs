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
    public class Role : ISerializable
    {
        public Role()
        {
            Name = String.Empty;
            Reference = null;
        }

        public Role(SerializationInfo info, StreamingContext ctxt)
        {
            Name = (String)info.GetValue("BabelMeta.Model.Role.Name", typeof(String));
            Reference = (QualifiedName?)info.GetValue("BabelMeta.Model.Role.Reference", typeof(QualifiedName?));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Role.Name", Name);
            info.AddValue("BabelMeta.Model.Role.Reference", Reference);
        }

        public String Name { get; set; }

        public QualifiedName? Reference { get; set; }

        public enum QualifiedName
        {
            Arranger,               // FUGA native
            Choir,                  // FUGA native
            Composer,               // FUGA native
            Conductor,              // FUGA native
            ContributingArtist,     // FUGA native
            Engineer,               // FUGA native
            Ensemble,               // FUGA native
            Featuring,              // FUGA native
            Lyricist,               // FUGA native
            MasteringEngineer,      // Solstice
            Mixer,                  // FUGA native
            MusicalDirector,        // Solstice
            Orchestra,              // FUGA native
            Performer,              // FUGA native
            Producer,               // FUGA native
            Remixer,                // FUGA native
            Soloist,                // FUGA native
            Transcriptor,           // Solstice
            Writer,                 // FUGA native
        }
    }
}
