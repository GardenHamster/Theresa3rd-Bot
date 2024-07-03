﻿using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using TheresaBot.Core.Command;
using TheresaBot.Core.Model.Invoker;
using TheresaBot.Core.Session;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Command
{
    public class MiraiGroupQuoteCommand : GroupQuoteCommand
    {
        private IGroupMessageEventArgs Args { get; init; }

        public override long MessageId => Args.GetMessageId();

        public override long GroupId => Args.Sender.Group.Id;

        public override long MemberId => Args.Sender.Id;

        public override string MemberName => Args.Sender.Name;

        public override string MemberNick => Args.Sender.Name;

        public MiraiGroupQuoteCommand(BaseSession baseSession, CommandHandler<GroupQuoteCommand> invoker, IGroupMessageEventArgs args, string message, string instruction, string command, string prefix)
            : base(baseSession, invoker, message, instruction, command, prefix)
        {
            this.Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Chain.OfType<ImageMessage>().Select(o => o.Url).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

        public override long GetQuoteMessageId()
        {
            var quoteMessage = Args.Chain.Where(v => v is QuoteMessage).FirstOrDefault();
            return quoteMessage is null ? 0 : ((QuoteMessage)quoteMessage).Id;
        }

    }
}
