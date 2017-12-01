//using Common.Logging;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Scheduler.Datasources;
using Drone.Scheduler.Listeners;
using Drone.Shared;
using MongoDB.Bson;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;

namespace Drone.Scheduler.Components
{
    [Export(typeof(IDroneComponent))]
    public class JobScheduler : BaseComponent<JobSchedulerComponent>
    {
        #region public properties

        private SchedulerDataSource _datasource;
        IScheduler sched = null;
        ISchedulerFactory sf = null;
        private DateTime _lastCheck = DateTime.Now;

        #endregion

        #region constructors

        [ImportingConstructor]
        public JobScheduler()
            : base()
        {
            _datasource = new SchedulerDataSource();
            sf = new StdSchedulerFactory();
        }

        #endregion

        public override void GetData(object context)
        {
            try
            {
                BaseContext cont = context as BaseContext;

                lock (cont) cont.NextRun = DateTime.MinValue; //If this process exits for some reason, Drone.Manager will restart based on this

                if (!Object.Equals(cont, null) && GetContextStatus(cont) != "processing")
                {
                    Context = cont;
                    SetContextStatus("processing", cont);

                    while (!cont.ShuttingDown)
                    {

                        try
                        {
                            //1. Get list of jobs to run from Mongo
                            List<Job> allJobs = _datasource.GetJobCollectionFromServer(); //config file these?? CONST these?                           

                            //get scheduler instance if it doesn't already exist
                            sched = sf.GetScheduler("DroneQuartzScheduler");
                            if (Object.Equals(null, sched))
                            {
                                sched = sf.GetScheduler();
                                //scheduler listener
                                sched.ListenerManager.AddSchedulerListener(new SchedulerListener());
                            }

                            //2. For each job, check exist..create or reschedule QuartzJob, Trigger, and wire listener
                            foreach (Job job in allJobs)
                            {
                                if (!ValidateJob(job)) continue;

                                JobKey jk = new JobKey(job.ProjectName, job.JobType); //get current job if it exists
                                if (!sched.CheckExists(jk) && Conversions.ConvertTo(job.IsActive, false))
                                {
                                    CreateJob(job);
                                }
                                else
                                {
                                    //job already exists, create new if its been modified since last check
                                    if (DateTime.Parse(job.LastModified) > _lastCheck)
                                    {
                                        if (!Conversions.ConvertTo(job.IsActive, true))
                                        {
                                            //pauses all the triggers for the job, so it won't fire.
                                            sched.PauseJob(jk);
                                        }
                                        else //something else changed, reschedule it
                                        {
                                            RescheduleJob(jk, job);
                                        }
                                    }
                                }
                            }


                            // All of the jobs have been added to the scheduler, now start it
                            if (!sched.IsStarted)
                                sched.Start();

                        }
                        catch (Exception ex)
                        {
                            ExceptionExtensions.LogError(ex, "Scheduler.GetData()");
                        }

                        // in 1 min... check for updates in MongoDB again
                        _lastCheck = DateTime.Now;

                        while (!cont.ShuttingDown && _lastCheck.AddMinutes(1) > DateTime.Now)
                            Thread.Sleep(3000);
                    }

                    //shutting down
                    sched.Shutdown(true);

                    WriteToUsageLogFile("Scheduler.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Stopped Scheduler");

                    SetContextStatus("waiting", cont);
                }

                RaiseProcessingCompletedEvent(new EventArgs());
            }
            catch (Exception e)
            {
                SetContextStatus("waiting", Context);
                ExceptionExtensions.LogError(e, "Drone.Scheduler.Components.Scheduler.GetData()");
            }
        }

        private bool ValidateJob(Job job)
        {
            return !String.IsNullOrEmpty(job.ScriptFileName)
                && !String.IsNullOrEmpty(job.ProjectName)
                && !String.IsNullOrEmpty(job.JobType)
                && !String.IsNullOrEmpty(job.Schedule[JobSchedule.CronX.ToString()])
                && !String.IsNullOrEmpty(job.Schedule[JobSchedule.StartTime.ToString()]);
        }

        /// <summary>
        /// Creates a job then creates and attaches a trigger. 
        /// If a listener for this jobtype doesn't exist, it will create and attach one as well
        /// </summary>
        /// <param name="job"></param>
        private void CreateJob(Job job)
        {
            IJobDetail job1 = JobBuilder.Create(Type.GetType("Drone.Scheduler.Components." + job.JobType.ToUpper() + "Job"))
                .WithIdentity(job.ProjectName, job.JobType)
                .Build();


            TriggerBuilder tb = TriggerBuilder.Create();
            ITrigger trigger = BuildCronTrigger(tb, job);

            //setup listener if one doesn't exist for this "group".  group is also JobType in MongoDB
            IList<IJobListener> listList = sched.ListenerManager.GetJobListeners();
            if (Object.Equals(null, listList.FirstOrDefault(l => l.Name == job.JobType)))
            {
                sched.ListenerManager.AddJobListener(new BTEQListener(), GroupMatcher<JobKey>.GroupEquals(job.JobType));
            }

            // schedule the job to run
            DateTimeOffset scheduleTime1 = sched.ScheduleJob(job1, trigger);
        }

        /// <summary>
        /// Gets a reference to the old trigger builder and builds a new trigger using it.
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="job"></param>
        private void RescheduleJob(JobKey jobKey, Job job)
        {
            ITrigger oldTrigger = sched.GetTrigger(new TriggerKey(job.ProjectName + "_trigger", job.JobType));
            TriggerBuilder tb = oldTrigger.GetTriggerBuilder();

            ITrigger newTrigger = BuildCronTrigger(tb, job);
            sched.RescheduleJob(oldTrigger.Key, newTrigger);


        }

        private ICronTrigger BuildCronTrigger(TriggerBuilder tb, Job job)
        {
            DateTimeOffset startTime = new DateTimeOffset(Conversions.ConvertTo(job.Schedule[JobSchedule.StartTime.ToString()], DateTime.Now));

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                         .WithIdentity(job.ProjectName + "_trigger", job.JobType)
                                         .StartAt(startTime)
                                         .WithCronSchedule(job.Schedule[JobSchedule.CronX.ToString()], x => x.WithMisfireHandlingInstructionDoNothing())
                                         .Build();

            trigger.JobDataMap.Put(BaseJob.JOB_OBJ, job);

            return trigger;
        }

        //todo: move this to webapp creation of job
        public string GetCronExpression(Job job, DateTimeOffset startTime)
        {
            //0 5/30 * * * ?    - every 30 min at 5 after
            //0 12 * * * ?      - every hour at 12 after
            //0 0 0/1 1/1 * ?   - every hour on the hour, starting now
            //0 0 2/1 1/1 * ?   - every hour on the hour, starting now if its after 2am...otherwise starting at 2am
            //0 12 5 * * ?      - every day at 5:12am
            //0 0 5/12 * * ?    - twice a day at 5am and 5pm
            //0 0 5 ? * SUN,THU - weekly SUN and THU (twice a week) (just one day is weekly on that day)
            //0 0 5 1 1/1 ?       - monthly, at 5am on the 1st of the month
            //0 0 5 1 1/2 ?     - every 2 months at 5am on the 1st of the month
            // * can also be 1/1.  1/2 is every other.  1/3 is every 3rd.
            //sec min hr DoM Month DofW [Year]

            string cronx = "0 0 0/1 1/1 * ?"; //every hour, top of the hour, starting now

            int sec = startTime.Second;
            int min = startTime.Minute;
            string hr = "1/1";
            string day = "1/1";
            string mnth = "1/1";
            string dow = "?";

            switch (job.Schedule["Interval"].ToLower())
            {
                case "hourly":
                    hr = "0/1";
                    break;
                case "twiceday":
                    hr = startTime.Hour + "/12";
                    break;
                case "daily":
                    hr = startTime.Hour.ToString();
                    break;
                case "weekly":
                    hr = startTime.Hour.ToString();
                    day = "?";
                    dow = startTime.ToString("ddd");
                    break;
                case "monthly":
                    hr = startTime.Hour.ToString();
                    day = startTime.Day.ToString();
                    mnth = "1/1";
                    break;
                default:
                    break;
            }

            cronx = string.Format("{0} {1} {2} {3} {4} {5}", sec, min, hr, day, mnth, dow);

            return cronx;
        }

        #region WebService Helpers

        /// <summary>
        /// Creates a new instance of a quartz scheduler, adds a new job and triggers immediately
        /// Can't figure out how to get ahold of the running scheduler
        /// </summary>
        /// <param name="id">ObjectId.tostring from MongoDB.PMG.Jobs</param>
        public void RunJob(string id)
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            if (!Object.Equals(null, sched))
            {
                Job job = _datasource.GetJobByID(ObjectId.Parse(id.Replace("\"", string.Empty)));
                IJobDetail jobDet = CreateRunNowJob(job);
                sched.AddJob(jobDet, true);
                sched.ListenerManager.AddJobListener(new BTEQListener(), GroupMatcher<JobKey>.GroupEquals(job.JobType));
                sched.Start();
                sched.TriggerJob(jobDet.Key, jobDet.JobDataMap);
            }
            Thread.Sleep(1000);
            sched.Shutdown(true);
        }

        /// <summary>
        /// Creates a durable Job with no associated triggers and adds it to the scheduler as dormant.
        /// This job can be run by either scheduling it or triggerJob immediately.
        /// Called by the SchedulerService webapi and can be used for chained dependent jobs.
        /// </summary>
        /// <param name="job">Drone Job entity</param>
        /// <returns>IJobDetail</returns>
        public IJobDetail CreateRunNowJob(Job job)
        {
            IJobDetail job1 = JobBuilder.Create(Type.GetType("Drone.Scheduler.Components." + job.JobType.ToUpper() + "Job"))
                .WithIdentity(job.ProjectName, job.JobType)
                .StoreDurably(true)
                .Build();

            job1.JobDataMap.Put(BaseJob.JOB_OBJ, job);

            return job1;
        }

        public List<string> GetTriggerSchedule(string cron, string startDateTime)
        {
            List<string> nextFive = new List<string>();
            DateTime dtToStart = DateTime.MinValue;
            DateTime.TryParse(startDateTime, out dtToStart);

            if (!Object.Equals(DateTime.MinValue, dtToStart))
            {
                DateTimeOffset? dto = new DateTimeOffset(dtToStart);

                CronExpression ce = new CronExpression(cron);
                dto = ce.GetTimeAfter(dto.Value);

                for (int i = 0; i < 5; i++)
                {
                    if (!dto.HasValue)
                        break;

                    nextFive.Add(dto.Value.ToLocalTime().ToString("ddd MMM dd, yyyy  H:mm:ss"));

                    dto = ce.GetTimeAfter(dto.Value);
                }
            }
            return nextFive;
        }

        #endregion

        #region events

        public void Scheduler_ShuttingDown(object sender)
        {
            Context.ShuttingDown = true;
        }

        #endregion

    }
}
