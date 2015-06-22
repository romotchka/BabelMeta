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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BabelMeta.Model
{
    /// <summary>
    /// Artist class represents both Creators (Composer, Arrangr, etc.) and Performers.
    /// </summary>
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
            FirstName = (Dictionary<string, string>)info.GetValue("BabelMeta.Model.Artist.FirstName", typeof(Dictionary<string, string>));
            LastName = (Dictionary<string, string>)info.GetValue("BabelMeta.Model.Artist.LastName", typeof(Dictionary<string, string>));
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
        /// Artist first name, in different languages available.
        /// </summary>
        public Dictionary<string, string> FirstName { get; set; }

        /// <summary>
        /// Artist last name, in different languages available.
        /// </summary>
        public Dictionary<string, string> LastName { get; set; }
    }
}
