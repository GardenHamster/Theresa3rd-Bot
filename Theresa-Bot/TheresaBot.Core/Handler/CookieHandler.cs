using TheresaBot.Core.Command;
using TheresaBot.Core.Common;
using TheresaBot.Core.Datas;
using TheresaBot.Core.Exceptions;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.PO;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Services;
using TheresaBot.Core.Session;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Handler
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
                    await command.ReplyPrivateMessageAsync($"未检测到Cookie");
                    return;
                }
                var website = websiteService.UpdatePixivCookie(cookie);
                WebsiteDatas.LoadWebsite();
                var expireDate = website.CookieExpireDate.ToSimpleString();
                await command.ReplyPrivateMessageAsync($"Cookie更新完毕，过期时间为：{expireDate}");
            }
            catch (HandleException ex)
            {
                await command.ReplyPrivateMessageAsync(ex.RemindMessage);
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
                    await command.ReplyPrivateMessageAsync($"未检测到Cookie");
                    return;
                }
                var website = websiteService.UpdateSaucenaoCookie(cookie);
                WebsiteDatas.LoadWebsite();
                var expireDate = website.CookieExpireDate.ToSimpleString();
                await command.ReplyPrivateMessageAsync($"Cookie更新完毕，过期时间为：{expireDate}");
            }
            catch (HandleException ex)
            {
                await command.ReplyPrivateMessageAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "PixivCookie更新异常");
            }
        }

        /// <summary>
        /// 更新Pixiv Cookie
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task UpdatePixivCSRFTokenAsync(PrivateCommand command)
        {
            try
            {
                var token = command.KeyWord;
                if (string.IsNullOrWhiteSpace(token))
                {
                    await command.ReplyPrivateMessageAsync($"未检测到csrf-token");
                    return;
                }
                var pixivCode = WebsiteType.Pixiv.ToString();
                var website = websiteService.UpdateCsrfToken(pixivCode, token);
                WebsiteDatas.LoadWebsite();
                await command.ReplyPrivateMessageAsync($"Token更新完毕");
            }
            catch (HandleException ex)
            {
                await command.ReplyPrivateMessageAsync(ex.RemindMessage);
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
