using Microsoft.Extensions.DependencyInjection;
using Mirai.CSharp.Builders;
using Mirai.CSharp.HttpApi.Builder;
using Mirai.CSharp.HttpApi.Invoking;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Options;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Event;

namespace TheresaBot.MiraiHttpApi.Helper
{
    public static class MiraiHelper
    {
        public static IServiceProvider Services;

        public static IServiceScope Scope;

        public static IMiraiHttpSession Session;

        public static async Task ConnectMirai()
        {
            try
            {
                LogHelper.Info("尝试连接到mirai-console...");
                Services = new ServiceCollection().AddMiraiBaseFramework()
                                                               .Services
                                                               .AddDefaultMiraiHttpFramework()
                                                               .AddInvoker<MiraiHttpMessageHandlerInvoker>()
                                                               .AddHandler<BotInvitedJoinGroupEvent>()
                                                               .AddHandler<FriendMessageEvent>()
                                                               .AddHandler<GroupApplyEvent>()
                                                               .AddHandler<GroupMessageEvent>()
                                                               .AddHandler<NewFriendApplyEvent>()
                                                               .AddHandler<GroupMemberJoinedEvent>()
                                                               .AddHandler<DisconnectedEvent>()
                                                               .AddClient<MiraiHttpSession>()
                                                               .Services
                                                               .Configure<MiraiHttpSessionOptions>(options =>
                                                               {
                                                                   options.Host = BotConfig.MiraiConfig.Host;
                                                                   options.Port = BotConfig.MiraiConfig.Port;
                                                                   options.AuthKey = BotConfig.MiraiConfig.AuthKey;
                                                                   options.SuppressAwaitMessageInvoker = true;
                                                               })
                                                               .AddLogging()
                                                               .BuildServiceProvider();
                Scope = Services.CreateAsyncScope();
                Services = Scope.ServiceProvider;
                Session = Services.GetRequiredService<IMiraiHttpSession>();
                await Session.ConnectAsync(BotConfig.MiraiConfig.BotQQ);
                LogHelper.Info("已成功连接到mirai-console...");
            }
            catch (Exception ex)
            {
                LogHelper.FATAL(ex, "连接到mirai-console失败");
                throw;
            }
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static MiraiGroupCommand CheckCommand(this string instruction, CommandHandler<GroupCommand> handler, IMiraiHttpSession session, IGroupMessageEventArgs args, long groupId, long memberId)
        {
            if (handler.Commands is null || handler.Commands.Count == 0) return null;
            foreach (string command in handler.Commands)
            {
                if (instruction.CheckCommand(handler, session, args, command, groupId, memberId) is { } botCommand) return botCommand;
            }
            return null;
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static MiraiGroupCommand CheckCommand(this string instruction, CommandHandler<GroupCommand> handler, IMiraiHttpSession session, IGroupMessageEventArgs args, string command, long groupId, long memberId)
        {
            if (string.IsNullOrWhiteSpace(command)) return null;
            string lowerInstructions = instruction.ToLower().Trim();
            string lowerCommand = command.ToLower().Trim();
            if (lowerInstructions.StartsWith(lowerCommand) == false) return null;
            string[] keyWords = instruction.splitKeyWords(command);
            return new(handler, args.GetMessageId(), session, args, keyWords, instruction, groupId, memberId);
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static MiraiFriendCommand CheckCommand(this string instruction, CommandHandler<FriendCommand> handler, IMiraiHttpSession session, IFriendMessageEventArgs args, long memberId)
        {
            if (handler.Commands is null || handler.Commands.Count == 0) return null;
            foreach (string command in handler.Commands)
            {
                if (instruction.CheckCommand(handler, session, args, command, memberId) is { } botCommand) return botCommand;
            }
            return null;
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static MiraiFriendCommand CheckCommand(this string instruction, CommandHandler<FriendCommand> handler, IMiraiHttpSession session, IFriendMessageEventArgs args, string command, long memberId)
        {
            if (string.IsNullOrWhiteSpace(command)) return null;
            string lowerInstructions = instruction.ToLower().Trim();
            string lowerCommand = command.ToLower().Trim();
            if (lowerInstructions.StartsWith(lowerCommand) == false) return null;
            string[] keyWords = instruction.splitKeyWords(command);
            return new(handler, args.GetMessageId(), session, args, keyWords, instruction, memberId);
        }


        public static int GetMessageId(this IGroupMessageEventArgs args)
        {
            try
            {
                SourceMessage sourceMessage = (SourceMessage)args.Chain.First();
                return sourceMessage.Id;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "GetMessageId异常");
                return 0;
            }
        }

        public static int GetMessageId(this IFriendMessageEventArgs args)
        {
            try
            {
                SourceMessage sourceMessage = (SourceMessage)args.Chain.First();
                return sourceMessage.Id;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "GetMessageId异常");
                return 0;
            }
        }

        public static async Task<List<IChatMessage>> ToMiraiMessageAsync(this List<BaseContent> chatContents)
        {
            List<IChatMessage> chatList = new List<IChatMessage>();
            foreach (BaseContent content in chatContents)
            {
                IChatMessage chatMessage = await content.ToMiraiMessageAsync();
                if(chatMessage is not null) chatList.Add(chatMessage);
            }
            return chatList;
        }

        public static async Task<IChatMessage> ToMiraiMessageAsync(this BaseContent chatContent)
        {
            if (chatContent is PlainContent plainContent)
            {
                return new PlainMessage(plainContent.Content);
            }
            if (chatContent is LocalImageContent localImageContent)
            {
                return await UploadPictureAsync(localImageContent);
            }
            if (chatContent is WebImageContent webImageContent)
            {
                return new ImageMessage(null, webImageContent.Url, null);
            }
            return null;
        }

        public static async Task<List<IChatMessage>> UploadPictureAsync(List<FileInfo> setuFiles, UploadTarget target)
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

        private static async Task<IChatMessage> UploadPictureAsync(LocalImageContent imageContent)
        {
            return imageContent.SendTarget switch
            {
                SendTarget.Group => (IImageMessage)await Session.UploadPictureAsync(UploadTarget.Group, imageContent.FullFilePath),
                SendTarget.Friend => (IImageMessage)await Session.UploadPictureAsync(UploadTarget.Friend, imageContent.FullFilePath),
                SendTarget.Temp => (IImageMessage)await Session.UploadPictureAsync(UploadTarget.Temp, imageContent.FullFilePath),
                _ => null
            };
        }

    }
}
