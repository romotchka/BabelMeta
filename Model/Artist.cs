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

        public Artist(SerializationInfo info, StreamingContext context)
        {
            Id = (int)info.GetValue("BabelMeta.Model.Artist.Id", typeof(int));
            Birth = (short?)info.GetValue("BabelMeta.Model.Artist.Birth", typeof(short?));
            Death = (short?)info.GetValue("BabelMeta.Model.Artist.Death", typeof(short?));
            FirstName = (Dictionary<String, String>)info.GetValue("BabelMeta.Model.Artist.FirstName", typeof(Dictionary<String, String>));
            LastName = (Dictionary<String, String>)info.GetValue("BabelMeta.Model.Artist.LastName", typeof(Dictionary<String, String>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Artist.Id", Id);
            info.AddValue("BabelMeta.Model.Artist.Birth", Birth);
            info.AddValue("BabelMeta.Model.Artist.Death", Death);
            info.AddValue("BabelMeta.Model.Artist.FirstName", FirstName);
            info.AddValue("BabelMeta.Model.Artist.LastName", LastName);
        }

        public int Id { get; set; }

        public short? Birth { get; set; }

        public short? Death { get; set; }

        /// <summary>
        /// Artist first name, in different languages available.
        /// </summary>
        public Dictionary<String, String> FirstName { get; set; }

        /// <summary>
        /// Artist last name, in different languages available.
        /// </summary>
        public Dictionary<String, String> LastName { get; set; }
    }
}
