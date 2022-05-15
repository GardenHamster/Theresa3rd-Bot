using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Error;

namespace Theresa3rd_Bot.Util
{
    public static class LogHelper
    {
        private static readonly string RepositoryName = "NETCoreRepository";
        private static readonly string ConfigFile = "log4net.config";

        private static int LastSendHour = DateTime.Now.Hour;
        private static List<SendError> SendErrorList = new List<SendError>();

        private static ILog RollingLog { get; set; }
        private static ILog ConsoleLog { get; set; }
        private static ILog FileLog { get; set; }
        private static ILoggerRepository repository { get; set; }

        /// <summary>
        /// 初始化日志
        /// </summary>
        public static void ConfigureLog()
        {
            repository = LogManager.CreateRepository(RepositoryName);
            XmlConfigurator.Configure(repository, new FileInfo(ConfigFile));
            RollingLog = LogManager.GetLogger(RepositoryName, "RollingLog");
            ConsoleLog = LogManager.GetLogger(RepositoryName, "ConsoleLog");
            FileLog = LogManager.GetLogger(RepositoryName, "FileLog");
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
            RollingLog.Error(GetDetailError(ex));
            SendError(ex);
        }

        /// <summary>
        /// 记录Error级别的日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public static void Error(Exception ex, string message)
        {
            ConsoleLog.Error($"{message}：{ex.Message}");
            RollingLog.Error(GetDetailError(ex, message));
            SendError(ex, message);
        }

        /// <summary>
        /// 记录FATAL级别的日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public static void FATAL(Exception ex, string message, bool sendError)
        {
            ConsoleLog.Error(ex.Message);
            RollingLog.Error(GetDetailError(ex, message));
            if (sendError) SendErrorAnyway(ex, message);
        }

        /// <summary>
        /// 获取子类型的错误日志消息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static Exception GetInnerException(Exception ex)
        {
            if (ex.InnerException != null) return ex.InnerException;
            return ex;
        }

        /// <summary>
        /// 获取详细的错误日志消息
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string GetDetailError(Exception ex, string message = "")
        {
            StringBuilder errorMsg = new StringBuilder();
            if (!string.IsNullOrEmpty(message)) errorMsg.AppendLine($"[message]{message}");
            errorMsg.AppendLine($"[Message]{ex.Message}");
            errorMsg.AppendLine($"[InnerMessage]{ex.InnerException?.Message}");
            errorMsg.AppendLine($"[InnerInnerMessage]{ex.InnerException?.InnerException?.Message}");
            errorMsg.AppendLine($"[StackTrace]{ex.StackTrace}");
            errorMsg.AppendLine($"[InnerStackTrace]{ex.InnerException?.StackTrace}");
            return errorMsg.ToString();
        }

        /// <summary>
        /// 将错误日志发送到日志群中
        /// 每个小时最多发送10种类型且每种类型最多不超过3次的日志
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public static void SendError(Exception exception, string message = "")
        {
            try
            {
                if (BotConfig.GeneralConfig?.ErrorGroups == null) return;
                if (IsSendError(exception) == false) return;

                if (string.IsNullOrWhiteSpace(message)) message = "未知错误";
                string sendMessage = $"{message}：{exception.Message}\r\n详细请查看Log日志";
                foreach (var groupId in BotConfig.GeneralConfig.ErrorGroups)
                {
                    MiraiHelper.Session.SendGroupMessageAsync(groupId, new Mirai.CSharp.HttpApi.Models.ChatMessages.PlainMessage(sendMessage));
                }

                AddSendError(exception);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        /// <summary>
        /// 将错误日志发送到日志群中，忽略发送限制
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public static void SendErrorAnyway(Exception exception, string message)
        {
            try
            {
                if (BotConfig.GeneralConfig?.ErrorGroups == null) return;
                string sendMessage = $"{message}\r\n{exception.Message}\r\n{exception.StackTrace}";
                foreach (var groupId in BotConfig.GeneralConfig.ErrorGroups)
                {
                    MiraiHelper.Session.SendGroupMessageAsync(groupId, new Mirai.CSharp.HttpApi.Models.ChatMessages.PlainMessage(sendMessage));
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        /// <summary>
        /// 判断这个小时能是否还可以发送日志
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static bool IsSendError(Exception exception)
        {
            if (LastSendHour != DateTime.Now.Hour) return true;
            if (SendErrorList.Count >= 10) return false;
            string message = exception.Message;
            SendError sendError = SendErrorList.Where(m => m.Message == message).FirstOrDefault();
            if (sendError == null) return true;
            return sendError.SendTimes < 3;
        }

        /// <summary>
        /// 添加发送记录
        /// </summary>
        /// <param name="exception"></param>
        private static void AddSendError(Exception exception)
        {
            if (LastSendHour != DateTime.Now.Hour) SendErrorList.Clear();

            LastSendHour = DateTime.Now.Hour;

            string message = exception.Message;
            SendError sendError = SendErrorList.Where(m => m.Message == message).FirstOrDefault();
            if (sendError == null)
            {
                SendErrorList.Add(new SendError(message));
            }
            else
            {
                sendError.SendTimes++;
                sendError.LastSendTime = DateTime.Now;
            }
        }

    }
}
