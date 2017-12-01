using System;
using Drone.Shared.LoggingService;

namespace Drone.Shared
{
    public static class ExceptionExtensions
    {
        public static BIException ConvertToBIException(this Exception ex, LogActionEnum logAction, LogTypeEnum logType, string title, string sourceMethod, string user, string server, string additionalMessage = null)
        {
            return new BIException { ApplicationName = Utility.ApplicationName, LogAction = logAction, LogType = logType, Message = ex.Message + " - Additional Info: " + additionalMessage, StackTrace = ex.StackTrace, Title = title, User = user, Server = server, Source = sourceMethod };
        }

        public static BIException ConvertToBIException(this Exception ex, LogActionEnum logAction, LogTypeEnum logType, string title, string sourceMethod, string user, string server, string applicationName, string additionalMessage = null)
        {
            return new BIException { ApplicationName = applicationName, LogAction = logAction, LogType = logType, Message = ex.Message + " - Additional Info: " + additionalMessage, StackTrace = ex.StackTrace, Title = title, User = user, Server = server, Source = sourceMethod };
        }

        /// <summary>
        /// Log an error to the service.  
        /// This method will log to the BI Logging Service unless a log error file is specified in the config
        /// </summary>
        /// <param name="e"></param>
        /// <param name="method">Method where the exception occured</param>
        /// <param name="additionalMessage"></param>
        public static void LogError(Exception e, string method, string additionalMessage = null)
        {
            LogException(e, method, LogTypeEnum.Error, Utility.ErrorLogFile, additionalMessage);
        }

        /// <summary>
        /// Log an error to the service.  
        /// This method forces the error to be logged to the BI Logging Service even if a log file is supplied in the config
        /// </summary>
        /// <param name="e"></param>
        /// <param name="method">Method where the exception occured</param>
        /// <param name="applicationName">Application Name that matches the logging database</param>
        /// <param name="additionalMessage"></param>
        public static void LogError(Exception e, string method, string applicationName, string additionalMessage = null)
        {
            using (var logClient = new LoggingService.BILoggerServiceClient())
            {
                logClient.HandleBIExceptionAsync(e.ConvertToBIException(LogActionEnum.LogAndEmail
                                                                                , LogTypeEnum.Error
                                                                                , method + " Error"
                                                                                , method
                                                                                , "nouser"
                                                                                , System.Environment.MachineName
                                                                                , applicationName
                                                                                , additionalMessage));
            }
        }

        public static void LogWarning(Exception e, string sourceMethod, string additionalMessage = null)
        {
            LogException(e, sourceMethod, LogTypeEnum.Warning, Utility.WarningLogFile, additionalMessage);
        }

        public static void LogInformation(string sourceMethod, string additionalMessage)
        {
            LogException(new Exception("Drone Processor Information"), sourceMethod, LogTypeEnum.Information, Utility.InfoLogFile, additionalMessage);
        }

        private static void LogException(Exception e, string sourceMethod, LogTypeEnum type, string logFile, string additionalMessage = null)
        {
            if (!String.IsNullOrEmpty(logFile))
            {
                Utility.WriteToLogFile(logFile, string.Format("[{0}][{1}][{2}][{3}][{4}][{5}][{6}]"
                                                                                                                                    , DateTime.Now.ToString()
                                                                                                                                    , type
                                                                                                                                    , sourceMethod
                                                                                                                                    , "nouser"
                                                                                                                                    , System.Environment.MachineName
                                                                                                                                    , e.Message
                                                                                                                                    , additionalMessage));
            }
            else
            {
                using (var logClient = new LoggingService.BILoggerServiceClient())
                {
                    logClient.HandleBIExceptionAsync(e.ConvertToBIException(LogActionEnum.LogAndEmail
                                                                                    , type
                                                                                    , sourceMethod + " Error"
                                                                                    , sourceMethod
                                                                                    , "nouser"
                                                                                    , System.Environment.MachineName
                                                                                    , additionalMessage));
                }
            }
        }
    }
}
