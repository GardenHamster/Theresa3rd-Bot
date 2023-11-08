using log4net;
using log4net.Config;
using log4net.Repository;

namespace TheresaBot.Main.Helper
{
    public static class LogHelper
    {
        private static readonly string RepositoryName = "NETCoreRepository";
        private static ILog RollingLog { get; set; }
        private static ILog ConsoleLog { get; set; }
        private static ILog FileLog { get; set; }
        private static ILoggerRepository repository { get; set; }

        /// <summary>
        /// 初始化日志
        /// </summary>
        public static void ConfigureLog()
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, "log4net.config");
            repository = LogManager.CreateRepository(RepositoryName);
            XmlConfigurator.Configure(repository, new FileInfo(configPath));
            RollingLog = LogManager.GetLogger(RepositoryName, "RollingLog");
            ConsoleLog = LogManager.GetLogger(RepositoryName, "ConsoleLog");
            FileLog = LogManager.GetLogger(RepositoryName, "FileLog");
        }

        /// <summary>
        /// 输出日志到控制台
        /// </summary>
        /// <param name="message"></param>
        public static void OutputConsole(string message)
        {
            ConsoleLog.Info(message);
        }

        /// <summary>
        /// 记录Info级别的日志
        /// </summary>
        /// <param name="message"></param>
        public static void Info(object message)
        {
            FileLog.Info(message);
            ConsoleLog.Info(message);
        }

        /// <summary>
        /// 记录Error级别的日志
        /// </summary>
        /// <param name="ex"></param>
        public static void Error(Exception ex)
        {
            ConsoleLog.Error(ex.Message);
            RollingLog.Error("", ex);
        }

        /// <summary>
        /// 记录Error级别的日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public static void Error(Exception ex, string message)
        {
            ConsoleLog.Error($"{message}：{ex.Message}");
            RollingLog.Error(message, ex);
        }

        /// <summary>
        /// 记录FATAL级别的日志
        /// </summary>
        /// <param name="message"></param>
        public static void FATAL(string message)
        {
            FileLog.Info(message);
            ConsoleLog.Fatal(message);
            RollingLog.Fatal(message);
        }

        /// <summary>
        /// 记录FATAL级别的日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public static void FATAL(Exception ex, string message)
        {
            FileLog.Info(message);
            ConsoleLog.Fatal(message, ex);
            RollingLog.Fatal(message, ex);
        }

    }
}
