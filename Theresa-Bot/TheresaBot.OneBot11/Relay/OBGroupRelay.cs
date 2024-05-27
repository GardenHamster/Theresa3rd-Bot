﻿using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Core.Relay;

namespace TheresaBot.OneBot11.Relay
{
    public class OBGroupRelay : GroupRelay
    {
        public CqGroupMessagePostContext Args { get; set; }

        public override long MsgId => Args.MessageId;

        public override long QuoteMsgId => Args.Message.OfType<CqReplyMsg>().FirstOrDefault()?.Id ?? 0;

        public override long GroupId => Args.GroupId;

        public override long MemberId => Args.Sender.UserId;

        public OBGroupRelay(CqGroupMessagePostContext args, string message, bool isAt, bool isQuote, bool isInstruct) : base(message, isAt, isQuote, isInstruct)
        {
            Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Message.OfType<CqImageMsg>().Select(o => o.Url?.ToString()).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

    }
}
