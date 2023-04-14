using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Invoker
{
    public static class HandlerInvokers
    {
        public readonly static List<CommandHandler<GroupCommand>> GroupCommands = new()
        {
            //菜单
            new(BotConfig.MenuConfig?.Commands, CommandType.Menu, new(async (botCommand, session, reporter) =>
            {
                await new MenuHandler(session, reporter).sendMenuAsync(botCommand);
                return true;
            })),
            //拉黑成员
            new(BotConfig.ManageConfig?.DisableMemberCommands, CommandType.BanMember, new(async (botCommand, session, reporter) =>
            {
                BanWordHandler handler = new BanWordHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.disableMemberAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //解禁成员
            new(BotConfig.ManageConfig?.EnableMemberCommands, CommandType.BanMember, new(async (botCommand, session, reporter) =>
            {
                BanWordHandler handler = new BanWordHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.enableMemberAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //订阅pixiv画师
            new(BotConfig.SubscribeConfig?.PixivUser?.AddCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                PixivSubscribeHandler handler = new PixivSubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.subscribeUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //订阅pixiv关注画师列表
            new(BotConfig.SubscribeConfig?.PixivUser?.SyncCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                PixivSubscribeHandler handler = new PixivSubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.subscribeFollowUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //退订pixiv画师
            new(BotConfig.SubscribeConfig?.PixivUser?.RmCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                PixivSubscribeHandler handler = new PixivSubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                await handler.cancleSubscribeUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //订阅pixiv标签
            new(BotConfig.SubscribeConfig?.PixivTag?.AddCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                PixivSubscribeHandler handler = new PixivSubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.PixivTag) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.subscribeTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //退订pixiv标签
            new(BotConfig.SubscribeConfig?.PixivTag?.RmCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                PixivSubscribeHandler handler = new PixivSubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.PixivTag) == false) return false;
                await handler.cancleSubscribeTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //订阅米游社用户
            new(BotConfig.SubscribeConfig?.Miyoushe?.AddCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                MYSHandler handler = new MYSHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.Miyoushe) == false) return false;
                await handler.subscribeMYSUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //退订米游社用户
            new(BotConfig.SubscribeConfig?.Miyoushe?.RmCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                MYSHandler handler = new MYSHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.Miyoushe) == false) return false;
                await handler.cancleSubscribeMysUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //禁止色图标签
            new(BotConfig.ManageConfig?.DisableTagCommands, CommandType.BanSetuTag, new(async (botCommand, session, reporter) =>
            {
                BanWordHandler handler = new BanWordHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.disableSetuTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //解禁色图标签
            new(BotConfig.ManageConfig?.EnableTagCommands, CommandType.BanSetuTag, new(async (botCommand, session, reporter) =>
            {
                BanWordHandler handler = new BanWordHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.enableSetuTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //Pixiv
            new(BotConfig.SetuConfig?.Pixiv?.Commands, CommandType.Setu, new(async (botCommand, session, reporter) =>
            {
                PixivHandler handler = new PixivHandler(session, reporter);
                if (await handler.CheckSetuEnableAsync(botCommand, BotConfig.SetuConfig?.Pixiv) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckMemberSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckGroupSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckSetuUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                CoolingCache.SetGroupSetuCooling(botCommand.GroupId);
                await handler.pixivSearchAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //Pixiv画师作品列表
            new(BotConfig.SetuConfig?.PixivUser?.Commands, CommandType.Setu, new(async (botCommand, session, reporter) =>
            {
                PixivHandler handler = new PixivHandler(session, reporter);
                if (await handler.CheckSetuEnableAsync(botCommand, BotConfig.SetuConfig?.PixivUser) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckMemberSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckGroupSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckSetuUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                CoolingCache.SetGroupSetuCooling(botCommand.GroupId);
                await handler.pixivUserProfileAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //Lolicon
            new(BotConfig.SetuConfig?.Lolicon?.Commands, CommandType.Setu, new(async (botCommand, session, reporter) =>
            {
                LoliconHandler handler = new LoliconHandler(session, reporter);
                if (await handler.CheckSetuEnableAsync(botCommand, BotConfig.SetuConfig?.Lolicon) == false) return false;
                if (await handler.CheckMemberSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckGroupSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckSetuUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                CoolingCache.SetGroupSetuCooling(botCommand.GroupId);
                await handler.loliconSearchAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //Lolisuki
            new(BotConfig.SetuConfig?.Lolisuki?.Commands, CommandType.Setu, new(async (botCommand, session, reporter) =>
            {
                LolisukiHandler handler = new LolisukiHandler(session, reporter);
                if (await handler.CheckSetuEnableAsync(botCommand, BotConfig.SetuConfig?.Lolisuki) == false) return false;
                if (await handler.CheckMemberSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckGroupSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckSetuUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                CoolingCache.SetGroupSetuCooling(botCommand.GroupId);
                await handler.lolisukiSearchAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //LocalSetu
            new(BotConfig.SetuConfig?.Local?.Commands, CommandType.Setu, new(async (botCommand, session, reporter) =>
            {
                LocalSetuHandler handler = new LocalSetuHandler(session, reporter);
                if (await handler.CheckSetuEnableAsync(botCommand, BotConfig.SetuConfig?.Local) == false) return false;
                if (await handler.CheckMemberSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckGroupSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckSetuUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                CoolingCache.SetGroupSetuCooling(botCommand.GroupId);
                await handler.localSearchAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //Saucenao
            new(BotConfig.SaucenaoConfig?.Commands, CommandType.Saucenao, new(async (botCommand, session, reporter) =>
            {
                SaucenaoHandler handler = new SaucenaoHandler(session, reporter);
                if (await handler.CheckSaucenaoEnableAsync(botCommand) == false) return false;
                if (BotConfig.SaucenaoConfig.PullOrigin && await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckMemberSaucenaoCoolingAsync(botCommand)) return false;
                if (await handler.CheckSaucenaoUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.searchResult(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //日榜
            new(BotConfig.PixivRankingConfig?.Daily?.Commands, CommandType.PixivRanking, new(async (botCommand, session, reporter) =>
            {
                PixivRankingHandler handler = new PixivRankingHandler(session, reporter);
                if (await handler.CheckPixivRankingEnableAsync(botCommand, BotConfig.PixivRankingConfig?.Daily) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupRankingCoolingAsync(botCommand, PixivRankingType.Daily)) return false;
                if (await handler.CheckPixivRankingHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.sendDailyRanking(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //AI日榜
            new(BotConfig.PixivRankingConfig?.DailyAI?.Commands, CommandType.PixivRanking, new(async (botCommand, session, reporter) =>
            {
                PixivRankingHandler handler = new PixivRankingHandler(session, reporter);
                if (await handler.CheckPixivRankingEnableAsync(botCommand, BotConfig.PixivRankingConfig?.DailyAI) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupRankingCoolingAsync(botCommand, PixivRankingType.DailyAI)) return false;
                if (await handler.CheckPixivRankingHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.sendDailyAIRanking(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //受男性欢迎日榜
            new(BotConfig.PixivRankingConfig?.Male?.Commands, CommandType.PixivRanking, new(async (botCommand, session, reporter) =>
            {
                PixivRankingHandler handler = new PixivRankingHandler(session, reporter);
                if (await handler.CheckPixivRankingEnableAsync(botCommand, BotConfig.PixivRankingConfig?.Male) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupRankingCoolingAsync(botCommand, PixivRankingType.Male)) return false;
                if (await handler.CheckPixivRankingHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.sendMaleRanking(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //周榜
            new(BotConfig.PixivRankingConfig?.Weekly?.Commands, CommandType.PixivRanking, new(async (botCommand, session, reporter) =>
            {
                PixivRankingHandler handler = new PixivRankingHandler(session, reporter);
                if (await handler.CheckPixivRankingEnableAsync(botCommand, BotConfig.PixivRankingConfig?.Weekly) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupRankingCoolingAsync(botCommand, PixivRankingType.Weekly)) return false;
                if (await handler.CheckPixivRankingHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.sendWeeklyRanking(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //月榜
            new(BotConfig.PixivRankingConfig?.Monthly?.Commands, CommandType.PixivRanking, new(async (botCommand, session, reporter) =>
            {
                PixivRankingHandler handler = new PixivRankingHandler(session, reporter);
                if (await handler.CheckPixivRankingEnableAsync(botCommand, BotConfig.PixivRankingConfig?.Monthly) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupRankingCoolingAsync(botCommand, PixivRankingType.Monthly)) return false;
                if (await handler.CheckPixivRankingHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.sendMonthlyRanking(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //R18日榜
            new(BotConfig.PixivRankingConfig?.Daily?.R18Commands, CommandType.PixivRanking, new(async (botCommand, session, reporter) =>
            {
                PixivRankingHandler handler = new PixivRankingHandler(session, reporter);
                if (await handler.CheckPixivRankingEnableAsync(botCommand, BotConfig.PixivRankingConfig?.Daily) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckR18ImgEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupRankingCoolingAsync(botCommand, PixivRankingType.Daily)) return false;
                if (await handler.CheckPixivRankingHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.sendDailyR18Ranking(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //R18AI日榜
            new(BotConfig.PixivRankingConfig?.DailyAI?.R18Commands, CommandType.PixivRanking, new(async (botCommand, session, reporter) =>
            {
                PixivRankingHandler handler = new PixivRankingHandler(session, reporter);
                if (await handler.CheckPixivRankingEnableAsync(botCommand, BotConfig.PixivRankingConfig?.DailyAI) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckR18ImgEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupRankingCoolingAsync(botCommand, PixivRankingType.DailyAI)) return false;
                if (await handler.CheckPixivRankingHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.sendDailyAIR18Ranking(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //R18受男性欢迎日榜
            new(BotConfig.PixivRankingConfig?.Male?.R18Commands, CommandType.PixivRanking, new(async (botCommand, session, reporter) =>
            {
                PixivRankingHandler handler = new PixivRankingHandler(session, reporter);
                if (await handler.CheckPixivRankingEnableAsync(botCommand, BotConfig.PixivRankingConfig?.Male) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckR18ImgEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupRankingCoolingAsync(botCommand, PixivRankingType.Male)) return false;
                if (await handler.CheckPixivRankingHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.sendMaleR18Ranking(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //R18周榜
            new(BotConfig.PixivRankingConfig?.Weekly?.R18Commands, CommandType.PixivRanking, new(async (botCommand, session, reporter) =>
            {
                PixivRankingHandler handler = new PixivRankingHandler(session, reporter);
                if (await handler.CheckPixivRankingEnableAsync(botCommand, BotConfig.PixivRankingConfig?.Weekly) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckR18ImgEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupRankingCoolingAsync(botCommand, PixivRankingType.Weekly)) return false;
                if (await handler.CheckPixivRankingHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.sendWeeklyR18Ranking(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //version
            new(new List<string>() { "版本", "version" }, CommandType.Version, new(async (botCommand, session, reporter) =>
            {
                await botCommand.ReplyGroupMessageAsync($"Theresa3rd-Bot {BotConfig.BotVersion}");
                return false;
            }))
        };

        public readonly static List<CommandHandler<FriendCommand>> FriendCommands = new()
        {
            //PixivCookie
            new(BotConfig.ManageConfig?.PixivCookieCommands, CommandType.SetCookie, new(async (botCommand, session, reporter) =>
            {
                CookieHandler handler = new CookieHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.UpdatePixivCookieAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),
            //SaucenaoCookie
            new(BotConfig.ManageConfig?.SaucenaoCookieCommands, CommandType.BanSetuTag, new(async (botCommand, session, reporter) =>
            {
                CookieHandler handler = new CookieHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.UpdateSaucenaoCookieAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            }))
        };



    }
}
