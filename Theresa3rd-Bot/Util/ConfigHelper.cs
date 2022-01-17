using System;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Util
{
    public class ConfigHelper
    {

        public static void loadWebsite()
        {
            try
            {
                WebsiteBusiness websiteBusiness = new WebsiteBusiness();
                WebsitePO pixivWebsite = new WebsiteBusiness().getOrInsertWebsite(Enum.GetName(typeof(WebsiteType), WebsiteType.Pixiv));
                WebsitePO biliWebsite = new WebsiteBusiness().getOrInsertWebsite(Enum.GetName(typeof(WebsiteType), WebsiteType.Bili));
                BotConfig.PixivWebsite = pixivWebsite;
                BotConfig.BiliWebsite = biliWebsite;
                LogHelper.Info("加载cookie完毕");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载cookie失败");
            }
        }
        /*
        public static void loadSubscribeTask()
        {
            try
            {
                Setting.Subscribe.SubscribeTaskMap = new SubscribeController().getSubscribeTask();
                CQHelper.CQLog.InfoSuccess("加载订阅任务完成");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex,"加载订阅任务失败");
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
                LogHelper.Error(ex, "加载违禁词失败");
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
                LogHelper.Error(ex, "加载打卡信息失败");
            }
        }

        */

    }
}
