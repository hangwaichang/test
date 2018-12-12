
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
    public class SoulRepository: Repository<soul>
    {
        public SoulRepository() : base(DBManager.GetMongoDBUrl()) { }

        Random Rnd = new Random();

        #region 取值
        public soul Get()
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