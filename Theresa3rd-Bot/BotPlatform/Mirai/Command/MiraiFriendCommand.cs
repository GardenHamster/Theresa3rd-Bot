using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Theresa3rd_Bot.BotPlatform.Base.Command;
using Theresa3rd_Bot.BotPlatform.Mirai.Util;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Content;
using Theresa3rd_Bot.Model.Invoker;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Command
{
    public class MiraiFriendCommand : FriendCommand
    {
        public IFriendMessageEventArgs Args { get; set; }
        public IMiraiHttpSession Session { get; set; }

        public MiraiFriendCommand(CommandHandler<FriendCommand> invoker, IMiraiHttpSession session, IFriendMessageEventArgs args, string[] keyWords, string instruction, long memberId) : base(invoker, keyWords, instruction, memberId)
        {
            this.Args = args;
            this.Session = session;
        }

        public override async Task<int> ReplyFriendTemplateAsync(string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            List<IChatMessage> msgList = await template.SplitToChainAsync().ToMiraiMessageAsync();
            return await Session.SendFriendMessageAsync(MemberId, msgList.ToArray());
        }

        public async Task<List<IChatMessage>> UploadPictureAsync(List<FileInfo> setuFiles, UploadTarget target)
        {
            List<IChatMessage> imgMsgs = new List<IChatMessage>();
            foreach (FileInfo setuFile in setuFiles)
            {
                if (setuFile is null)
                {

                    imgMsgs.AddRange(await BusinessHelper.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, SendTarget.Group).ToMiraiMessageAsync());
                }
                else
                {
                    imgMsgs.Add((IChatMessage)await Session.UploadPictureAsync(target, setuFile.FullName));
                }
            }
            return imgMsgs;
        }

        /// <summary>
        /// 发送错误记录
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="message"></param>
        private static void sendReport(long groupId, string message)
        {
            try
            {
                MiraiHelper.Session.SendGroupMessageAsync(groupId, new PlainMessage(message)).Wait();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }




    }
}
