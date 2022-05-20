using Mirai.CSharp.Builders;
using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
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

                bool isInstruct = instructions != null && prefix != null && prefix.Trim().Length > 0 && instructions.StartsWith(prefix);
                if (isInstruct) instructions = instructions.Remove(0, prefix.Length);

                if (isAt == false && isInstruct == false)//没有@也不是一条指令
                {
                    //复读机
                    if (RepeatCache.CheckCanRepeat(groupId, botId, memberId, message)) await SendRepeat(session, args);
                    return;
                }

                //订阅pixiv画师
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivUser?.AddCommand))
                {
                    if (BusinessHelper.CheckSuperManagersAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivUser).Result == false) return;
                    if (BusinessHelper.CheckPixivCookieExpireAsync(session, args).Result) return;
                    await new SubscribeBusiness().subscribePixivUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //退订pixiv画师
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivUser?.RmCommand))
                {
                    if (BusinessHelper.CheckSuperManagersAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivUser).Result == false) return;
                    await new SubscribeBusiness().cancleSubscribePixivUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //订阅pixiv标签
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivTag?.AddCommand))
                {
                    if (BusinessHelper.CheckSuperManagersAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivTag).Result == false) return;
                    if (BusinessHelper.CheckPixivCookieExpireAsync(session, args).Result) return;
                    await new SubscribeBusiness().subscribePixivTagAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //退订pixiv标签
                if (instructions.StartWithCommand(BotConfig.SubscribeConfig?.PixivTag?.RmCommand))
                {
                    if (BusinessHelper.CheckSuperManagersAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivTag).Result == false) return;
                    await new SubscribeBusiness().cancleSubscribePixivTagAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //禁止色图标签
                if (instructions.StartWithCommand(BotConfig.SetuConfig?.DisableTagCommand))
                {
                    if (BusinessHelper.CheckSuperManagersAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSTEnableAsync(session, args).Result == false) return;
                    await new BanWordBusiness().disableSetuAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.BanWord, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //解禁色图标签
                if (instructions.StartWithCommand(BotConfig.SetuConfig?.EnableTagCommand))
                {
                    if (BusinessHelper.CheckSuperManagersAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSTEnableAsync(session, args).Result == false) return;
                    await new BanWordBusiness().enableSetuAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.BanWord, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //瑟图
                if (instructions.StartWithCommand(BotConfig.SetuConfig?.Lolicon?.Command))
                {
                    if (BusinessHelper.CheckSTEnableAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSTTagEnableAsync(session, args, message).Result == false) return;
                    if (BusinessHelper.CheckMemberSTCoolingAsync(session, args).Result) return;
                    if (BusinessHelper.ChecekGroupSTCoolingAsync(session, args).Result) return;
                    if (BusinessHelper.CheckSTUseUpAsync(session, args).Result) return;
                    if (BusinessHelper.CheckHandingAsync(session, args).Result) return;
                    CoolingCache.SetGroupSTCooling(groupId, memberId);
                    await new LoliconBusiness().sendGeneralLoliconImageAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Setu, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //涩图
                if (instructions.StartWithCommand(BotConfig.SetuConfig?.Pixiv?.Command))
                {
                    if (BusinessHelper.CheckSTEnableAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSTTagEnableAsync(session, args, message).Result == false) return;
                    if (BusinessHelper.CheckPixivCookieExpireAsync(session, args).Result) return;
                    if (BusinessHelper.CheckMemberSTCoolingAsync(session, args).Result) return;
                    if (BusinessHelper.ChecekGroupSTCoolingAsync(session, args).Result) return;
                    if (BusinessHelper.CheckSTUseUpAsync(session, args).Result) return;
                    if (BusinessHelper.CheckHandingAsync(session, args).Result) return;
                    CoolingCache.SetGroupSTCooling(groupId, memberId);
                    await new PixivBusiness().sendGeneralPixivImageAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Setu, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                if (instructions.StartWithCommand("test"))
                {
                    //await session.SendGroupMessageAsync(args.Sender.Group.Id, new PlainMessage("hello word"));
                    IChatMessage imgMessage = (IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, "D:\\other\\85972415_p0.jpg");
                    await session.SendTempMessageAsync(args.Sender.Id, args.Sender.Group.Id, imgMessage);
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
