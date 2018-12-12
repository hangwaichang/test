
using Data.Model;
using Data.MongoDB;
using MongoDB.Driver;
using Repository.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class LifeRepository : Repository<life>
    {
        public LifeRepository() : base(DBManager.GetMongoDBUrl()) { }

        Random Rnd = new Random();

        /// <summary>
        /// 單筆新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public life Add(life entity)
        {
            try
            {
                base.Insert(entity);;

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        #region 取值
        public life Get()
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
