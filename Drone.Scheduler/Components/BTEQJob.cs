using System;
using System.Diagnostics;
using System.IO;
using Drone.Core;
using Drone.Shared;
using Quartz;
using System.Threading;
using System.Net.Mail;
using System.Text;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Drone.Scheduler.Components
{
    [DisallowConcurrentExecution]
    public class BTEQJob : BaseJob
    {
        private String BTEQ = "bteq.exe"; //okay if its in the path
        private Process BTEQProcess;

        public override void Execute(IJobExecutionContext context)
        {
            try
            {

                JobKey jobKey = context.JobDetail.Key;
                JobDataMap data = context.Trigger.JobDataMap;

                MyJob = data.Get(BaseJob.JOB_OBJ) as Job;

                if (!Object.Equals(null, MyJob))
                {
                    SetupFilesAndDirectories();

                    RunScript();

                    ExceptionExtensions.LogInformation("BTEQJob.Execute()", string.Format("BTEQJob: {0} executing at {1}\n  workingdir is {2}\n  bteqfilename is {3}\n ",
                                                                        context.JobDetail.Key,
                                                                        DateTime.Now.ToString("r"),
                                                                        WorkingDirectory,
                                                                        ScriptFileName));
                }
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "BTEQJob.Execute()", string.Format("WorkingDir: {0}, Script: {1}", WorkingDirectory, ScriptFileName));
            }
        }

        /// <summary>
        /// Initiate BTEQ process and execute script
        /// </summary>
        public void RunScript()
        {
            try
            {
                File.Delete(OutputLogFileName);
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "BTEQJob.RunScript().FileDelete", string.Format("OutputLogFileName: {0}", OutputLogFileName));
            }

            try
            {
                BTEQProcess = new Process();
                BTEQProcess.StartInfo.FileName = BTEQ;
                BTEQProcess.StartInfo.CreateNoWindow = true;
                BTEQProcess.StartInfo.UseShellExecute = false;
                BTEQProcess.StartInfo.WorkingDirectory = WorkingDirectory;
                BTEQProcess.StartInfo.RedirectStandardInput = true;
                BTEQProcess.StartInfo.RedirectStandardOutput = true;
                BTEQProcess.StartInfo.RedirectStandardError = true;
                // Handle process events
                BTEQProcess.EnableRaisingEvents = true;
                BTEQProcess.OutputDataReceived += new DataReceivedEventHandler(BTEQProcess_OutputDataReceived);
                BTEQProcess.ErrorDataReceived += new DataReceivedEventHandler(BTEQProcess_OutputErrorDataReceived);
                BTEQProcess.Exited += new EventHandler(BTEQProcess_Exited);
                // Start
                BTEQProcess.Start();
                BTEQProcess.BeginOutputReadLine();
                BTEQProcess.BeginErrorReadLine();
                using (StreamReader sr = new StreamReader(Path.Combine(WorkingDirectory, MyJob.ScriptFileName)))
                {
                    BTEQProcess.StandardInput.WriteLine(sr.ReadToEnd()); sr.Close();
                }

                BTEQProcess.WaitForExit();
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "BTEQJob.RunScript()", string.Format("WorkingDir: {0}, Script: {1}", WorkingDirectory, ScriptFileName));
            }
        }

        #region events

        private void BTEQProcess_Exited(object sender, EventArgs e)
        {
            MyJob.Schedule[JobSchedule.LastRunExitCode.ToString()] = BTEQProcess.ExitCode.ToString();

            if (BTEQProcess.ExitCode != 0)
            {
                StringBuilder sbBody = new StringBuilder();
                sbBody.AppendFormat("<br /><br/><b>ScriptFile:</b> {1}<br /><br/>", "Drone.Scheduler", ScriptFileName);
                sbBody.AppendFormat("<b>Error Log File:</b> {0}<br/><br/>", ErrLogFileName);
                sbBody.AppendFormat("<b>Script Output File:</b> {0}<br/><br/>", OutputLogFileName);
                sbBody.AppendFormat("<b>Script Exit Code: {0}</b> ", BTEQProcess.ExitCode);

                ExceptionExtensions.LogError(new Exception()
                                            , "BTEQJob.BTEQProcess_Exited()"
                                            , "Drone Scheduler"
                                            , sbBody.ToString());
            }
        }

        private void BTEQProcess_OutputDataReceived(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
                Utility.WriteToLogFile(OutputLogFileName, outLine.Data);

        }

        private void BTEQProcess_OutputErrorDataReceived(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
                Utility.WriteToLogFile(outLine.Data.ToLower().Contains("warning") ? OutputLogFileName : ErrLogFileName, string.Format("[{0}][{1}]", DateTime.Now, outLine.Data)); //will append by default
        }


        #endregion
    }
}