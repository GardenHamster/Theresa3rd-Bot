using Mirai.CSharp.Builders;
using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
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
        private PixivBusiness pixivBusiness;
        private SubscribeBusiness subscribeBusiness;
        private RequestRecordBusiness requestRecordBusiness;

        public GroupMessageEvent()
        {
            this.pixivBusiness = new PixivBusiness();
            this.subscribeBusiness = new SubscribeBusiness();
            this.requestRecordBusiness = new RequestRecordBusiness();
        }

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

                if (instructions.StartsWith(BotConfig.SubscribeConfig.PixivUser.AddCommand))
                {
                    await subscribeBusiness.subscribePixivUserAsync(session, args, message);
                    requestRecordBusiness.addRecord(args, CommandType.Subscribe, message);
                    return;
                }

                if (instructions.StartsWith(BotConfig.SubscribeConfig.PixivUser.RmCommand))
                {
                    await subscribeBusiness.cancleSubscribePixivUserAsync(session, args, message);
                    requestRecordBusiness.addRecord(args, CommandType.Subscribe, message);
                    return;
                }

                if (instructions.StartsWith(BotConfig.SetuConfig.Pixiv.Command))
                {
                    await pixivBusiness.sendGeneralPixivImageAsync(session, args, message);
                    requestRecordBusiness.addRecord(args, CommandType.Setu, message);
                    return;
                }

                if (instructions.StartsWith("test"))
                {
                    IMessageChainBuilder builder = session.GetMessageChainBuilder();
                    builder.AddPlainMessage("Hello World!");
                    int msgId = await session.SendGroupMessageAsync(args.Sender.Group.Id, builder);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "GroupMessageEvent异常");
            }


            //IChatMessage[] chain = new IChatMessage[] {
            //    new AtMessage(args.Sender.Id,""),
            //    new PlainMessage("emmm\nemmmm")
            //};
            //await session.SendGroupMessageAsync(args.Sender.Group.Id, chain);

            args.BlockRemainingHandlers = true;

        }



    }
}
