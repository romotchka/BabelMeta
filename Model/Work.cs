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
    /// Work represents a composition or a part of it (movement, etc.).
    /// </summary>
    [Serializable()]
    public class Work : ISerializable
    {
        public Work()
        {
            Id = 0;
            Parent = null;
            MovementNumber = null;
            Title = null;
            MovementTitle = null;
            Contributors = null;
            ClassicalCatalog = string.Empty;
            Tonality = null;
            Year = null;
        }

        public Work(SerializationInfo info, StreamingContext ctxt)
        {
            Id = (Int32)info.GetValue("BabelMeta.Model.Work.Id", typeof(Int32));
            Parent = (Int32?)info.GetValue("BabelMeta.Model.Work.Parent", typeof(Int32?));
            MovementNumber = (Int16?)info.GetValue("BabelMeta.Model.Work.MovementNumber", typeof(Int16?));
            Title = (Dictionary<string, string>)info.GetValue("BabelMeta.Model.Work.Title", typeof(Dictionary<string, string>));
            MovementTitle = (Dictionary<string, string>)info.GetValue("BabelMeta.Model.Work.MovementTitle", typeof(Dictionary<string, string>));
            Contributors = (Dictionary<Int32, Role>)info.GetValue("BabelMeta.Model.Work.Contributors", typeof(Dictionary<Int32, Role>));
            ClassicalCatalog = (string)info.GetValue("BabelMeta.Model.Work.ClassicalCatalog", typeof(string));
            Tonality = (Key?)info.GetValue("BabelMeta.Model.Work.Tonality", typeof(Key?));
            Year = (Int16?)info.GetValue("BabelMeta.Model.Work.Year", typeof(Int16?));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BabelMeta.Model.Work.Id", Id);
            info.AddValue("BabelMeta.Model.Work.Parent", Parent);
            info.AddValue("BabelMeta.Model.Work.MovementNumber", MovementNumber);
            info.AddValue("BabelMeta.Model.Work.Title", Title);
            info.AddValue("BabelMeta.Model.Work.MovementTitle", MovementTitle);
            info.AddValue("BabelMeta.Model.Work.Contributors", Contributors);
            info.AddValue("BabelMeta.Model.Work.ClassicalCatalog", ClassicalCatalog);
            info.AddValue("BabelMeta.Model.Work.Tonality", Tonality);
            info.AddValue("BabelMeta.Model.Work.Year", Year);
        }

        /// <summary>
        /// Work internal id.
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Parent work, if any. Applicable mostly for classical works.
        /// </summary>
        public Int32? Parent { get; set; }

        /// <summary>
        /// Movement number if the work is a sub-work of a parent work. Applicable mostly for classical works.
        /// </summary>
        public Int16? MovementNumber { get; set; }

        /// <summary>
        /// Work main title in the different available languages.
        /// If a parent work exists, the Title would typically contain the parent work title, a separator, and the movement title.
        /// </summary>
        public Dictionary<string, string> Title { get; set; }

        /// <summary>
        /// Movement title when available, the different available languages.
        /// </summary>
        public Dictionary<string, string> MovementTitle { get; set; }

        /// <summary>
        /// Any Contributor in the work e.g. Composer or Arranger.
        /// </summary>
        public Dictionary<Int32, Role> Contributors { get; set; }

        /// <summary>
        /// Opus number. Applicable mostly for classical works.
        /// </summary>
        public string ClassicalCatalog { get; set; }

        /// <summary>
        /// Work tonality.
        /// </summary>
        public Key? Tonality { get; set; }

        /// <summary>
        /// Year of work composition.
        /// </summary>
        public Int16? Year { get; set; }
    }
}
