
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
    public class DISCRepository : Repository<disc>
    {
        public DISCRepository() : base(DBManager.GetMongoDBUrl()) { }

        public IEnumerable<disc> Get()
        {
            try
            {
                var result = this.FindAll();

                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}