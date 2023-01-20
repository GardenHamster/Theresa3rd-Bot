using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
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
    public class GroupMessageEvent : BaseEvent, IMiraiHttpMessageHandler<IGroupMessageEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                long botId = session.QQNumber ?? 0;
                if (!BusinessHelper.IsHandleMessage(groupId)) return;
                if (memberId == BotConfig.MiraiConfig.BotQQ) return;
                if (CheckBanMemberAsync(session, args)) return;//黑名单成员

                string prefix = BotConfig.GeneralConfig.Prefix;
                List<string> chainList = args.Chain.Select(m => m.ToString()).ToList();
                List<string> plainList = args.Chain.Where(v => v is PlainMessage && v.ToString().Trim().Length > 0).Select(m => m.ToString().Trim()).ToList();
                if (chainList == null || chainList.Count == 0) return;

                string message = chainList.Count > 0 ? string.Join(null, chainList.Skip(1).ToArray()) : "";
                string instructions = plainList.FirstOrDefault()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(message)) return;
                message = message.Trim();

                bool isAt = args.Chain.Where(v => v is AtMessage atMsg && atMsg.Target == session.QQNumber).Any();
                bool isInstruct = string.IsNullOrWhiteSpace(instructions) == false && string.IsNullOrWhiteSpace(prefix) == false && instructions.StartsWith(prefix);
                if (isInstruct) instructions = instructions.Remove(0, prefix.Length).Trim();

                if (isAt == false && isInstruct == false)//没有@也不是一条指令
                {
                    if (StepCache.HandleStep(session, args, message)) return; //分步处理
                    if (RepeatCache.CheckCanRepeat(groupId, botId, memberId, message)) await SendRepeat(session, args);//复读机
                    return;
                }

                if (string.IsNullOrWhiteSpace(instructions)) return;//不存在任何指令
                
                //菜单
                if (instructions.StartWithCommand(BotConfig.MenuConfig?.Commands))
                {
                    await new MenuHandler().sendMenuAsync(session, args);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //拉黑成员
                if (instructions.StartWithCommand(BotConfig.ManageConfig?.DisableMemberCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    await new BanWordHandler().disableMemberAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.BanMember, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //解禁成员
                if (instructions.StartWithCommand(BotConfig.ManageConfig?.EnableMemberCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    await new BanWordHandler().enableMemberAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.BanMember, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //订阅pixiv画师
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivUser?.AddCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    if (await CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivUser) == false) return;
                    if (await CheckPixivCookieAvailableAsync(session, args) == false) return;
                    await new PixivHandler().subscribeUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //订阅pixiv关注画师列表
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivUser?.SyncCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    if (await CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivUser) == false) return;
                    if (await CheckPixivCookieAvailableAsync(session, args) == false) return;
                    await new PixivHandler().subscribeFollowUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //退订pixiv画师
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivUser?.RmCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    if (await CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivUser) == false) return;
                    await new PixivHandler().cancleSubscribeUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //订阅pixiv标签
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivTag?.AddCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    if (await CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivTag) == false) return;
                    if (await CheckPixivCookieAvailableAsync(session, args) == false) return;
                    await new PixivHandler().subscribeTagAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //退订pixiv标签
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivTag?.RmCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    if (await CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivTag) == false) return;
                    await new PixivHandler().cancleSubscribeTagAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //订阅米游社用户
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.Mihoyo?.AddCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    if (await CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.Mihoyo) == false) return;
                    await new MYSHandler().subscribeMYSUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //退订米游社用户
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.Mihoyo?.RmCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    if (await CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.Mihoyo) == false) return;
                    await new MYSHandler().cancleSubscribeMysUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //禁止色图标签
                if (instructions.StartWithCommand(BotConfig.ManageConfig?.DisableTagCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    if (await CheckSetuEnableAsync(session, args) == false) return;
                    await new BanWordHandler().disableSetuTagAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.BanSetuTag, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //解禁色图标签
                if (instructions.StartWithCommand(BotConfig.ManageConfig?.EnableTagCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    if (await CheckSetuEnableAsync(session, args) == false) return;
                    await new BanWordHandler().enableSetuTagAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.BanSetuTag, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //Lolicon
                if (instructions.StartWithCommand(BotConfig.SetuConfig?.Lolicon?.Command))
                {
                    if (await CheckSetuEnableAsync(session, args) == false) return;
                    if (await CheckMemberSetuCoolingAsync(session, args)) return;
                    if (await CheckGroupSetuCoolingAsync(session, args)) return;
                    if (await CheckSetuUseUpAsync(session, args)) return;
                    if (await CheckHandingAsync(session, args)) return;
                    CoolingCache.SetGroupSetuCooling(groupId, memberId);
                    await new LoliconHandler().sendGeneralLoliconImageAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Setu, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //Lolisuki
                if (instructions.StartWithCommand(BotConfig.SetuConfig?.Lolisuki?.Command))
                {
                    if (await CheckSetuEnableAsync(session, args) == false) return;
                    if (await CheckMemberSetuCoolingAsync(session, args)) return;
                    if (await CheckGroupSetuCoolingAsync(session, args)) return;
                    if (await CheckSetuUseUpAsync(session, args)) return;
                    if (await CheckHandingAsync(session, args)) return;
                    CoolingCache.SetGroupSetuCooling(groupId, memberId);
                    await new LolisukiHandler().sendGeneralLolisukiImageAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Setu, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //Pixiv
                if (instructions.StartWithCommand(BotConfig.SetuConfig?.Pixiv?.Command))
                {
                    if (await CheckSetuEnableAsync(session, args) == false) return;
                    if (await CheckPixivCookieAvailableAsync(session, args) == false) return;
                    if (await CheckMemberSetuCoolingAsync(session, args)) return;
                    if (await CheckGroupSetuCoolingAsync(session, args)) return;
                    if (await CheckSetuUseUpAsync(session, args)) return;
                    if (await CheckHandingAsync(session, args)) return;
                    CoolingCache.SetGroupSetuCooling(groupId, memberId);
                    await new PixivHandler().pixivSearchAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Setu, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //Saucenao
                if (instructions.StartWithCommand(BotConfig.SaucenaoConfig?.Command))
                {
                    if (await CheckSaucenaoEnableAsync(session, args) == false) return;
                    if (BotConfig.SaucenaoConfig.PullOrigin && await CheckPixivCookieAvailableAsync(session, args) == false) return;
                    if (await CheckMemberSaucenaoCoolingAsync(session, args)) return;
                    if (await CheckSaucenaoUseUpAsync(session, args)) return;
                    if (await CheckHandingAsync(session, args)) return;
                    await new SaucenaoHandler().searchResult(session, args);
                    new RequestRecordBusiness().addRecord(args, CommandType.Saucenao, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //version
                if (instructions.StartWithCommand("version") || instructions.StartWithCommand("版本"))
                {
                    await session.SendGroupMessageWithAtAsync(args, new PlainMessage($"Theresa3rd-Bot：Version：{BotConfig.BotVersion}"));
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
