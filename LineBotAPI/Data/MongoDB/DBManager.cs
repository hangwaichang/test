
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.MongoDB
{
    class DBManager
    {
        private MongoClient _client = null;

        private IMongoDatabase _db = null;

        public DBManager()
        {
            var ConnectionString = GetMongoDBUrl();
            MongoUrl url = new MongoUrl(ConnectionString);
            var dbName = url.DatabaseName;

            _client = new MongoClient(ConnectionString);
            //_server = _client.GetServer();
            _db = _client.GetDatabase(dbName);
        }

        public static string GetMongoDBUrl()
        {
            string _connectionString = "mongodb://localhost:27017/Windom";
            return _connectionString;
        }
    }
}
