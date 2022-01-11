using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using Theresa3rd_Bot.Model.Error;

namespace Theresa3rd_Bot.Util
{
    public static class LogHelper
    {
        private static readonly string RepositoryName = "NETCoreRepository";
        private static readonly string ConfigFile = "log4net.config";
        private static List<SendError> SendErrorList = new List<SendError>();

        private static ILog RollingLog { get; set; }
        private static ILog ConsoleLog { get; set; }
        private static ILog FileLog { get; set; }
        private static ILoggerRepository repository { get; set; }

        public static void ConfigureLog()
        {
            repository = LogManager.CreateRepository(RepositoryName);
            XmlConfigurator.Configure(repository, new FileInfo(ConfigFile));
            RollingLog = LogManager.GetLogger(RepositoryName, "RollingLog");
            ConsoleLog = LogManager.GetLogger(RepositoryName, "ConsoleLog");
            FileLog = LogManager.GetLogger(RepositoryName, "FileLog");
        }

        public static void Info(object message)
        {
            FileLog.Info(message);
            ConsoleLog.Info(message);
        }

        public static void Error(Exception ex)
        {
            string logMsg = $"[Message]{ex.Message}\r\n[StackTrace]\r\n{ex.StackTrace}";
            RollingLog.Error(logMsg, ex);
            ConsoleLog.Error(logMsg, ex);
        }

        public static void Error(Exception ex, string message)
        {
            string logMsg = $"{message}:\r\n[Message]{ex.Message}\r\n[StackTrace]\r\n{ex.StackTrace}";
            RollingLog.Error(logMsg, ex);
            ConsoleLog.Error(logMsg, ex);
        }

        public static void sendError(string title,string message)
        {
            try
            {

            }
            catch (Exception)
            {

            }
        }

    }
}
