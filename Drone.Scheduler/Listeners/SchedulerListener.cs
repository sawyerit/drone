using System;
//using Common.Logging;
using Quartz;
using Drone.Shared;

namespace Drone.Scheduler.Listeners
{
    public class SchedulerListener : ISchedulerListener
    {
        public void JobAdded(IJobDetail jobDetail)
        {
            WriteToLog(string.Format("[{0}][INFO][Job Added][Job Key: {1}]", DateTime.Now.ToString(), jobDetail.Key.Name));
        }

        public void JobDeleted(JobKey jobKey)
        {
            WriteToLog(string.Format("[{0}][INFO][Job Deleted][Job Key: {1}]", DateTime.Now.ToString(), jobKey.Name));
        }

        public void JobPaused(JobKey jobKey)
        {
            WriteToLog(string.Format("[{0}][INFO][Job Paused][Job Key: {1}]", DateTime.Now.ToString(), jobKey.Name));
        }

        public void JobResumed(JobKey jobKey)
        {
            WriteToLog(string.Format("[{0}][INFO][Job Resumed][Job Key: {1}]", DateTime.Now.ToString(), jobKey.Name));
        }

        public void JobScheduled(ITrigger trigger)
        {
            WriteToLog(string.Format("[{0}][INFO][Job Scheduled][Job Key: {1}][Trigger: {2}]", DateTime.Now.ToString(), trigger.JobKey.Name, trigger.Key.Name));            
        }

        public void JobUnscheduled(TriggerKey triggerKey)
        {
            WriteToLog(string.Format("[{0}][INFO][Job Unscheduled][Trigger: {1}]", DateTime.Now.ToString(), triggerKey.Name));            
        }

        public void JobsPaused(string jobGroup)
        {
            WriteToLog(string.Format("[{0}][INFO][Jobs Paused][Group: {1}]", DateTime.Now.ToString(), jobGroup)); 
        }

        public void JobsResumed(string jobGroup)
        {
            WriteToLog(string.Format("[{0}][INFO][Jobs Resumed][Group: {1}]", DateTime.Now.ToString(), jobGroup)); 
        }

        public void SchedulerError(string msg, SchedulerException cause)
        {
            ExceptionExtensions.LogError(cause.InnerException, "SchedulerListener.SchedulerError()", msg);
            WriteToLog(string.Format("[{0}][ERROR][Scheduler Exception][Error: {1}][Message: {2}]", DateTime.Now.ToString(), cause.Message, msg), "ERROR");
        }

        public void SchedulerInStandbyMode()
        {
            WriteToLog(string.Format("[{0}][INFO][Scheduler StandBy]", DateTime.Now.ToString())); 
        }

        public void SchedulerShutdown()
        {
            WriteToLog(string.Format("[{0}][INFO][Scheduler Shut Down]", DateTime.Now.ToString())); 
        }

        public void SchedulerShuttingdown()
        {
            WriteToLog(string.Format("[{0}][INFO][Scheduler Shutting Down]", DateTime.Now.ToString())); 
        }

        public void SchedulerStarted()
        {
            WriteToLog(string.Format("[{0}][INFO][Scheduler Started]", DateTime.Now.ToString())); 
        }

        public void SchedulerStarting()
        {
            WriteToLog(string.Format("[{0}][INFO][Scheduler Starting]", DateTime.Now.ToString())); 
        }

        public void SchedulingDataCleared()
        {
            WriteToLog(string.Format("[{0}][INFO][Scheduler Data Cleared]", DateTime.Now.ToString())); 
        }

        public void TriggerFinalized(ITrigger trigger)
        {
            WriteToLog(string.Format("[{0}][INFO][Trigger Finalized][Job Key: {1}][Trigger: {2}]", DateTime.Now.ToString(), trigger.JobKey.Name, trigger.Key.Name));
        }

        public void TriggerPaused(TriggerKey triggerKey)
        {
            WriteToLog(string.Format("[{0}][INFO][Trigger Paused][Job Key: {1}][Trigger: {2}]", DateTime.Now.ToString(), triggerKey));
        }

        public void TriggerResumed(TriggerKey triggerKey)
        {
            WriteToLog(string.Format("[{0}][INFO][Trigger Resumed][Job Key: {1}][Trigger: {2}]", DateTime.Now.ToString(), triggerKey));
        }

        public void TriggersPaused(string triggerGroup)
        {
            WriteToLog(string.Format("[{0}][INFO][Triggers Paused][Group: {1}]", DateTime.Now.ToString(), triggerGroup));
        }

        public void TriggersResumed(string triggerGroup)
        {
            WriteToLog(string.Format("[{0}][INFO][Triggers Resumed][Group: {1}]", DateTime.Now.ToString(), triggerGroup));
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