using System.Text;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Error;

namespace TheresaBot.Main.Reporter
{
    public abstract class BaseReporter
    {
        private const int childSendTimes = 3;
        private const int exceptionSendTimes = 10;
        private static int LastSendHour = DateTime.Now.Hour;
        private static Dictionary<System.Type, List<ErrorRecord>> SendDic = new Dictionary<System.Type, List<ErrorRecord>>();

        protected abstract Task<long> SendReport(long groupId, string message);

        /// <summary>
        /// 将错误日志发送到日志群中
        /// 每个小时最多发送10种类型且每种类型最多不超过3次的日志
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public async Task SendError(Exception exception, string message = "")
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
                    await SendReport(groupId, messageBuilder.ToString());
                    Task.Delay(1000).Wait();
                }
                AddSendRecord(exception);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "错误日志发送失败");
            }
        }

        /// <summary>
        /// 将错误日志发送到日志群中，忽略发送限制
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public async Task SendErrorForce(Exception exception, string message)
        {
            try
            {
                if (BotConfig.GeneralConfig?.ErrorGroups is null) return;
                string sendMessage = $"{message}\r\n{exception.Message}\r\n{exception.StackTrace}";
                foreach (var groupId in BotConfig.GeneralConfig.ErrorGroups) await SendReport(groupId, sendMessage);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "错误日志发送失败");
            }
        }

        /// <summary>
        /// 判断这个小时能是否还可以发送日志
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsSendError(Exception ex)
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
        private void AddSendRecord(Exception exception)
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
