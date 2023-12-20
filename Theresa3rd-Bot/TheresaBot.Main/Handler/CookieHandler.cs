using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    internal class CookieHandler : BaseHandler
    {
        private WebsiteService websiteService;

        public CookieHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            websiteService = new WebsiteService();
        }

        /// <summary>
        /// 更新Pixiv Cookie
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task UpdatePixivCookieAsync(PrivateCommand command)
        {
            try
            {
                var cookie = command.KeyWord;
                if (string.IsNullOrWhiteSpace(cookie))
                {
                    await command.ReplyFriendMessageAsync($"未检测到Cookie");
                    return;
                }
                var website = websiteService.UpdatePixivCookie(cookie);
                WebsiteDatas.LoadWebsite();
                var expireDate = website.CookieExpireDate.ToSimpleString();
                await command.ReplyFriendMessageAsync($"Cookie更新完毕，过期时间为：{expireDate}");
            }
            catch (HandleException ex)
            {
                await command.ReplyFriendMessageAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "PixivCookie更新异常");
            }
        }

        /// <summary>
        /// 更新Saucenao Cookie
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task UpdateSaucenaoCookieAsync(PrivateCommand command)
        {
            try
            {
                var cookie = command.KeyWord;
                if (string.IsNullOrWhiteSpace(cookie))
                {
                    await command.ReplyFriendMessageAsync($"未检测到Cookie");
                    return;
                }
                var website = websiteService.UpdateSaucenaoCookie(cookie);
                WebsiteDatas.LoadWebsite();
                var expireDate = website.CookieExpireDate.ToSimpleString();
                await command.ReplyFriendMessageAsync($"Cookie更新完毕，过期时间为：{expireDate}");
            }
            catch (HandleException ex)
            {
                await command.ReplyFriendMessageAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "PixivCookie更新异常");
            }
        }

        /// <summary>
        /// 判断Cookie是否即将过期并发送提醒消息
        /// </summary>
        /// <param name="website"></param>
        /// <param name="diffDay"></param>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public async Task CheckAndWarn(WebsitePO website, int diffDay, string cookieName)
        {
            var expireDate = website.CookieExpireDate;
            if (DateTime.Now.AddDays(diffDay) < expireDate) return;
            if (expireDate.AddDays(diffDay) < DateTime.Now) return;
            var warnMessage = $"{cookieName}将在{expireDate.ToSimpleString()}过期，请尽快更新Cookie";
            foreach (long groupId in BotConfig.ErrorPushGroups)
            {
                await Session.SendGroupMessageAsync(groupId, warnMessage);
                await Task.Delay(1000);
            }
            foreach (long memberId in BotConfig.SuperManagers)
            {
                await Session.SendFriendMessageAsync(memberId, warnMessage);
                await Task.Delay(1000);
            }
        }

    }
}
