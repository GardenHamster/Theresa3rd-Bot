using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Error;

namespace TheresaBot.Main.Helper
{
    public class ReportHelper
    {
        private const int childSendTimes = 3;
        private const int exceptionSendTimes = 10;
        private static int LastSendHour = DateTime.Now.Hour;
        private static Dictionary<System.Type, List<ErrorRecord>> SendDic = new Dictionary<System.Type, List<ErrorRecord>>();

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
                if (BotConfig.GeneralConfig?.ErrorGroups is null) return;
                if (IsSendError(exception) == false) return;
                StringBuilder messageBuilder = new StringBuilder();
                if (string.IsNullOrWhiteSpace(message) == false)
                {
                    messageBuilder.AppendLine(message);
                }
                if (string.IsNullOrWhiteSpace(exception.Message) == false)
                {
                    messageBuilder.AppendLine(exception.Message);
                }
                if (string.IsNullOrWhiteSpace(exception.InnerException?.Message) == false)
                {
                    messageBuilder.AppendLine(exception.InnerException.Message);
                }
                messageBuilder.Append("详细请查看Log日志");
                foreach (var groupId in BotConfig.GeneralConfig.ErrorGroups)
                {
                    sendReport(groupId, messageBuilder.ToString());
                    Task.Delay(1000).Wait();
                }
                AddSendRecord(exception);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        /// <summary>
        /// 将错误日志发送到日志群中，忽略发送限制
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public static void SendErrorForce(Exception exception, string message)
        {
            try
            {
                if (BotConfig.GeneralConfig?.ErrorGroups is null) return;
                string sendMessage = $"{message}\r\n{exception.Message}\r\n{exception.StackTrace}";
                foreach (var groupId in BotConfig.GeneralConfig.ErrorGroups) sendReport(groupId, sendMessage);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        /// <summary>
        /// 判断这个小时能是否还可以发送日志
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static bool IsSendError(Exception ex)
        {
            lock (SendDic)
            {
                if (ex is null) return false;
                if (LastSendHour != DateTime.Now.Hour) return true;
                System.Type exType = ex.GetType();
                if (!SendDic.ContainsKey(exType)) return true;
                List<ErrorRecord> recordList = SendDic[exType];
                if (recordList is null || recordList.Count == 0) return true;
                if (exType == typeof(Exception)) return recordList.Count < exceptionSendTimes;
                return recordList.Count < childSendTimes;
            }
        }

        /// <summary>
        /// 添加发送记录
        /// </summary>
        /// <param name="exception"></param>
        private static void AddSendRecord(Exception exception)
        {
            lock (SendDic)
            {
                System.Type exType = exception.GetType();
                if (LastSendHour != DateTime.Now.Hour)
                {
                    SendDic = new Dictionary<System.Type, List<ErrorRecord>>();
                    LastSendHour = DateTime.Now.Hour;
                }
                if (!SendDic.ContainsKey(exType))
                {
                    SendDic[exType] = new List<ErrorRecord>();
                }
                SendDic[exType].Add(new ErrorRecord(exception));
            }
        }




    }
}
