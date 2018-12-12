
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
    public class soul : Entity
    {
        /// <summary>
        /// 心靈小語
        /// </summary>
        public string message { get; set; }
    }
}