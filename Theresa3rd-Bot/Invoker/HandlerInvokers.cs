using System.Collections.Generic;
using Theresa3rd_Bot.BotPlatform.Base.Command;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Handler;
using Theresa3rd_Bot.Model.Invoker;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Invoker
{
    public static class HandlerInvokers
    {
        public readonly static List<CommandHandler<GroupCommand>> GroupCommandInvokers = new()
        {
            //菜单
            new(BotConfig.MenuConfig?.Commands, CommandType.Menu, new(async (botCommand) =>
            {
                await new MenuHandler().sendMenuAsync(botCommand);
                return true;
            })),

            //拉黑成员
            new(BotConfig.ManageConfig?.DisableMemberCommands, CommandType.BanMember, new(async (botCommand) =>
            {
                BanWordHandler handler = new BanWordHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.disableMemberAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //解禁成员
            new(BotConfig.ManageConfig?.EnableMemberCommands, CommandType.BanMember, new(async (botCommand) =>
            {
                BanWordHandler handler = new BanWordHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.enableMemberAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //订阅pixiv画师
            new(BotConfig.SubscribeConfig?.PixivUser?.AddCommands, CommandType.Subscribe, new(async (botCommand) =>
            {
                PixivHandler handler = new PixivHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.subscribeUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //订阅pixiv关注画师列表
            new(BotConfig.SubscribeConfig?.PixivUser?.SyncCommands, CommandType.Subscribe, new(async (botCommand) =>
            {
                PixivHandler handler = new PixivHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.subscribeFollowUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //退订pixiv画师
            new(BotConfig.SubscribeConfig?.PixivUser?.RmCommands, CommandType.Subscribe, new(async (botCommand) =>
            {
                PixivHandler handler = new PixivHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                await handler.cancleSubscribeUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //订阅pixiv标签
            new(BotConfig.SubscribeConfig?.PixivTag?.AddCommands, CommandType.Subscribe, new(async (botCommand) =>
            {
                PixivHandler handler = new PixivHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.PixivTag) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.subscribeTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //退订pixiv标签
            new(BotConfig.SubscribeConfig?.PixivTag?.RmCommands, CommandType.Subscribe, new(async (botCommand) =>
            {
                PixivHandler handler = new PixivHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.PixivTag) == false) return false;
                await handler.cancleSubscribeTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //订阅米游社用户
            new(BotConfig.SubscribeConfig?.Mihoyo?.AddCommands, CommandType.Subscribe, new(async (botCommand) =>
            {
                MYSHandler handler = new MYSHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.Mihoyo) == false) return false;
                await handler.subscribeMYSUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //退订米游社用户
            new(BotConfig.SubscribeConfig?.Mihoyo?.RmCommands, CommandType.Subscribe, new(async (botCommand) =>
            {
                MYSHandler handler = new MYSHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.Mihoyo) == false) return false;
                await handler.cancleSubscribeMysUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //禁止色图标签
            new(BotConfig.ManageConfig?.DisableTagCommands, CommandType.BanSetuTag, new(async (botCommand) =>
            {
                BanWordHandler handler = new BanWordHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.disableSetuTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //解禁色图标签
            new(BotConfig.ManageConfig?.EnableTagCommands, CommandType.BanSetuTag, new(async (botCommand) =>
            {
                BanWordHandler handler = new BanWordHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.enableSetuTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //Pixiv
            new(BotConfig.SetuConfig?.Pixiv?.Commands, CommandType.Setu, new(async (botCommand) =>
            {
                PixivHandler handler = new PixivHandler();
                if (await handler.CheckSetuEnableAsync(botCommand,BotConfig.SetuConfig?.Pixiv) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckMemberSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckGroupSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckSetuUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                CoolingCache.SetGroupSetuCooling(botCommand.GroupId, botCommand.MemberId);
                await handler.pixivSearchAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //Lolicon
            new(BotConfig.SetuConfig?.Lolicon?.Commands, CommandType.Setu, new(async (botCommand) =>
            {
                LoliconHandler handler=new LoliconHandler();
                if (await handler.CheckSetuEnableAsync(botCommand,BotConfig.SetuConfig?.Lolicon) == false) return false;
                if (await handler.CheckMemberSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckGroupSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckSetuUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                CoolingCache.SetGroupSetuCooling(botCommand.GroupId, botCommand.MemberId);
                await handler.loliconSearchAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //Lolisuki
            new(BotConfig.SetuConfig?.Lolisuki?.Commands, CommandType.Setu, new(async (botCommand) =>
            {
                LolisukiHandler handler=new LolisukiHandler();
                if (await handler.CheckSetuEnableAsync(botCommand,BotConfig.SetuConfig?.Lolisuki) == false) return false;
                if (await handler.CheckMemberSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckGroupSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckSetuUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                CoolingCache.SetGroupSetuCooling(botCommand.GroupId, botCommand.MemberId);
                await handler.lolisukiSearchAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //Saucenao
            new(BotConfig.SaucenaoConfig?.Commands, CommandType.Saucenao, new(async (botCommand) =>
            {
                SaucenaoHandler handler=new SaucenaoHandler();
                if (await handler.CheckSaucenaoEnableAsync(botCommand) == false) return false;
                if (BotConfig.SaucenaoConfig.PullOrigin && await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckMemberSaucenaoCoolingAsync(botCommand)) return false;
                if (await handler.CheckSaucenaoUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.searchResult(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //version
            new(new() { "版本", "version" }, CommandType.Version, new(async (botCommand) =>
            {
                await botCommand.ReplyGroupMessageWithAtAsync($"Theresa3rd-Bot：Version：{BotConfig.BotVersion}");
                return false;
            }))
        };

        public readonly static List<CommandHandler<FriendCommand>> FriendCommandInvokers = new()
        {
            //PixivCookie
            new(BotConfig.ManageConfig?.PixivCookieCommands, CommandType.SetCookie, new(async (botCommand) =>
            {
                CookieHandler handler=new CookieHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.UpdatePixivCookieAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //SaucenaoCookie
            new(BotConfig.ManageConfig?.SaucenaoCookieCommands, CommandType.BanSetuTag, new(async (botCommand) =>
            {
                CookieHandler handler=new CookieHandler();
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.UpdateSaucenaoCookieAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            }))
        };



    }
}
