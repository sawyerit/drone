using Drone.Shared;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Drone.Scheduler.Datasources
{
	public class SchedulerDataSource
	{
        public MongoServer MongoSrvr { get; set; }
        public MongoDatabase MongoDB_PMG { get; set; }
        private const string JOBS_COLL = "Jobs";

        //methods to interact w/the mongo db
        public SchedulerDataSource()
        {
            string connectionString = ConfigurationManager.AppSettings["MongoDBConn"].ToString(); //"mongodb://mongodb1.jomax.paholdings.com/?replicaSet=BiDataMongoDBReplSet&readPreference=primary";            
            var client = new MongoClient(connectionString);
            GetMongoServer(client, true);
        }

        private void GetMongoServer(MongoClient client, bool retry)
        {
            try
            {
                MongoSrvr = client.GetServer();
                MongoDB_PMG = MongoSrvr.GetDatabase("PMG");
            }
            catch (Exception e)
            {
                if (retry)
                {
                    ExceptionExtensions.LogWarning(e, "SchedulerDataSource.GetMongoServer()", "Attempting one more time to connect to MongoDB");
                    GetMongoServer(client, false);
                }
                else
                {
                    ExceptionExtensions.LogError(e, "SchedulerDataSource.GetMongoServer()", "Second attempt to connect to MongoDB failed.");
                }
            }
        }

        public List<Job> GetJobCollectionFromServer()
        {
            MongoCollection jobsColl = null;

            jobsColl = MongoDB_PMG.GetCollection(JOBS_COLL);            
            List<Job> jobsList = jobsColl.FindAllAs<Job>().ToList(); //enumerate and find AS Job           

            foreach (Job job in jobsList)
                ConvertToLocalTime(job);

            return jobsList;
        }
        
        public Job GetJobByID(ObjectId objectId)
        {
            Job job = MongoDB_PMG.GetCollection<Job>(JOBS_COLL).FindOneById(objectId);
            //convert job lastrundates to localtime
            ConvertToLocalTime(job);
            return job;
        }

        public void AddJobToCollection(Job job)
        {
            ConvertToUtcTime(job);   
            MongoCollection<Job> jobCol = MongoDB_PMG.GetCollection<Job>(JOBS_COLL);
            WriteConcernResult res = jobCol.Insert<Job>(job);
            ConvertToLocalTime(job); //convert back because the obj reference will store the utc convert
        }

        public bool SaveJob(Job job)
        {
            ConvertToUtcTime(job);
            MongoCollection jobsColl = MongoDB_PMG.GetCollection(JOBS_COLL);
            WriteConcernResult res = jobsColl.Save<Job>(job, WriteConcern.Acknowledged);
            ConvertToLocalTime(job); //convert back because the obj reference will store the utc convert
            return res.Ok;
        }

        /// <summary>
        /// Convert the lastrunstart and lastruncomplete dates to local time from utc
        /// if the value is an empty string, that will be returned on purpose (for the UI)
        /// </summary>
        /// <param name="job"></param>
        private void ConvertToLocalTime(Job job)
        {
            if (!String.IsNullOrEmpty(job.Schedule[JobSchedule.LastRunStart.ToString()]))
            {
                DateTime start = DateTime.MinValue;
                DateTime.TryParse(job.Schedule[JobSchedule.LastRunStart.ToString()], out start);
                job.Schedule[JobSchedule.LastRunStart.ToString()] = DateTime.SpecifyKind(start, DateTimeKind.Utc).ToLocalTime().ToString();
            }

            if (!String.IsNullOrEmpty(job.Schedule[JobSchedule.LastRunComplete.ToString()]))
            {
                DateTime end = DateTime.MinValue;
                DateTime.TryParse(job.Schedule[JobSchedule.LastRunComplete.ToString()], out end);
                job.Schedule[JobSchedule.LastRunComplete.ToString()] = DateTime.SpecifyKind(end, DateTimeKind.Utc).ToLocalTime().ToString();
            }
        }

        private void ConvertToUtcTime(Job job)
        {
            if (!String.IsNullOrEmpty(job.Schedule[JobSchedule.LastRunStart.ToString()]))
            {
                DateTime start = DateTime.MinValue;
                DateTime.TryParse(job.Schedule[JobSchedule.LastRunStart.ToString()], out start);
                job.Schedule[JobSchedule.LastRunStart.ToString()] = DateTime.SpecifyKind(start, DateTimeKind.Local).ToUniversalTime().ToString();
            }

            if (!String.IsNullOrEmpty(job.Schedule[JobSchedule.LastRunComplete.ToString()]))
            {
                DateTime end = DateTime.MinValue;
                DateTime.TryParse(job.Schedule[JobSchedule.LastRunComplete.ToString()], out end);
                job.Schedule[JobSchedule.LastRunComplete.ToString()] = DateTime.SpecifyKind(end, DateTimeKind.Local).ToUniversalTime().ToString();
            }
        }
    }
    //internal void UpdateJobData(ObjectId jobID, string exitCode, string startTime, string endTime)
    //{
    //    //MongoDatabase localDB = MongoSrvr.GetDatabase(PMG_DB);

    //    IMongoQuery query = Query.EQ("_id", jobID);
    //    IMongoSortBy sortBy = SortBy.Null;
    //    IMongoUpdate update = Update.Set("Schedule.LastRunExitCode", exitCode)
    //                                .Set("Schedule.LastRunStart", startTime)
    //                                .Set("Schedule.LastRunComplete", endTime);

    //    MongoCollection jobsColl = MongoDB_PMG.GetCollection(JOBS_COLL);
    //    FindAndModifyResult result = jobsColl.FindAndModify(query, sortBy, update);

    //}
}
