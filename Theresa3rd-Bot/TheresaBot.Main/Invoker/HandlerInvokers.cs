using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Helper;
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
                await new MenuHandler(session, reporter).SendMenuAsync(botCommand);
                return true;
            })),
            //拉黑成员
            new(BotConfig.ManageConfig?.DisableMemberCommands, CommandType.BanMember, new(async (botCommand, session, reporter) =>
            {
                ManageHandler handler = new ManageHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.DisableMemberAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //解禁成员
            new(BotConfig.ManageConfig?.EnableMemberCommands, CommandType.BanMember, new(async (botCommand, session, reporter) =>
            {
                ManageHandler handler = new ManageHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.EnableMemberAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //查询订阅
            new(BotConfig.ManageConfig?.ListSubCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                SubscribeHandler handler = new SubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.ListSubscribeAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //取消订阅
            new(BotConfig.ManageConfig?.RemoveSubCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                SubscribeHandler handler = new SubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.CancleSubscribeAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //订阅pixiv画师
            new(BotConfig.SubscribeConfig?.PixivUser?.AddCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                PixivSubscribeHandler handler = new PixivSubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.SubscribeUserAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //订阅pixiv关注画师列表
            new(BotConfig.SubscribeConfig?.PixivUser?.SyncCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                PixivSubscribeHandler handler = new PixivSubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.SubscribeFollowUserAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //退订pixiv画师
            new(BotConfig.SubscribeConfig?.PixivUser?.RmCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                PixivSubscribeHandler handler = new PixivSubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                await handler.CancleSubscribeUserAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //订阅pixiv标签
            new(BotConfig.SubscribeConfig?.PixivTag?.AddCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                PixivSubscribeHandler handler = new PixivSubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.PixivTag) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.SubscribeTagAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //退订pixiv标签
            new(BotConfig.SubscribeConfig?.PixivTag?.RmCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                PixivSubscribeHandler handler = new PixivSubscribeHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.PixivTag) == false) return false;
                await handler.CancleSubscribeTagAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //订阅米游社用户
            new(BotConfig.SubscribeConfig?.Miyoushe?.AddCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                MiyousheHandler handler = new MiyousheHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.Miyoushe) == false) return false;
                await handler.SubscribeUserAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //退订米游社用户
            new(BotConfig.SubscribeConfig?.Miyoushe?.RmCommands, CommandType.Subscribe, new(async (botCommand, session, reporter) =>
            {
                MiyousheHandler handler = new MiyousheHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand, BotConfig.SubscribeConfig?.Miyoushe) == false) return false;
                await handler.CancleSubscribeUserAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //禁止色图标签
            new(BotConfig.ManageConfig?.DisableTagCommands, CommandType.Manage, new(async (botCommand, session, reporter) =>
            {
                ManageHandler handler = new ManageHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.DisableTagAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //解禁色图标签
            new(BotConfig.ManageConfig?.EnableTagCommands, CommandType.Manage, new(async (botCommand, session, reporter) =>
            {
                ManageHandler handler = new ManageHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.EnableTagAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //绑定标签
            new(BotConfig.ManageConfig?.BindTagCommands, CommandType.Manage, new(async (botCommand, session, reporter) =>
            {
                SugarTagHandler handler = new SugarTagHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.BindPixivTagAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //解绑标签
            new(BotConfig.ManageConfig?.UnBindTagCommands, CommandType.Manage, new(async (botCommand, session, reporter) =>
            {
                SugarTagHandler handler = new SugarTagHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.UnBindPixivTagAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //添加词云词汇
            new(BotConfig.WordCloudConfig?.AddWordCommands, CommandType.Manage, new(async (botCommand, session, reporter) =>
            {
                DictionaryHandler handler = new DictionaryHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.AddCloudWordAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //隐藏词云词汇
            new(BotConfig.WordCloudConfig?.HideWordCommands, CommandType.Manage, new(async (botCommand, session, reporter) =>
            {
                DictionaryHandler handler = new DictionaryHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.HideCloudWordAsync(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.PixivSearchAsync(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.PixivUserProfileAsync(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.LoliconSearchAsync(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.LolisukiSearchAsync(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.LocalSearchAsync(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.SearchSource(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.SendDailyRanking(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.SendDailyAIRanking(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.SendMaleRanking(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.SendWeeklyRanking(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.SendMonthlyRanking(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.SendDailyR18Ranking(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.SendDailyAIR18Ranking(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.SendMaleR18Ranking(botCommand);
                await handler.InsertRecord(botCommand);
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
                await handler.SendWeeklyR18Ranking(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //查询词云
            new(BotConfig.WordCloudConfig?.BasicCommands, CommandType.WordCloud, new(async (botCommand, session, reporter) =>
            {
                WordCloudHandler handler = new WordCloudHandler(session, reporter);
                if (await handler.CheckWordCloudEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupWordCloudCoolingAsync(botCommand)) return false;
                if (await handler.CheckWordCloudHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.ReplyCustomWordCloudAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //今日词云
            new(BotConfig.WordCloudConfig?.DailyCommands, CommandType.WordCloud, new(async (botCommand, session, reporter) =>
            {
                WordCloudHandler handler = new WordCloudHandler(session, reporter);
                if (await handler.CheckWordCloudEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupWordCloudCoolingAsync(botCommand)) return false;
                if (await handler.CheckWordCloudHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.ReplyDailyWordCloudAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //本周词云
            new(BotConfig.WordCloudConfig?.WeeklyCommands, CommandType.WordCloud, new(async (botCommand, session, reporter) =>
            {
                WordCloudHandler handler = new WordCloudHandler(session, reporter);
                if (await handler.CheckWordCloudEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupWordCloudCoolingAsync(botCommand)) return false;
                if (await handler.CheckWordCloudHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.ReplyWeeklyWordCloudAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //本月词云
            new(BotConfig.WordCloudConfig?.MonthlyCommands, CommandType.WordCloud, new(async (botCommand, session, reporter) =>
            {
                WordCloudHandler handler = new WordCloudHandler(session, reporter);
                if (await handler.CheckWordCloudEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupWordCloudCoolingAsync(botCommand)) return false;
                if (await handler.CheckWordCloudHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.ReplyMonthlyWordCloudAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //本年词云
            new(BotConfig.WordCloudConfig?.YearlyCommands, CommandType.WordCloud, new(async (botCommand, session, reporter) =>
            {
                WordCloudHandler handler = new WordCloudHandler(session, reporter);
                if (await handler.CheckWordCloudEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupWordCloudCoolingAsync(botCommand)) return false;
                if (await handler.CheckWordCloudHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.ReplyYearlyWordCloudAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //昨日词云
            new(BotConfig.WordCloudConfig?.YesterdayCommands, CommandType.WordCloud, new(async (botCommand, session, reporter) =>
            {
                WordCloudHandler handler = new WordCloudHandler(session, reporter);
                if (await handler.CheckWordCloudEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupWordCloudCoolingAsync(botCommand)) return false;
                if (await handler.CheckWordCloudHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.ReplyYesterdayWordCloudAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //上周词云
            new(BotConfig.WordCloudConfig?.LastWeekCommands, CommandType.WordCloud, new(async (botCommand, session, reporter) =>
            {
                WordCloudHandler handler = new WordCloudHandler(session, reporter);
                if (await handler.CheckWordCloudEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupWordCloudCoolingAsync(botCommand)) return false;
                if (await handler.CheckWordCloudHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.ReplyLastWeekWordCloudAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //上月词云
            new(BotConfig.WordCloudConfig?.LastMonthCommands, CommandType.WordCloud, new(async (botCommand, session, reporter) =>
            {
                WordCloudHandler handler = new WordCloudHandler(session, reporter);
                if (await handler.CheckWordCloudEnableAsync(botCommand) == false) return false;
                if (await handler.CheckGroupWordCloudCoolingAsync(botCommand)) return false;
                if (await handler.CheckWordCloudHandingAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.ReplyLastMonthWordCloudAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //version
            new(new List<string>() { "版本", "version" }, CommandType.Version, new(async (botCommand, session, reporter) =>
            {
                await botCommand.ReplyGroupMessageWithQuoteAsync($"Theresa3rd-Bot v{BotConfig.BotVersion}");
                return false;
            })),
            //test
            new(new List<string>() { "test" }, CommandType.Other, new(async (botCommand, session, reporter) =>
            {
                await botCommand.Test(botCommand);
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
                await handler.InsertRecord(botCommand);
                return true;
            })),
            //SaucenaoCookie
            new(BotConfig.ManageConfig?.SaucenaoCookieCommands, CommandType.Manage, new(async (botCommand, session, reporter) =>
            {
                CookieHandler handler = new CookieHandler(session, reporter);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.UpdateSaucenaoCookieAsync(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            }))
        };

        public readonly static List<CommandHandler<GroupQuoteCommand>> GroupQuoteCommands = new()
        {
            //Saucenao
            new(BotConfig.SaucenaoConfig?.Commands, CommandType.Saucenao, new(async (botCommand, session, reporter) =>
            {
                SaucenaoHandler handler = new SaucenaoHandler(session, reporter);
                if (await handler.CheckSaucenaoEnableAsync(botCommand) == false) return false;
                if (BotConfig.SaucenaoConfig.PullOrigin && await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckMemberSaucenaoCoolingAsync(botCommand)) return false;
                if (await handler.CheckSaucenaoUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.SearchSource(botCommand);
                await handler.InsertRecord(botCommand);
                return true;
            }))
        };



    }
}
