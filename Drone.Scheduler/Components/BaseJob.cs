using Drone.Shared;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace Drone.Scheduler.Components
{
    public abstract class BaseJob : IJob
    {
        public const string JOB_OBJ = "jobobject";
        public Job MyJob { get; set; }
        public String OutputLogFileName = String.Empty;
        public String ErrLogFileName = String.Empty;
        public String WarningLogFileName = String.Empty;
        public string WorkingDirectory = String.Empty;
        public String ScriptFileName = String.Empty;
        public const string SCHED_PATH = @"\\jomax.paholdings.com\data\busintel\scheduler";//todo: config this

        public abstract void Execute(IJobExecutionContext context);

        public void SetupFilesAndDirectories()
        {

            ScriptFileName = Path.GetFileName(MyJob.ScriptFileName);
            WorkingDirectory = Path.Combine(SCHED_PATH, MyJob.ProjectName);

            try
            {
                //set security level to everyone
                DirectorySecurity ds = new DirectorySecurity();
                ds.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));

                //create/empty export folder
                if (!Directory.Exists(Path.Combine(WorkingDirectory, "Export")))
                    Directory.CreateDirectory(Path.Combine(WorkingDirectory, "Export"), ds);
                else
                    Array.ForEach(Directory.GetFiles(Path.Combine(WorkingDirectory, "Export")), File.Delete);

                //create/empty logs folder
                if (!Directory.Exists(Path.Combine(WorkingDirectory, "Logs")))
                    Directory.CreateDirectory(Path.Combine(WorkingDirectory, "Logs"), ds);
                else
                    Array.ForEach(Directory.GetFiles(Path.Combine(WorkingDirectory, "Logs")), File.Delete);


                ErrLogFileName = Path.Combine(WorkingDirectory, "logs", string.Format("{0}_{1}.log", Path.GetFileNameWithoutExtension(ScriptFileName), "error"));
                WarningLogFileName = Path.Combine(WorkingDirectory, "logs", string.Format("{0}_{1}.log", Path.GetFileNameWithoutExtension(ScriptFileName), "warning"));
                OutputLogFileName = Path.Combine(WorkingDirectory, "logs", string.Format("{0}_{1}.log", Path.GetFileNameWithoutExtension(ScriptFileName), "output"));

            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "BaseJob.SetupFilesAndDirectories()"
                                                , string.Format("Script:{0}, WorkingDir: {1}, ErrorLog:{2}, WarningLog: {3}, Output:{4}"
                                                                , ScriptFileName
                                                                , WorkingDirectory
                                                                , ErrLogFileName
                                                                , WarningLogFileName
                                                                , OutputLogFileName));
            }
        }
    }
}
