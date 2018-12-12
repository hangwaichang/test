using MongoDB.Bson.Serialization.Attributes;
using Repository.Mongo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Model
{
    [BsonIgnoreExtraElements]
    public class life : Entity
    {
        /// <summary>
        /// 格言
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string author { get; set; }


    }
}
