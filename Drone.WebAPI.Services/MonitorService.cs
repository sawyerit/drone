using Drone.Data.Queue;
using Drone.Entities.WebAPI;
using Drone.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Xml;

namespace Drone.WebAPI.Services
{
    /// <summary>
    /// Monitors all running windows services defined in the XML.
    /// Does not implement IXXXService because its called via the hubs rather than WebAPI controller
    /// No need to instantiate DI (or IoC) on this component.
    /// </summary>
    public class MonitorService
    {
        private static DateTime lastRestart;

        public static Status GetStatus()
        {
            Status s = new Status();
            try
            {
                s.ServiceBoxName = System.Environment.MachineName;
                
                foreach (string serv in GetServicesFromXML())
                {
                    //temp until both servers are called the same or svc boxes are cutoff
                    if (serv == "CrawlDaddy" && s.ServiceBoxName.ToLower().Contains("svc"))
                        s.DroneServices.Add(new DroneService { ServiceName = serv, Status = GetServiceStatus(serv), HasError = CheckErrors(serv) });
                    else
                        s.DroneServices.Add(new DroneService { ServiceName = "Drone." + serv + ".Service", Status = GetServiceStatus("Drone." + serv + ".Service"), HasError = CheckErrors(serv) });
                }

                s.Queue = GetQueueInfo();
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Drone.WebApi.MonitorService.GetStatus()");
            }
            return s;
        }

        private static List<string> GetServicesFromXML()
        {

            List<string> winServices = new List<string>();

            XmlDocument xmlDoc = new XmlDocument();
            string sXMLPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_data", "services.xml");

            try
            {
                xmlDoc.Load(sXMLPath);
                XmlNodeList xnList = xmlDoc.SelectNodes("services/service");
                
                foreach (XmlNode node in xnList)
                {
                    winServices.Add(node.InnerText);
                }
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Drone.WebAPI.Services.MonitorService.GetServicesFromXML()", "Path: " + sXMLPath);
            }
            return winServices;
        }


        private static QueueInfo GetQueueInfo()
        {
            QueueInfo qi = new QueueInfo();

            qi.QueueName = "Drone Queue";
            qi.ServerName = System.Environment.MachineName;
            qi.QueueCount = QueueManager.Instance.GetMessageCount();

            try
            {
                string[] files = System.IO.Directory.GetFiles(ConfigurationManager.AppSettings["APILogPath"], "Errors_*.log", System.IO.SearchOption.TopDirectoryOnly);
                qi.HasErrors = files.Length > 0;
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Drone.WebAPI.Services.MonitorService GetQueueInfo()", "Ensure APILogPath is set in the configuration file");
            }

            return qi;
        }

        public static string GetServiceStatus(string serviceName)
        {
            string retVal = string.Empty;

            try
            {
                ServiceController service = new ServiceController(serviceName);
                retVal = service.Status.ToString();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("not found"))
                    retVal = "NotFound";
                else
                    ExceptionExtensions.LogError(e, "Drone.WebApi.MonitorService.GetServiceStatus");
            }

            return retVal;
        }

        public static void RunCommand(string command, string service)
        {
            try
            {
                ServiceController controller = new ServiceController(service);
                if (!Object.Equals(null, controller))
                {
                    if (command.ToLower() == "restart")
                    {
                        RunCommand("stop", service);
                        RunCommand("start", service);
                    }
                    else if (command.ToLower() == "stop")
                    {
                        if (controller.Status != ServiceControllerStatus.Stopped && controller.Status != ServiceControllerStatus.StopPending)
                        {
                            WindowsServiceHelper.ChangeStartMode(controller, ServiceStartMode.Disabled);
                            controller.Stop();
                        }

                        controller.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                    else
                    {
                        if (controller.Status != ServiceControllerStatus.Running && controller.Status != ServiceControllerStatus.StartPending)
                        {
                            WindowsServiceHelper.ChangeStartMode(controller, ServiceStartMode.Automatic);
                            controller.Start();
                        }
                        controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    }
                }
                else
                {
                    ExceptionExtensions.LogInformation("Drone.Monitor.Service.Business.StatusManager RunCommand", "Controller is NULL. ServiceName: " + service);
                }
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogWarning(e, "Drone.Monitor.Service.Business.StatusManager RunCommand", "ServiceName: " + service);
            }
        }

        public static bool CheckErrors(string service)
        {
            string logFolder = string.Format(ConfigurationManager.AppSettings["ServiceLogPath"], service);

            if (Directory.Exists(logFolder))
            {
                List<string> files = System.IO.Directory.GetFiles(logFolder, "Errors_*.log", System.IO.SearchOption.TopDirectoryOnly).ToList();

                if (service == "CrawlDaddy")
                {
                    string splunkFile = logFolder + "\\SplunkLog.txt";

                    if (File.Exists(splunkFile) && GetServiceStatus(service) == "Running" && GetLastWrite(splunkFile) < DateTime.Now.AddMinutes(-5))
                    {
                        if (lastRestart < DateTime.Now.AddMinutes(-5))
                        {
                            ExceptionExtensions.LogWarning(new Exception(service + " service was found stalled."), "Drone.Monitor.Service.Business.StatusManager CheckErrors", "Attempting to restart");
                            lastRestart = DateTime.Now;
                            RunCommand("restart", service);

                            if (GetServiceStatus(service) != ServiceControllerStatus.Running.ToString())
                            {
                                files.Add(splunkFile);
                            }
                        }
                        else
                        {
                            files.Add(splunkFile);
                        }
                    }
                }
                return files.Count > 0;
            }

            return false;
        }

        private static DateTime GetLastWrite(string splunkFile)
        {
            DateTime retVal = DateTime.MinValue;
            var fs = new FileStream(splunkFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var lastLine = string.Empty;
            var line = string.Empty;
            using (var sr = new StreamReader(fs))
                while ((line = sr.ReadLine()) != null)
                    lastLine = line;

            string[] lastLineSplit = lastLine.Split(',');
            if (lastLineSplit.Length == 2)
            {
                retVal = DateTime.ParseExact(lastLineSplit[0], "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            return retVal;
        }
    }
}
