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
                WebsitePO saucenaoWebsite = new WebsiteBusiness().getOrInsertWebsite(Enum.GetName(typeof(WebsiteType), WebsiteType.Saucenao));
                BotConfig.WebsiteConfig.Pixiv = pixivWebsite;
                BotConfig.WebsiteConfig.Bili = biliWebsite;
                BotConfig.WebsiteConfig.Saucenao = saucenaoWebsite;
                LogHelper.Info("网站cookie加载完成...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载cookie失败");
            }
        }

        
        public static void loadSubscribeTask()
        {
            try
            {
                BotConfig.SubscribeTaskMap = new SubscribeBusiness().getSubscribeTask();
                LogHelper.Info("订阅任务加载完成...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "订阅任务加载失败");
            }
        }

        
        public static void loadBanWord()
        {
            try
            {
                BanWordBusiness banWordBusiness = new BanWordBusiness();
                BotConfig.BanSetuMap = banWordBusiness.getBanSetuMap();
                LogHelper.Info("加载违禁词完成");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载违禁词失败");
            }
        }

        /*
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
