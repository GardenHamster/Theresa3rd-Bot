using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models.ChatMessages;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
{
    public class CookieWarningJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await CheckAndWarn(BotConfig.WebsiteConfig.Pixiv, 2, "Pixiv Cookie");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "CookieWarningJob异常");
            }
        }

        private async Task CheckAndWarn(WebsitePO website, int diffDay, string cookieName)
        {
            try
            {
                DateTime expireDate = website.CookieExpireDate;
                if (DateTime.Now.AddDays(diffDay) < expireDate) return;
                if (expireDate.AddDays(diffDay) < DateTime.Now) return;
                string warnMessage = $"{cookieName}将在{expireDate.ToString("yyyy-MM-dd HH:mm:ss")}过期，请尽快更新cookie";
                foreach (long memberId in BotConfig.PermissionsConfig.SuperManagers)
                {
                    await MiraiHelper.Session.SendFriendMessageAsync(memberId, new PlainMessage(warnMessage));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "CookieWarningJob.CheckAndWarn异常");
            }
        }


    }
}
