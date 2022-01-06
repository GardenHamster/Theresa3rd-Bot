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
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IGroupMessageEventArgs, GroupMessageEventArgs>))]
    public class GroupMessageEvent : IMiraiHttpMessageHandler<IGroupMessageEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            long memberId = args.Sender.Id;
            long groupId = args.Sender.Group.Id;
            long botId = session.QQNumber ?? 0;
            if (!BusinessHelper.IsHandleMessage(groupId)) return;

            string prefix = BotConfig.GeneralConfig.Prefix;
            bool isAt = args.Chain.Where(v => v is AtMessage atMsg && atMsg.Target == session.QQNumber).Any();
            List<string> chainList = args.Chain.Select(m => m.ToString()).ToList();
            List<string> plainList = args.Chain.Where(v => v is PlainMessage && v.ToString().Trim().Length > 0).Select(m => m.ToString().Trim()).ToList();
            string messageStr = chainList.Count > 0 ? string.Join(null, chainList.Skip(1).ToArray()) : "";
            string instructions = plainList.FirstOrDefault();
            bool isInstruct = instructions != null && prefix != null && prefix.Trim().Length > 0 && instructions.StartsWith(prefix);
            if (isInstruct) instructions = instructions.Remove(0, prefix.Length);

            if (isAt == false && isInstruct == false) //没有@也不是一条指令
            {
                if (RepeatCache.CheckCanRepeat(groupId, botId, memberId, messageStr))
                {
                    IChatMessage[] repeatChain = args.Chain.Length > 1 ? args.Chain.Skip(1).ToArray() : new IChatMessage[0];
                    await session.SendGroupMessageAsync(args.Sender.Group.Id, repeatChain);//复读机
                }
                return;
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
