using System;
using TheresaBot.Main.Business;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Helper
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

        
        public static void loadBanTag()
        {
            try
            {
                BotConfig.BanSetuTagList = new BanWordBusiness().getBanSetuTagList();
                LogHelper.Info("加载禁止标签完毕");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载禁止标签失败");
            }
        }

        public static void loadBanMember()
        {
            try
            {
                BotConfig.BanMemberList = new BanWordBusiness().getBanMemberList();
                LogHelper.Info("加载黑名单完毕");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载黑名单失败");
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
