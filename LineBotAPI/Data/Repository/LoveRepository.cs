using Data.Model;
using Data.MongoDB;
using Repository.Mongo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Data.Repository
{
    public class LoveRepository:Repository<love>
    {
        public LoveRepository() : base(DBManager.GetMongoDBUrl()) { }

        Random Rnd = new Random();

        #region 取值
        public love Get()
        {
            try
            {
                int skip = Rnd.Next((int)this.Count());
                var result = this.FindAll().Skip(skip).First();

                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}