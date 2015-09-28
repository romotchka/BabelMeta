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
using System.ComponentModel;
using System.Runtime.Serialization;
using BabelMeta.Services.DbDriver;

namespace BabelMeta.Model
{
    /// <summary>
    /// Role determines how a contributor participates in an asset.
    /// </summary>
    [Serializable()]
    public class Role : ISerializable
    {
        public Role()
        {
            Name = String.Empty;
            Reference = QualifiedName.Unknown;
        }

        public Role(SerializationInfo info, StreamingContext context)
        {
            Name = (String)info.GetValue("BabelMeta.Model.Role.Name", typeof(String));
            Reference = (QualifiedName)info.GetValue("BabelMeta.Model.Role.Reference", typeof(QualifiedName));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Role.Name", Name);
            info.AddValue("BabelMeta.Model.Role.Reference", Reference);
        }

        private String _name = String.Empty;

        [DbField(MaxSize = 128)]
        public String Name 
        {
            get { return _name; }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (String.Compare(_name, value, StringComparison.Ordinal) == 0)
                {
                    return;
                }

                _name = value;

                // Attempt to retrieve a qualified name (standardized)
                switch (value)
                {
                    case "arranger": Reference = QualifiedName.Arranger; break;
                    case "choir": Reference = QualifiedName.Choir; break;
                    case "composer": Reference = QualifiedName.Composer; break;
                    case "conductor": Reference = QualifiedName.Conductor; break;
                    case "contributingartist": Reference = QualifiedName.ContributingArtist; break;
                    case "engineer": Reference = QualifiedName.Engineer; break;
                    case "ensemble": Reference = QualifiedName.Ensemble; break;
                    case "featuring": Reference = QualifiedName.Featuring; break;
                    case "lyricist": Reference = QualifiedName.Lyricist; break;
                    case "masteringengineer": Reference = QualifiedName.MasteringEngineer; break;
                    case "mixer": Reference = QualifiedName.Mixer; break;
                    case "musicaldirector": Reference = QualifiedName.MusicalDirector; break;
                    case "orchestra": Reference = QualifiedName.Orchestra; break;
                    case "performer": Reference = QualifiedName.Performer; break;
                    case "producer": Reference = QualifiedName.Producer; break;
                    case "remixer": Reference = QualifiedName.Remixer; break;
                    case "soloist": Reference = QualifiedName.Soloist; break;
                    case "transcriptor": Reference = QualifiedName.Transcriptor; break;
                    case "writer": Reference = QualifiedName.Writer; break;
                    default: Reference = QualifiedName.Unknown; break;
                }
            }
        }

        [DbField(MaxSize = 32)]
        public QualifiedName Reference { get; set; }

        /// <summary>
        /// Origin of the qualified name mentioned in comments, for informational traceability.
        /// Keep minimalist, do not add redundant roles.
        /// </summary>
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

            Unknown,
        }
    }
}
