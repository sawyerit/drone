using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Scheduler
{
    public class Job
    {
        public ObjectId _id { get; set; }
        public string JobType { get; set; }
        public string ProjectName { get; set; }
        public string ScriptFileName { get; set; }
        public string LastModified { get; set; }
        public string IsActive { get; set; }
        public Dictionary<string, string> Schedule { get; set; }
        
        //{ 
            //_id : 'the mongo id'
            //, JobType : 'bteq' (or R)
            //, ProjectName : 'some proj name'
            //, ScriptFileName : 'script.bteq'
            //, LastModified : 'date last modified'
            //, IsActive : 'true'
            //, Schedule : {
                            //LastRunStart : 'time last run started',
                            //LastRunComplete : 'time last run completed',
                            //LastRunExitCode : 'ExitCode for last run',
                            //StartTime : 'initial time to start the job',
                            //Status : jobstatus enum,
                            //Cronx : "0 0/5 * 1/1 * ?"
                            //}
            //}
    }
    
    public enum JobStatus { Disabled, Ready, Running, Failed }
    public enum JobSchedule { StartTime, LastRunStart, LastRunComplete, LastRunExitCode, CronX, Status }
}
