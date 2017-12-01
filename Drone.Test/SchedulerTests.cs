using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Drone.Scheduler.Components;
using System.Collections.Generic;
using System.Threading;
using Drone.Scheduler.Datasources;
using Drone.Shared;
using Drone.Scheduler;
using MongoDB.Bson;

namespace Drone.Test
{
    [TestClass]
    public class SchedulerTests
    {
        [TestInitialize]
        public void Setup()
        {
            Utility.ComponentBaseFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scheduler");
        }

        [TestMethod]
        public void SetupFilesAndDirectories()
        {
            Job job = new Job
            {
                IsActive = "false"
                ,ScriptFileName = @"\\jomax.paholdings.com\data\Busintel\Scheduler\TrafficBatch\trafficbatch.bteq"
                ,ProjectName = "TrafficBatch"
                ,JobType = "bteq"
                ,LastModified = DateTime.Now.ToString()
            };

            job.Schedule = new Dictionary<string, string>();
            job.Schedule.Add("StartTime", "05:00");
            job.Schedule.Add("LastRunStart", String.Empty);
            job.Schedule.Add("LastRunComplete", String.Empty);
            job.Schedule.Add("LastRunExitCode", String.Empty);
            job.Schedule.Add("Status", JobStatus.Disabled.ToString());
            job.Schedule.Add("CronX", "0 0/10 * * * ?");

            BTEQJob bteq = new BTEQJob();
            bteq.MyJob = job;
            bteq.SetupFilesAndDirectories();
            
            Assert.IsTrue(bteq.WorkingDirectory.Contains(@"\\jomax.paholdings.com\data\busintel\scheduler"));
        }

        [TestMethod]
        [Ignore]
        public void AddJobToMongoPMGJobs()
        {
            Job job = new Job
            {
                IsActive = "false"
                ,ScriptFileName = @"\\jomax.paholdings.com\data\Busintel\Scheduler\TrafficBatch\trafficbatch.bteq"
                ,ProjectName = "TrafficBatch"
                ,JobType = "bteq"
                ,LastModified = DateTime.Now.ToString()
            };

            job.Schedule = new Dictionary<string, string>();
            job.Schedule.Add("StartTime", "05:00");
            job.Schedule.Add("LastRunStart", String.Empty);
            job.Schedule.Add("LastRunComplete", String.Empty);
            job.Schedule.Add("LastRunExitCode", String.Empty);
            job.Schedule.Add("Status", JobStatus.Disabled.ToString());
            job.Schedule.Add("CronX", "0 0/10 * * * ?");

            SchedulerDataSource schds = new SchedulerDataSource();
            schds.AddJobToCollection(job);
        }

        [TestMethod]
        public void Scheduler_GetData()
        {            
            JobScheduler sch = new JobScheduler();
            
            sch.GetData(sch.Context);
        }


        [TestMethod]
        [Ignore]
        public void WebAPI_SchedulerDataSource_SaveJob()
        {
            ObjectId oid = ObjectId.GenerateNewId();
            Job job = new Job
            {
                _id = oid
                ,ScriptFileName = "scriptname.bteq"
                ,ProjectName = "TestRun"
                ,IsActive = "False"
            };

            job.Schedule = new Dictionary<string, string>();
            job.Schedule.Add("StartTime", "12:15");
            job.Schedule.Add("LastRunStart", DateTime.Now.ToString());
            job.Schedule.Add("LastRunComplete", DateTime.Now.AddMinutes(1).ToString());
            job.Schedule.Add("LastRunExitCode", "0");
            job.Schedule.Add("Status", JobStatus.Disabled.ToString());
            job.Schedule.Add("CronX", "0 0/2 * * * ?");

            SchedulerDataSource ds = new SchedulerDataSource();
            ds.SaveJob(job);
        }

        [TestMethod]
        public void GetCronExpression_Multiple()
        {
            //verify cron expression at http://www.cronmaker.com/
            ObjectId oid = ObjectId.GenerateNewId();
            JobScheduler js = new JobScheduler();
            Job job = new Job
            {
                _id = oid,
                ScriptFileName = @"C:\bteqfiles\Testtest.bteq",
                ProjectName = "Drone.Test.BTEQ"
            };
            job.Schedule = new Dictionary<string, string>();
            job.Schedule.Add("LastRunStart", String.Empty);
            job.Schedule.Add("LastRunComplete", String.Empty);
            job.Schedule.Add("LastRunExitCode", String.Empty);


            //hourly 5:15am
            job.Schedule.Add("StartTime", "5:15");
            job.Schedule.Add("Interval", "Hourly");
            DateTimeOffset startTime = new DateTimeOffset(Conversions.ConvertTo(job.Schedule["StartTime"], DateTime.Now));
                       
            Assert.AreEqual("0 15 0/1 1/1 1/1 ?", js.GetCronExpression(job, startTime));


            //twice daily (every 12 hours at 3am and 3pm)
            job.Schedule["StartTime"] = "3:00";
            job.Schedule["Interval"] = "TwiceDay";
            startTime = new DateTimeOffset(Conversions.ConvertTo(job.Schedule["StartTime"], DateTime.Now));

            Assert.AreEqual("0 0 3/12 1/1 1/1 ?", js.GetCronExpression(job, startTime));


            //Daily 5am
            job.Schedule["StartTime"]= "5:00";
            job.Schedule["Interval"] = "Daily";
            startTime = new DateTimeOffset(Conversions.ConvertTo(job.Schedule["StartTime"], DateTime.Now));
            
            Assert.AreEqual("0 0 5 1/1 1/1 ?", js.GetCronExpression(job, startTime));


            //Weekly 5am Mondays
            job.Schedule["StartTime"] = "02/03/2014 5:00";
            job.Schedule["Interval"] = "Weekly";
            startTime = new DateTimeOffset(Conversions.ConvertTo(job.Schedule["StartTime"], DateTime.Now));

            Assert.AreEqual("0 0 5 ? 1/1 Mon", js.GetCronExpression(job, startTime));

             
            //Monthly 5am 1st of the month
            job.Schedule["StartTime"] = "02/01/2014 5:00";
            job.Schedule["Interval"] = "Monthly";
            startTime = new DateTimeOffset(Conversions.ConvertTo(job.Schedule["StartTime"], DateTime.Now));

            Assert.AreEqual("0 0 5 1 1/1 ?", js.GetCronExpression(job, startTime));

            //twice a day at 5am and 5pm, only every 2nd tues of the month. lol.
            //0 0 5/12 ? 1/1 TUE#2
        }
    }
}
