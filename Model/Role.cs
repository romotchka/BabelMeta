/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelMeta.Model
{
    public class Role
    {
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
