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
            ClassicalCatalog = String.Empty;
            Tonality = null;
            Year = null;
        }

        public Work(SerializationInfo info, StreamingContext ctxt)
        {
            Id = (Int32)info.GetValue("BabelMeta.Model.Work.Id", typeof(Int32));
            Parent = (Int32?)info.GetValue("BabelMeta.Model.Work.Parent", typeof(Int32?));
            MovementNumber = (Int16?)info.GetValue("BabelMeta.Model.Work.MovementNumber", typeof(Int16?));
            Title = (Dictionary<Lang, String>)info.GetValue("BabelMeta.Model.Work.Title", typeof(Dictionary<Lang, String>));
            MovementTitle = (Dictionary<Lang, String>)info.GetValue("BabelMeta.Model.Work.MovementTitle", typeof(Dictionary<Lang, String>));
            Contributors = (Dictionary<Int32, Role>)info.GetValue("BabelMeta.Model.Work.Contributors", typeof(Dictionary<Int32, Role>));
            ClassicalCatalog = (String)info.GetValue("BabelMeta.Model.Work.ClassicalCatalog", typeof(String));
            Tonality = (Key?)info.GetValue("BabelMeta.Model.Work.Tonality", typeof(Key?));
            Year = (Int16?)info.GetValue("BabelMeta.Model.Artist.Year", typeof(Int16?));
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

        public Int32 Id { get; set; }

        public Int32? Parent { get; set; }

        public Int16? MovementNumber { get; set; }

        /// <summary>
        /// The field name is singular because it represents the same entry in different languages available
        /// </summary>
        public Dictionary<Lang, String> Title { get; set; }

        /// <summary>
        /// The field name is singular because it represents the same entry in different languages available
        /// </summary>
        public Dictionary<Lang, String> MovementTitle { get; set; }

        /// <summary>
        /// Work Contributor is e.g. the Composer or Arranger
        /// </summary>
        public Dictionary<Int32, Role> Contributors { get; set; }

        public String ClassicalCatalog { get; set; }

        public Key? Tonality { get; set; }

        public Int16? Year { get; set; }
    }
}
