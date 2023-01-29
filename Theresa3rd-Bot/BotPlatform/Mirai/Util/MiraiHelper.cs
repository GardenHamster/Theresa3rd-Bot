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
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.BotPlatform.Base.Command;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Event;
using Theresa3rd_Bot.Model.Command;
using Theresa3rd_Bot.Model.Content;
using Theresa3rd_Bot.Model.Invoker;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.BotPlatform.Mirai.Util
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
            return new(handler, session, args, keyWords, instruction, groupId, memberId);
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
            return new(handler, session, args, keyWords, instruction, memberId);
        }


        public static int GetMessageId(this IGroupMessageEventArgs args)
        {
            SourceMessage sourceMessage = (SourceMessage)args.Chain.First();
            return sourceMessage.Id;
        }

        public static async Task<List<IChatMessage>> ToMiraiMessageAsync(this List<ChatContent> chatContents)
        {
            List<IChatMessage> chatList = new List<IChatMessage>();
            foreach (ChatContent content in chatContents)
            {
                IChatMessage chatMessage = await content.ToMiraiMessageAsync();
                if(chatMessage is not null) chatList.Add(chatMessage);
            }
            return chatList;
        }

        public static async Task<IChatMessage> ToMiraiMessageAsync(this ChatContent chatContent)
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

        private static async Task<IImageMessage> UploadPictureAsync(LocalImageContent imageContent)
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
