using Drone.Scheduler;
using Drone.Scheduler.Components;
using Drone.Scheduler.Datasources;
using Drone.Shared;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Linq;

namespace Drone.WebAPI.Services
{
    /// <summary>
    /// Gets data about the scheduler service.
    /// Does not implement any interfaces because its used by an MVC controller, 
    /// and not controlled by the IoC conatiner for WebAPI services
    /// </summary>
    public class SchedulerService
    {
        public static List<Job> GetAllJobs()
        {
            SchedulerDataSource ds = new SchedulerDataSource();
            List<Job> allJobs = ds.GetJobCollectionFromServer();

            foreach (Job j in allJobs)
                Expand(j);

            return allJobs;
        }

        public static Job GetJob(string id)
        {
            SchedulerDataSource ds = new SchedulerDataSource();
            Job j = ds.GetJobByID(ObjectId.Parse(id));
            Expand(j);

            return j;
        }

        public List<string> GetNextFive(string cron, string startDateTime)
        {
            return new JobScheduler().GetTriggerSchedule(cron, startDateTime);
        }

        public static Job EnableDisableJob(string id)
        {
            SchedulerDataSource ds = new SchedulerDataSource();
            Job j = ds.GetJobByID(ObjectId.Parse(id));

            //swap the isactive flag around
            bool newstat = !Conversions.ConvertTo(j.IsActive, true);

            j.IsActive = (newstat).ToString();
            j.LastModified = DateTime.Now.ToString();
            j.Schedule["Status"] = newstat ? "Ready" : "Disabled";
            //save it back to mongo
            ds.SaveJob(j);

            //expand it for UI use
            Expand(j);

            return j;
        }

        public static List<Job> RunJob(string id)
        {
            new JobScheduler().RunJob(id);
            return GetAllJobs();
        }

        public static void SaveSharedFile(HttpPostedFileBase file, string projName)
        {
            if (!String.IsNullOrWhiteSpace(file.FileName))
            {
                string newDir = Path.Combine(@"\\jomax.paholdings.com\data\busintel\scheduler", projName); //todo: config this
                Directory.CreateDirectory(newDir); //todo: add directory security and check for existence first?
                string fullPath = Path.Combine(newDir, file.FileName);

                file.SaveAs(fullPath);
            }
        }

        /// <summary>
        /// Dynamically created props for the scheduler dashboard view
        /// These shouldn't be used for scheduling or relied upon in the db
        /// </summary>
        /// <param name="j"></param>
        private static void Expand(Job j)
        {
            string scriptLocation = Path.Combine(@"\\jomax.paholdings.com\data\busintel\scheduler", j.ProjectName);

            j.Schedule["ScriptFileOnly"] = Path.GetFileName(j.ScriptFileName);
            j.Schedule["ScriptLocation"] = scriptLocation; //todo: config this
            j.Schedule["LogLocation"] = Path.Combine(scriptLocation, "Logs");
            j.Schedule["ExportLocation"] = Path.Combine(scriptLocation, "Export");
            if (Directory.Exists(j.Schedule["LogLocation"]))
                j.Schedule["ErrorExists"] = (Directory.GetFiles(j.Schedule["LogLocation"], "Errors*.log", System.IO.SearchOption.TopDirectoryOnly).Length > 0) ? "error" : "";

            j.Schedule["StringId"] = j._id.ToString();
            j.Schedule["NextRun"] = (new JobScheduler().GetTriggerSchedule(j.Schedule[JobSchedule.CronX.ToString()], DateTime.Now.ToString()))[0];                     
        }

        public static bool CreateNewJob(string jobid, string jobType, string projName, string scriptName, string isActive, string start, string cronX)
        {
            Job j = new Job();
            j.JobType = jobType;
            j.ProjectName = projName;
            j.ScriptFileName = scriptName;
            j.IsActive = isActive == "on" ? "true" : "false";
            j.LastModified = DateTime.Now.ToString();
            j.Schedule = new Dictionary<string, string>();
            j.Schedule[JobSchedule.StartTime.ToString()] = start;
            j.Schedule[JobSchedule.CronX.ToString()] = cronX;
            j.Schedule[JobSchedule.Status.ToString()] = Conversions.ConvertTo(j.IsActive, false) ? JobStatus.Ready.ToString() : JobStatus.Disabled.ToString();
            j.Schedule[JobSchedule.LastRunStart.ToString()] = string.Empty;
            j.Schedule[JobSchedule.LastRunComplete.ToString()] = string.Empty;
            j.Schedule[JobSchedule.LastRunExitCode.ToString()] = string.Empty;

            SchedulerDataSource ds = new SchedulerDataSource();
            if (!String.IsNullOrEmpty(jobid))
            {
                j._id = ObjectId.Parse(jobid);
                Job curJob = SchedulerService.GetJob(jobid);
                j.Schedule[JobSchedule.LastRunStart.ToString()] = curJob.Schedule[JobSchedule.LastRunStart.ToString()];
                j.Schedule[JobSchedule.LastRunComplete.ToString()] = curJob.Schedule[JobSchedule.LastRunComplete.ToString()];
                j.Schedule[JobSchedule.LastRunExitCode.ToString()] = curJob.Schedule[JobSchedule.LastRunExitCode.ToString()];
            }
            else
            {
                j._id = ObjectId.GenerateNewId();
            }

            return ds.SaveJob(j);
            
        }
    }
}