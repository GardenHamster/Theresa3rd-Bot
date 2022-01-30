using Mirai.CSharp.Builders;
using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
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
                string message = chainList.Count > 0 ? string.Join(null, chainList.Skip(1).ToArray()) : "";
                string instructions = plainList.FirstOrDefault();
                bool isInstruct = instructions != null && prefix != null && prefix.Trim().Length > 0 && instructions.StartsWith(prefix);
                if (isInstruct) instructions = instructions.Remove(0, prefix.Length);

                if (isAt == false && isInstruct == false)//没有@也不是一条指令
                {
                    if (RepeatCache.CheckCanRepeat(groupId, botId, memberId, message))
                    {
                        IChatMessage[] repeatChain = args.Chain.Length > 1 ? args.Chain.Skip(1).ToArray() : new IChatMessage[0];
                        await session.SendGroupMessageAsync(args.Sender.Group.Id, repeatChain);//复读机
                    }
                    return;
                }

                if (!string.IsNullOrWhiteSpace(BotConfig.SubscribeConfig?.PixivUser?.AddCommand) && instructions.StartsWith(BotConfig.SubscribeConfig.PixivUser.AddCommand))
                {
                    if (BusinessHelper.CheckSuperManagersAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivUser).Result == false) return;
                    if (BusinessHelper.CheckPixivCookieExpireAsync(session, args).Result) return;
                    await new SubscribeBusiness().subscribePixivUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(BotConfig.SubscribeConfig?.PixivUser?.RmCommand) && instructions.StartsWith(BotConfig.SubscribeConfig.PixivUser.RmCommand))
                {
                    if (BusinessHelper.CheckSuperManagersAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivUser).Result == false) return;
                    await new SubscribeBusiness().cancleSubscribePixivUserAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(BotConfig.SubscribeConfig?.PixivTag?.AddCommand) && instructions.StartsWith(BotConfig.SubscribeConfig.PixivTag.AddCommand))
                {
                    if (BusinessHelper.CheckSuperManagersAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivTag).Result == false) return;
                    if (BusinessHelper.CheckPixivCookieExpireAsync(session, args).Result) return;
                    await new SubscribeBusiness().subscribePixivTagAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(BotConfig.SubscribeConfig?.PixivTag?.RmCommand) && instructions.StartsWith(BotConfig.SubscribeConfig.PixivTag.RmCommand))
                {
                    if (BusinessHelper.CheckSuperManagersAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckSubscribeEnableAsync(session, args, BotConfig.SubscribeConfig?.PixivTag).Result == false) return;
                    await new SubscribeBusiness().cancleSubscribePixivTagAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Subscribe, message);
                    return;
                }






                if (!string.IsNullOrWhiteSpace(BotConfig.SetuConfig?.Pixiv?.Command) && instructions.StartsWith(BotConfig.SetuConfig.Pixiv.Command))
                {
                    if (BusinessHelper.CheckSTEnableAsync(session, args).Result == false) return;
                    if (BusinessHelper.CheckPixivCookieExpireAsync(session, args).Result) return;
                    if (BusinessHelper.CheckMemberSTCoolingAsync(session, args).Result) return;
                    if (BusinessHelper.ChecekGroupSTCoolingAsync(session, args).Result) return;
                    if (BusinessHelper.CheckSTUseUpAsync(session, args).Result) return;
                    if (BusinessHelper.CheckHandingAsync(session, args).Result) return;
                    CoolingCache.SetGroupSTCooling(groupId, memberId);
                    await new PixivBusiness().sendGeneralPixivImageAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.Setu, message);
                    return;
                }

                if (instructions.StartsWith("test"))
                {
                    string aaa = AppContext.BaseDirectory;
                    await session.SendGroupMessageAsync(args.Sender.Group.Id, new PlainMessage("hello word"));
                    return;
                }
            }
            catch (System.Exception ex)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.ErrorMsg, " 出了点小问题，再试一次吧~");
                LogHelper.Error(ex, "GroupMessageEvent异常");
            }
            finally
            {
                args.BlockRemainingHandlers = true;
            }
        }



    }
}
