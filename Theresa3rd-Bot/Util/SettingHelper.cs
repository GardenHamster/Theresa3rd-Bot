using Business.Business;
using Business.Common;
using Business.Controller;
using Business.Model.PO;
using Business.Type;
using Native.Sdk.Cqp.EventArgs;
using System;

namespace Theresa3rd_Bot.Util
{

    public static class SettingHelper
    {
        public static void loadSetting()
        {
            loadWebsiteAndCookie();
            loadSubscribeTask();
            loadBanWord();
            loadMemberClock();
        }

        public static void loadWebsiteAndCookie()
        {
            try
            {
                Website pixivWebsite = new WebsiteBusiness().getOrInsertWebsite(WebsiteType.Pixiv);
                Website bilibiliWebsite = new WebsiteBusiness().getOrInsertWebsite(WebsiteType.Bilibili);
                Setting.Pixiv.Cookie = pixivWebsite.Cookie;
                Setting.Pixiv.CookieExpireDate = pixivWebsite.CookieExpireDate;
                Setting.Pixiv.UpdateDate = pixivWebsite.UpdateDate;
                Setting.Bilibili.Cookie = bilibiliWebsite.Cookie;
                Setting.Bilibili.CookieExpireDate = bilibiliWebsite.CookieExpireDate;
                Setting.Bilibili.UpdateDate = bilibiliWebsite.UpdateDate;
                CQHelper.CQLog.InfoSuccess("加载网站和cookie完成");
            }
            catch (Exception ex)
            {
                CQHelper.CQLog.Error("加载网站和cookie失败", ex.Message, ex.StackTrace);
            }
        }

        public static void loadSubscribeTask()
        {
            try
            {
                Setting.Subscribe.SubscribeTaskMap = new SubscribeController().getSubscribeTask();
                CQHelper.CQLog.InfoSuccess("加载订阅任务完成");
            }
            catch (Exception ex)
            {
                CQHelper.CQLog.Error("加载订阅任务失败", ex.Message, ex.StackTrace);
            }
        }

        public static void loadBanWord()
        {
            try
            {
                Setting.Word.BanSTKeyWord = new BanWordBusiness().getListByType(BanWordType.ST.TypeId);
                Setting.Word.BanMemberId = new BanWordBusiness().getListByType(BanWordType.Member.TypeId);
                CQHelper.CQLog.InfoSuccess("加载违禁词完成");
            }
            catch (Exception ex)
            {
                CQHelper.CQLog.Error("加载违禁词失败", ex.Message, ex.StackTrace);
            }
        }

        public static void loadMemberClock()
        {
            try
            {
                Setting.Clock.MemberClockMap = new MemberClockBusiness().loadMemberClockMap();
                CQHelper.CQLog.InfoSuccess("加载打卡信息完成");
            }
            catch (Exception ex)
            {
                CQHelper.CQLog.Error("加载打卡信息失败", ex.Message, ex.StackTrace);
            }
        }

    }
}
