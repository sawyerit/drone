using System;
//using Common.Logging;
using Quartz;
using Drone.Shared;
using Drone.Scheduler.Datasources;
using Drone.Scheduler.Components;

namespace Drone.Scheduler.Listeners
{
    public class BTEQListener : IJobListener
    {
        public virtual string Name
        {
            get { return "bteq"; } //same name as the job.jobtype from MongoDB
        }

        public virtual void JobToBeExecuted(IJobExecutionContext inContext)
        {
            //can determine what type of trigger this is an get more info from it if needed.
            Job job = inContext.Trigger.JobDataMap.Get(BaseJob.JOB_OBJ) as Job;
            job.Schedule[JobSchedule.LastRunStart.ToString()] = DateTime.Now.ToString();
            job.Schedule[JobSchedule.LastRunComplete.ToString()] = String.Empty;
            job.Schedule[JobSchedule.Status.ToString()] = JobStatus.Running.ToString();
            SchedulerDataSource ds = new SchedulerDataSource();
            ds.SaveJob(job);

            WriteToLog(string.Format("[{0}][INFO][Job Begin][Job Key: {1}]", DateTime.Now.ToString(), inContext.JobDetail.Key.Name));            
        }

        public virtual void JobExecutionVetoed(IJobExecutionContext inContext)
        {
            WriteToLog(string.Format("[{0}][INFO][Job Vetoed][Job Key: {1}]", DateTime.Now.ToString(), inContext.JobDetail.Key.Name));
        }

        public virtual void JobWasExecuted(IJobExecutionContext inContext, JobExecutionException inException)
        {
            Job job = inContext.Trigger.JobDataMap.Get(BaseJob.JOB_OBJ) as Job;
            job.Schedule[JobSchedule.LastRunComplete.ToString()] = DateTime.Now.ToString();
            job.Schedule[JobSchedule.Status.ToString()] = job.Schedule[JobSchedule.LastRunExitCode.ToString()] != "0" ? JobStatus.Failed.ToString() 
                                    : Conversions.ConvertTo(job.IsActive, false) ? JobStatus.Ready.ToString() 
                                    : JobStatus.Disabled.ToString();            

            WriteToLog(string.Format("[{0}][INFO][Job Complete][Job Key: {1}]", DateTime.Now.ToString(), inContext.JobDetail.Key.Name));
            if (!Object.Equals(null, inException))
            {
                job.Schedule[JobSchedule.Status.ToString()] = JobStatus.Failed.ToString();
                WriteToLog(string.Format("[{0}][ERROR][Job Exception][Job Key: {1}][{2}]", DateTime.Now.ToString(), inContext.JobDetail.Key.Name, inException.Message), "ERROR");
                ExceptionExtensions.LogError(inException.InnerException, "BTEQListener.JobWasExecuted()", "Job Key: " + inContext.JobDetail.Key.Name);
            }
            //else check the job datamap for any dependent jobs that should be triggered immediately

            SchedulerDataSource ds = new SchedulerDataSource();          
            ds.SaveJob(job);
        }

        private void WriteToLog(string message, string type = "INFO")
        {
            if (type == "ERROR")
            { 
                Utility.WriteToLogFile(String.Format("Error_Scheduler_{0:M_d_yyyy}", DateTime.Today) + ".log", message); 
            }
            else
            {
                Utility.WriteToLogFile(String.Format("Scheduler_Usage_{0:M_d_yyyy}", DateTime.Today) + ".log", message);
            }
        }
    }
}