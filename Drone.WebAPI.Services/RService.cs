using Drone.Shared;
using Drone.WebAPI.Interfaces;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace Drone.WebAPI.Services
{
    public class RService : IRService
    {
        public XElement Get(string id)
        {
            string stdout = string.Empty;
            string stderr = string.Empty;
            XDocument doc = XDocument.Parse("<root>none</root>");

            try
            {
                Process process = new Process();
                process.StartInfo.LoadUserProfile = true;
                process.StartInfo.FileName = Conversions.ConvertTo(ConfigurationManager.AppSettings["RPath"], string.Empty);
                process.StartInfo.Arguments = "--vanilla --slave  < " + Path.Combine(Conversions.ConvertTo(ConfigurationManager.AppSettings["RScriptDir"], string.Empty), id);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WorkingDirectory = Conversions.ConvertTo(ConfigurationManager.AppSettings["RWorkingDir"], string.Empty);
                process.Start();

                stdout = process.StandardOutput.ReadToEnd();
                stderr = process.StandardError.ReadToEnd();

                process.WaitForExit();
                if (!String.IsNullOrEmpty(stdout))
                    doc = XDocument.Parse(stdout.Replace("NA", string.Empty));
                else
                    doc = XDocument.Parse(string.Format("<root><rerror><![CDATA[{0}]]></rerror></root>", stderr));
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Drone.WebAPI.RService.Get(id)", string.Format("script: [{0}]  stderr: [{1}] stdout: [{2}] stack: [{3}]", id, stderr, stdout, e.StackTrace));
                doc = XDocument.Parse(string.Format("<root><rerror>{1}</rerror><serviceexception>{0}</serviceexception></root>", e.Message, stderr));
            }

            return doc.Root;
        }

        public XElement Get(string id, string parms)
        {
            string stdout = string.Empty;
            string stderr = string.Empty;
            XDocument doc = XDocument.Parse("<root>none</root>");

            try
            {
                string[] args = parms.Split('|');
                if (args.Length > 0 && args[0] != "-1" && !String.IsNullOrWhiteSpace(args[0]))
                {
                    AdjustArguments(ref args);
                    //string[] finalArgs = GetConnectionLogin(args);
                    string[] finalArgs = args;

                    //todo: get the svc user account from the BigM1DMStaging DSN permissions on LogiReporting datbase.
                    //determine most flexible way of knowing if the user needs access to teradata, or sql. Could be diff users!
                    //separate RService call depending on db? but what if one R script needs to access both?
                    Process process = new Process();
                    process.StartInfo.LoadUserProfile = true;
                    process.StartInfo.FileName = Conversions.ConvertTo(ConfigurationManager.AppSettings["RPath"], string.Empty);
                    process.StartInfo.Arguments = "--vanilla --slave --args " + String.Join(" ", finalArgs) + " < " + Path.Combine(Conversions.ConvertTo(ConfigurationManager.AppSettings["RScriptDir"], string.Empty), id);
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.WorkingDirectory = Conversions.ConvertTo(ConfigurationManager.AppSettings["RWorkingDir"], string.Empty);
                    process.Start();

                    stdout = process.StandardOutput.ReadToEnd();
                    stderr = process.StandardError.ReadToEnd();

                    process.WaitForExit();
                    if (!String.IsNullOrEmpty(stdout))
                        doc = XDocument.Parse(stdout.Replace("NA", string.Empty));
                    else
                        doc = XDocument.Parse(string.Format("<root><rerror><![CDATA[{0}]]></rerror></root>", stderr));
                }
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Drone.WebAPI.RService.Get(id, parms)", string.Format("script: [{0}]  params: [{1}] stderr: [{2}] stdout: [{3}] stack: [{4}]", id, parms, stderr, stdout, e.StackTrace));
                doc = XDocument.Parse(string.Format("<root><rerror>{1}</rerror><serviceexception>{0}</serviceexception></root>", e.Message, stderr));
            }

            return doc.Root;
        }

        /// <summary>
        /// Tries to parse each argument as a date. If it passes, its formatted correctly for R.  
        /// Otherwise it checks for space sand puts quotes as needed.
        /// </summary>
        /// <param name="args"></param>
        private void AdjustArguments(ref string[] args)
        {
            DateTime outDate = DateTime.MinValue;

            for (int i = 0; i < args.Length; i++)
            {
                if (DateTime.TryParse(args[i], out outDate))
                {
                    args[i] = string.Format("\"{0}\"", outDate.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else if (args[i].Contains(" "))
                {
                    args[i] = string.Format("\"{0}\"", args[i]);
                }
            }
        }

        /// <summary>
        /// Get the login information from Nimitz and update args for R
        /// </summary>
        private string[] GetConnectionLogin(string[] args)
        {
            netConnect.Info info = new netConnect.Info();
            string DSNName = ConfigurationManager.AppSettings["DBServer"];
            string setting = ConfigurationManager.AppSettings["Environment"];
            string sAppName = ConfigurationManager.AppSettings["NetConnect.ApplicationName"];
            string sCertName = ConfigurationManager.AppSettings[setting + ".NetConnect.CertificateName"];  //ConfigurationManager.AppSettings[setting + "NetConnect." + DSNName + ".CertificateName"];

            SqlConnectionStringBuilder conBuilder = new SqlConnectionStringBuilder(info.Get(DSNName
                                                                                        , sAppName
                                                                                        , sCertName
                                                                                        , netConnect.ConnectTypeEnum.CONNECT_TYPE_NET));

            string[] finalArgs = new string[args.Length + 2];
            finalArgs[0] = conBuilder.UserID;
            finalArgs[1] = conBuilder.Password;
            Array.Copy(args, 0, finalArgs, 2, args.Length);

            return finalArgs;
        }
    }
}