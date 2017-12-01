using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Drone.Data
{
    public static class BSONHelper
    {
        private static string mongoDroneConnString = ConfigurationManager.AppSettings["MongoDBDrone"].ToString();
        private static MongoServer mServer = new MongoClient(mongoDroneConnString).GetServer();
        private static MongoDatabase mDBDrone = mServer.GetDatabase(MongoUrl.Create(mongoDroneConnString).DatabaseName);

        public static QueryDocument GetQueryFromJsonString(string jsonQuery)
        {
            return new QueryDocument(BsonSerializer.Deserialize<BsonDocument>(jsonQuery));
        }

        public static IEnumerable<T> GetByJsonString<T>(string jsonQuery, string collectionName)
        {
            QueryDocument query = GetQueryFromJsonString(jsonQuery);
            MongoCursor<T> items = mDBDrone.GetCollection<T>(collectionName).Find(query);

            return items as IEnumerable<T>;
        }

        public static MongoCursor<T> GetCursorByJsonString<T>(string jsonQuery, string collectionName)
        {
            QueryDocument query = GetQueryFromJsonString(jsonQuery);
            MongoCursor<T> items = mDBDrone.GetCollection<T>(collectionName).Find(query);

            return items;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryObject">c# anon object</param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetByObject<T>(object queryObject, string collectionName)
        {
            QueryDocument query = new QueryDocument(queryObject.ToBsonDocument());
            MongoCursor<T> items = mDBDrone.GetCollection<T>(collectionName).Find(query);

            return items as IEnumerable<T>;
        }

        public static MongoCollection GetCollection(string collection)
        {
            return mDBDrone.GetCollection(collection);
        }

        public static MongoCollection GetCollection(MongoCollectionSettings settings)
        {
            return mDBDrone.GetCollection(settings);
        }
    }
}
