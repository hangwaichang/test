
using MongoDB.Bson.Serialization.Attributes;
using Repository.Mongo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Data.Model
{
    [BsonIgnoreExtraElements]
    public class disc : Entity
    {
        /// <summary>
        /// DISC問題
        /// </summary>
        public string titel { get; set; }

        /// <summary>
        /// 答案1
        /// </summary>
        public string ans1 { get; set; }

        /// <summary>
        /// 答案2
        /// </summary>
        public string ans2 { get; set; }

        /// <summary>
        /// 答案3
        /// </summary>
        public string ans3 { get; set; }

        /// <summary>
        /// 答案4
        /// </summary>
        public string ans4 { get; set; }

    }
}