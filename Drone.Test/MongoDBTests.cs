using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Configuration;
using System.Collections.Generic;
using Drone.Scheduler;

namespace Drone.Test
{
    [TestClass]
    public class MongoDBTests
    {
        public MongoServer MongoSrvr { get; set; }

        [TestInitialize]
        public void Setup()
        {
            string connectionString = ConfigurationManager.AppSettings["MongoDBConn"].ToString(); //"mongodb://mongodb1.jomax.paholdings.com/?replicaSet=BiDataMongoDBReplSet&readPreference=primary";
            var client = new MongoClient(connectionString);
            MongoSrvr = client.GetServer();
        }

        [TestMethod]
        public void AddJobToMongoAndRemove()
        {
            ObjectId oid = ObjectId.GenerateNewId();
            Job job = new Job
            {
                _id = oid
                ,
                ScriptFileName = "scriptname.bteq"
                ,
                ProjectName = "TestRun"
                ,
                IsActive = "False"
            };

            job.Schedule = new Dictionary<string, string>();
            job.Schedule.Add("StartTime", "12:15");
            job.Schedule.Add("LastRunStart", String.Empty);
            job.Schedule.Add("LastRunComplete", String.Empty);
            job.Schedule.Add("LastRunExitCode", String.Empty);
            job.Schedule.Add("Status", JobStatus.Disabled.ToString());
            job.Schedule.Add("CronX", "0 0/2 * * * ?");

            MongoDatabase localDB = MongoSrvr.GetDatabase("PMG");
            MongoCollection<Job> jobCol = localDB.GetCollection<Job>("Test");
            WriteConcernResult res = jobCol.Insert<Job>(job);

            Assert.IsTrue(res.Ok);

            //cleanup
            res = jobCol.Remove(Query.EQ("_id", oid), RemoveFlags.Single, WriteConcern.Acknowledged);
            Assert.IsTrue(res.Ok);
        }


        [TestMethod]
        public void UpdateJobByIDAndRemove()
        {
            string dt = DateTime.Now.ToString();
            ObjectId oid = ObjectId.GenerateNewId();

            Job job = new Job
            {
                _id = oid
                ,
                ScriptFileName = "scriptname.bteq"
                ,
                ProjectName = "TestRun"
                ,
                LastModified = DateTime.Now.AddHours(-1).ToString()
            };

            MongoDatabase localDB = MongoSrvr.GetDatabase("PMG");
            MongoCollection<Job> jobCol = localDB.GetCollection<Job>("Test");
            WriteConcernResult res = jobCol.Insert<Job>(job);

            Assert.IsTrue(res.Ok);

            IMongoQuery query = Query.EQ("_id", oid);
            IMongoSortBy sortBy = SortBy.Null;
            IMongoUpdate update = Update.Set("LastModified", dt);

            MongoCollection jobsColl = localDB.GetCollection("Test");
            FindAndModifyResult result = jobsColl.FindAndModify(query, sortBy, update, true);
            Job modJob = result.GetModifiedDocumentAs<Job>();

            Assert.AreEqual(modJob.LastModified, dt);

            //cleanup
            res = jobCol.Remove(Query.EQ("_id", oid), RemoveFlags.Single, WriteConcern.Acknowledged);
            Assert.IsTrue(res.Ok);
        }

        [TestMethod]
        public void AddSourceDomainToMongoCollection()
        {
            ObjectId oid = ObjectId.GenerateNewId();
            BsonDocument doc = new BsonDocument { 
                {"_id", oid},
                {"rptGdDomainsID", 1}, //if missing, its not a gd domain
                {"DomainID", 135123344}, //informational for lookups on domain. Portfolio save from MSMQ uses rptGdDomainsID.
                {"DomainName", "godaddy.com"},
                {"PrivateLabelID", 1}, //should be 0 for non-gd domains?
                {"ShopperID", "shopid123"}, //empty string for non gd domains? if null, it won't get inserted into mongo unless we specify
                {"CreateDate", DateTime.Now.ToUniversalTime()},
                {"LastCrawlDate", DateTime.Now.AddHours(1).ToUniversalTime()},
            };


            MongoDatabase localDB = MongoSrvr.GetDatabase("DRONE");
            MongoCollection<BsonDocument> srcColl = localDB.GetCollection<BsonDocument>("TestSource");
            WriteConcernResult res = srcColl.Insert<BsonDocument>(doc);

            Assert.IsTrue(res.Ok);

            //cleanup
            res = srcColl.Remove(Query.EQ("_id", oid), RemoveFlags.Single, WriteConcern.Acknowledged);
            Assert.IsTrue(res.Ok);
            /*
             * We know that portfolio queue processor save uses rptGdDomainsID, so if we are processing a domain w/out that id we can assume its 
             * NOT a GD domain since its not in rptGdDomains.
             * If it's missing that ID, the queueprocessor can decide to use a diff datasource to store it elsewhere. This way we still get use
             * of the MSMQ even for non-GD domains and we can batch upload to MongoDB if we want
             */
        }
    }
}
