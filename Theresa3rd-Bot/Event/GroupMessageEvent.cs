using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Handler;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IGroupMessageEventArgs, GroupMessageEventArgs>))]
    public class GroupMessageEvent : IMiraiHttpMessageHandler<IGroupMessageEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                long botId = session.QQNumber ?? 0;
                if (!BusinessHelper.IsHandleMessage(groupId)) return;

                string prefix = BotConfig.GeneralConfig.Prefix;
                bool isAt = args.Chain.Where(v => v is AtMessage atMsg && atMsg.Target == session.QQNumber).Any();

                List<string> chainList = args.Chain.Select(m => m.ToString()).ToList();
                List<string> plainList = args.Chain.Where(v => v is PlainMessage && v.ToString().Trim().Length > 0).Select(m => m.ToString().Trim()).ToList();
                if (chainList == null || chainList.Count == 0) return;
                if (plainList == null || plainList.Count == 0) return;

                string message = chainList.Count > 0 ? string.Join(null, chainList.Skip(1).ToArray()) : "";
                string instructions = plainList.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(instructions)) return;
                instructions = instructions.Trim();
                message = message.Trim();

                bool isInstruct = instructions != null && prefix != null && prefix.Trim().Length > 0 && instructions.StartsWith(prefix);
                if (isInstruct) instructions = instructions.Remove(0, prefix.Length).Trim();

                if (isAt == false && isInstruct == false)//没有@也不是一条指令
                {
                    if (StepCache.HandleStep(session, args, message)) return; //分步处理
                    if (RepeatCache.CheckCanRepeat(groupId, botId, memberId, message)) await SendRepeat(session, args);//复读机
                    return;
                }

                //菜单
                if (instructions.StartWithCommand(BotConfig.MenuConfig?.Commands))
                {
                    await new MenuHandler().sendMenuAsync(session, args);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //订阅pixiv画师
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivUser?.AddCommand))
                {
                    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivUser) == false) return;
                    if (await BusinessHelper.CheckPixivCookieAvailableAsync(session, args) == false) return;
                    await new PixivHandler().subscribeUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //同步pixiv画师
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivUser?.SyncCommand))
                {
                    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivUser) == false) return;
                    if (await BusinessHelper.CheckPixivCookieAvailableAsync(session, args) == false) return;
                    await new PixivHandler().subscribeFollowUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //退订pixiv画师
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivUser?.RmCommand))
                {
                    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivUser) == false) return;
                    await new PixivHandler().cancleSubscribeUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //订阅pixiv标签
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivTag?.AddCommand))
                {
                    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivTag) == false) return;
                    if (await BusinessHelper.CheckPixivCookieAvailableAsync(session, args) == false) return;
                    await new PixivHandler().subscribeTagAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //退订pixiv标签
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivTag?.RmCommand))
                {
                    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivTag) == false) return;
                    await new PixivHandler().cancleSubscribeTagAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //订阅米游社用户
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.Mihoyo?.AddCommand))
                {
                    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.Mihoyo) == false) return;
                    await new MYSHandler().subscribeMYSUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //退订米游社用户
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.Mihoyo?.RmCommand))
                {
                    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.Mihoyo) == false) return;
                    await new MYSHandler().cancleSubscribeMysUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //禁止色图标签
                if (instructions.StartWithCommand(BotConfig.SetuConfig?.DisableTagCommand))
                {
                    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckSetuEnableAsync(session, args) == false) return;
                    await new BanWordHandler().disableSetuAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.BanWord, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //解禁色图标签
                if (instructions.StartWithCommand(BotConfig.SetuConfig?.EnableTagCommand))
                {
                    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckSetuEnableAsync(session, args) == false) return;
                    await new BanWordHandler().enableSetuAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.BanWord, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //瑟图
                if (instructions.StartWithCommand(BotConfig.SetuConfig?.Lolicon?.Command))
                {
                    if (await BusinessHelper.CheckSetuEnableAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckSetuTagEnableAsync(session, args, message) == false) return;
                    if (await BusinessHelper.CheckMemberSetuCoolingAsync(session, args)) return;
                    if (await BusinessHelper.ChecekGroupSetuCoolingAsync(session, args)) return;
                    if (await BusinessHelper.CheckSetuUseUpAsync(session, args)) return;
                    if (await BusinessHelper.CheckHandingAsync(session, args)) return;
                    CoolingCache.SetGroupSetuCooling(groupId, memberId);
                    await new LoliconHandler().sendGeneralLoliconImageAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Setu, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //涩图
                if (instructions.StartWithCommand(BotConfig.SetuConfig?.Pixiv?.Command))
                {
                    if (await BusinessHelper.CheckSetuEnableAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckSetuTagEnableAsync(session, args, message) == false) return;
                    if (await BusinessHelper.CheckPixivCookieAvailableAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckMemberSetuCoolingAsync(session, args)) return;
                    if (await BusinessHelper.ChecekGroupSetuCoolingAsync(session, args)) return;
                    if (await BusinessHelper.CheckSetuUseUpAsync(session, args)) return;
                    if (await BusinessHelper.CheckHandingAsync(session, args)) return;
                    CoolingCache.SetGroupSetuCooling(groupId, memberId);
                    await new PixivHandler().sendGeneralPixivImageAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Setu, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //原图
                if (instructions.StartWithCommand(BotConfig.SaucenaoConfig?.Command))
                {
                    if (await BusinessHelper.CheckSaucenaoEnableAsync(session, args) == false) return;
                    if (BotConfig.SaucenaoConfig.PullOrigin && await BusinessHelper.CheckPixivCookieAvailableAsync(session, args) == false) return;
                    if (await BusinessHelper.CheckMemberSaucenaoCoolingAsync(session, args)) return;
                    if (await BusinessHelper.CheckSaucenaoUseUpAsync(session, args)) return;
                    if (await BusinessHelper.CheckHandingAsync(session, args)) return;
                    await new SaucenaoHandler().saucenaoSearch(session, args);
                    new RequestRecordBusiness().addRecord(args, CommandType.Saucenao, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //测试指令
                if (instructions.StartWithCommand("test"))
                {
                    await session.SendGroupMessageAsync(args.Sender.Group.Id, new PlainMessage("hello word"));
                    return;
                }
            }
            catch (System.Exception ex)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.ErrorMsg, " 出了点小问题，再试一次吧~");
                LogHelper.Error(ex, "GroupMessageEvent异常");
            }
        }

        public async Task SendRepeat(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            try
            {
                if (args.Chain.Length < 2) return;
                IChatMessage[] repeatChain = args.Chain.Skip(1).ToArray();
                await session.SendGroupMessageAsync(args.Sender.Group.Id, repeatChain);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "复读失败");
            }
        }


    }
}
