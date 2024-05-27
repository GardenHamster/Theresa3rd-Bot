﻿using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Core.Command;
using TheresaBot.Core.Model.Invoker;
using TheresaBot.Core.Session;

namespace TheresaBot.OneBot11.Command
{
    public class OBFriendCommand : PrivateCommand
    {
        private CqPrivateMessagePostContext Args { get; init; }

        public override long MessageId => Args.MessageId;

        public override long MemberId => Args.Sender.UserId;

        public override string MemberName => Args.Sender.Nickname;

        public OBFriendCommand(BaseSession baseSession, CommandHandler<PrivateCommand> invoker, CqPrivateMessagePostContext args, string instruction, string command, string prefix)
            : base(baseSession, invoker, instruction, command, prefix)
        {
            Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Message.OfType<CqImageMsg>().Select(o => o.Image).ToList();
        }


    }
}
