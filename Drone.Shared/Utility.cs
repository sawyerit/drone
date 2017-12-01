using System.Configuration;
using System.IO;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using Quartz;

namespace Drone.Shared
{
    public static class Utility
    {
        private static object _syncLock = new object();
        public static Dictionary<string, LockItem> lockList = new Dictionary<string, LockItem>();

        public static string ApplicationName
        {
            get { return Conversions.ConvertTo(ConfigurationManager.AppSettings["NetConnect.ApplicationName"], "Drone Processor"); }
        }

        public static string ErrorLogFile
        {
            get
            {
                string logFile = Conversions.ConvertTo(ConfigurationManager.AppSettings["LogFile.Error"], string.Empty);
                if (!string.IsNullOrEmpty(logFile))
                {
                    logFile = string.Format(logFile, DateTime.Today);
                }
                return logFile;
            }
        }

        public static string WarningLogFile
        {
            get
            {
                string logFile = Conversions.ConvertTo(ConfigurationManager.AppSettings["LogFile.Warning"], string.Empty);
                if (!string.IsNullOrEmpty(logFile))
                {
                    logFile = string.Format(logFile, DateTime.Today);
                }
                return logFile;
            }
        }

        public static string InfoLogFile
        {
            get
            {
                string logFile = Conversions.ConvertTo(ConfigurationManager.AppSettings["LogFile.Info"], string.Empty);
                if (!string.IsNullOrEmpty(logFile))
                {
                    logFile = string.Format(logFile, DateTime.Today);
                }
                return logFile;
            }
        }

        private static string _componentBaseFolder = string.Empty;

        public static string ComponentBaseFolder
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_componentBaseFolder))
                    ComponentBaseFolder = AppDomain.CurrentDomain.BaseDirectory;

                return _componentBaseFolder;
            }
            set { _componentBaseFolder = value; }
        }

        private static string _logLocation
        {
            get
            {
                string folder = Utility.ComponentBaseFolder + "\\Logs";

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                return folder;
            }
        }

        private static int CleanLogsAfterDays
        {
            get
            {
                return Conversions.ConvertTo(ConfigurationManager.AppSettings["CleanLogsAfterDays"], 3);
            }
        }

        private static bool AutoCleanInactiveLogs
        {
            get
            {
                return Conversions.ConvertTo(ConfigurationManager.AppSettings["AutoCleanInactiveLogs"], false);
            }
        }

        private static DateTime _lastLogClean = DateTime.MinValue;

        public static bool IsCriticalDBError(Exception e)
        {
            bool isCrit = false;
            if (e.Message.ToLowerInvariant().Contains("tempdb"))
            {
                isCrit = true;
            }
            return isCrit;
        }

        public static void WriteToLogFile(string fileName, string message)
        {
            WriteToLogFile(fileName, message, true);
        }

        public static void WriteToLogFile(string fileName, string message, bool append)
        {
            string logFile = Path.Combine(_logLocation, fileName);

            //Get lock for the file we are writing to, if it doesn't exist, create it
            lock (_syncLock)
            {
                if (!lockList.ContainsKey(fileName))
                    lockList[fileName] = new LockItem { Name = fileName, TimeStamp = DateTime.Now };
            }

            //write to file
            try
            {
                lock (lockList[fileName])
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(logFile, append))
                        file.WriteLine(message);
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "WriteToLogFile", "File: " + fileName);
            }

            //clean once an hour
            if (_lastLogClean <= DateTime.Now.AddHours(-1))
                RemoveOldLogs();

        }

        public static void RemoveOldLogs()
        {
            //clean lock list
            lock (_syncLock)
                foreach (var s in lockList.Where(p => p.Value.TimeStamp < DateTime.Now.AddDays(-CleanLogsAfterDays)).ToList())
                    lockList.Remove(s.Key);

            //clean files
            try
            {
                string[] files = System.IO.Directory.GetFiles(_logLocation);
                foreach (string item in files)
                {
                    if (File.GetCreationTime(Path.Combine(_logLocation, item)) < DateTime.Now.AddDays(-CleanLogsAfterDays))
                        File.Delete(Path.Combine(_logLocation, item));
                }
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogWarning(e, "Utility.RemoveOldLogs()");
            }

            _lastLogClean = DateTime.Now;
        }

        public static bool FileExists(string fileName)
        {
            return File.Exists(Path.Combine(_logLocation, fileName));
        }

        public static void WriteToLogFile(string fileName, string[] message)
        {
            string logFile = Path.Combine(_logLocation, fileName);
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(logFile, true))
                {
                    for (int i = 0; i < message.Length; i++)
                    {
                        if (i < message.Length - 1)
                            file.WriteLine(message[i]);
                        else
                            file.Write(message[i]);
                    }
                }
            }
            catch (Exception) { }
        }

        public static string ReadFirstLineFromFile(string fileName, bool delete)
        {
            string retVal = string.Empty;

            try
            {
                string file = Path.Combine(_logLocation, fileName);
                var lines = File.ReadAllLines(file);

                if (lines.Length > 0 && !String.IsNullOrWhiteSpace(lines[0]))
                {
                    retVal = lines.FirstOrDefault();

                    if (delete && !string.IsNullOrWhiteSpace(retVal))
                        File.WriteAllLines(file, lines.Skip(1));
                }
                else
                {
                    File.Delete(file);
                }
            }
            catch (Exception) { }

            return retVal;
        }

        public static List<string> ReadLinesFromFile(string fileName, int numLines, bool delete)
        {
            List<string> retVal = new List<string>();

            try
            {
                string file = Path.Combine(_logLocation, fileName);
                var lines = File.ReadAllLines(file);

                //get specified num of lines or the rest, whichever is smaller
                if (lines.Length > numLines)
                    retVal = lines.Take(numLines).ToList();
                else
                    retVal = lines.ToList();

                //delete the taken lines and write out the file
                if (lines.Length > 0)
                {
                    if (delete && retVal.Count() > 0)
                        File.WriteAllLines(file, lines.Skip(retVal.Count()));
                }
                else
                {
                    File.Delete(file);
                }
            }
            catch (Exception) { }

            return retVal;
        }

        public static string CleanUrl(string url)
        {
            string cleanUrl = string.Empty;
            cleanUrl = url.Replace("http://", "").Replace("https://", "").Replace("htttp://", "").Replace("www.", "");

            if (cleanUrl.Count(z => z == '.') > 1)
            {
                if (cleanUrl.StartsWith("info.") || cleanUrl.StartsWith("case.") || cleanUrl.StartsWith("beta.") || cleanUrl.StartsWith("blog."))
                    cleanUrl = cleanUrl.Remove(0, 5);

                if (cleanUrl.StartsWith("about."))
                    cleanUrl = cleanUrl.Remove(0, 6);

                if (cleanUrl.StartsWith("global."))
                    cleanUrl = cleanUrl.Remove(0, 7);

                if (cleanUrl.StartsWith("ir."))
                    cleanUrl = cleanUrl.Remove(0, 3);
            }

            if (cleanUrl.Contains("/"))
                cleanUrl = cleanUrl.Remove(cleanUrl.IndexOf('/'), cleanUrl.Length - cleanUrl.IndexOf('/'));

            return cleanUrl;
        }

        public static void AddLineToFile(string fileName, string value)
        {
            try
            {
                string file = Path.Combine(_logLocation, fileName);
                string[] lines = File.ReadAllLines(file);
                if (lines.Length > 0)
                {
                    if (lines[lines.Length - 1] == string.Empty)
                    {
                        lines[lines.Length - 1] = value + Environment.NewLine;
                    }
                    else
                    {
                        List<string> lineList = lines.ToList();
                        lineList.Add(value + Environment.NewLine);
                        lines = lineList.ToArray();
                    }
                }
                File.WriteAllLines(file, lines);
            }
            catch (Exception) { }
        }

        public static string SerializeToXMLString<T>(object serialize)
        {
            //let any errors bubble up to the caller
            //try
            //{
            StringWriter stringWriter = new StringWriter();
            XmlSerializer xs = new XmlSerializer(typeof(T));
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.OmitXmlDeclaration = true;
            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.Indent = false;
            xmlSettings.CloseOutput = true;

            XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlSettings);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            namespaces.Add(String.Empty, String.Empty);
            xs.Serialize(xmlWriter, serialize, namespaces);

            return stringWriter.ToString();
            //}
            //catch (Exception e)
            //{
            //    //todo: add typeof(T).tostring here so we know whats failing?
            //    ExceptionExtensions.LogError(e, "Utility.SerializeToXMLString<T>()", "type: " + typeof(T).ToString());
            //    return String.Empty;
            //}

        }

        public static T DeserializeXMLString<T>(string deserialize)
        {
            try
            {
                T returnValue = default(T);
                XmlSerializer xs = new XmlSerializer(typeof(T));
                StringReader stringReader = new StringReader(deserialize);
                XmlTextReader xmlReader = null;

                try
                {
                    xmlReader = new XmlTextReader(stringReader);
                }
                catch
                {
                    if (stringReader != null) stringReader.Dispose();
                }

                using (xmlReader)
                {
                    object obj = xs.Deserialize(xmlReader);
                    returnValue = Conversions.ConvertTo(obj, default(T));
                }

                return returnValue;
            }

            catch (Exception)
            {
                return default(T);
            }

        }

        public static DateTime GetNextRunFromCron(string cronX)
        {
            if (!String.IsNullOrWhiteSpace(cronX))
            {
                CronExpression ce = new CronExpression(cronX);
                DateTimeOffset? next = ce.GetTimeAfter(new DateTimeOffset(DateTime.Now));

                if (next.HasValue)
                    return next.Value.ToLocalTime().DateTime;
            }
            return DateTime.MinValue;
        }

    }

    public class LockItem
    {
        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
