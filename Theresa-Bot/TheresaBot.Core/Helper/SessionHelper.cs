using TheresaBot.Core.Model.Content;
using TheresaBot.Core.Model.Result;
using TheresaBot.Core.Session;

namespace TheresaBot.Core.Helper
{
    public static class SessionHelper
    {
        public static async Task<BaseResult> SendGroupMessageWithAtAsync(this BaseSession session, long groupId, List<long> atMembers, string message)
        {
            List<BaseContent> contents = new List<BaseContent> { new PlainContent(message) };
            return await session.SendGroupMessageAsync(groupId, contents, atMembers);
        }

        public static async Task<BaseResult> SendGroupMessageWithAtAsync(this BaseSession session, long groupId, List<long> atMembers, List<BaseContent> contents)
        {
            return await session.SendGroupMessageAsync(groupId, contents, atMembers);
        }

        public static async Task<BaseResult[]> SendGroupSetuAsync(this BaseSession session, long groupId, SetuContent setuContent, bool sendImgBehind)
        {
            List<BaseContent> msgContents = setuContent.SetuInfos ?? new();
            List<BaseContent> imgContents = setuContent.SetuImages.ToBaseContent().SetDefaultImage().ToList();
            if (sendImgBehind)
            {
                BaseResult workMsgResult = await session.SendGroupMessageAsync(groupId, msgContents);
                await Task.Delay(1000);
                BaseResult imgMsgResult = await session.SendGroupMessageAsync(groupId, imgContents);
                return new BaseResult[] { workMsgResult, imgMsgResult };
            }
            else
            {
                List<BaseContent> msgList = msgContents.Concat(imgContents).ToList();
                BaseResult result = await session.SendGroupMessageAsync(groupId, msgList);
                return new BaseResult[] { result };
            }
        }

        public static async Task<BaseResult> SendGroupMergeSetuAsync(this BaseSession session, List<SetuContent> setuContents, List<BaseContent[]> headerContents, long groupId)
        {
            List<BaseContent[]> sendContents = new List<BaseContent[]>();
            sendContents.AddRange(headerContents);
            sendContents.AddRange(setuContents.ToBaseContents().SetDefaultImage());
            return await session.SendGroupMergeAsync(groupId, sendContents);
        }

        public static async Task<BaseResult> SendGroupTemplateAsync(this BaseSession session, long groupId, string template, string defaultmsg = "")
        {
            template = template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return BaseResult.Undo;
            if (template.StartsWith(" ") == false) template = " " + template;
            return await session.SendGroupMessageAsync(groupId, template.SplitToChainAsync());
        }

        public static async Task ReplyGroupErrorAsync(this BaseSession session, Exception exception, long groupId)
        {
            try
            {
                List<BaseContent> contents = exception.GetErrorContents();
                await session.SendGroupMessageAsync(groupId, contents);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReplyGroupErrorAsync异常");
            }
        }

        public static async Task SendFriendMessageAsync(this BaseSession session, List<long> memberIds, string message, int delay = 1000)
        {
            foreach (var memberId in memberIds)
            {
                try
                {
                    await Task.Delay(delay);
                    await session.SendFriendMessageAsync(memberId, message);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "SendFriendMessageAsync异常");
                }
            }
        }

        public static async Task SendTempMessageAsync(this BaseSession session, long groupId, List<long> memberIds, string message, int delay = 1000)
        {
            foreach (var memberId in memberIds)
            {
                try
                {
                    await Task.Delay(delay);
                    await session.SendTempMessageAsync(groupId, memberId, message);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "SendFriendMessageAsync异常");
                }
            }
        }

        public static async Task ReplyFriendErrorAsync(this BaseSession session, Exception exception, long memberId)
        {
            try
            {
                List<BaseContent> contents = exception.GetErrorContents();
                await session.SendFriendMessageAsync(memberId, contents);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReplyFriendErrorAsync异常");
            }
        }

        public static async Task MuteGroupMemberAsync(this BaseSession session, long groupId, List<long> memberIds, int seconds)
        {
            try
            {
                foreach (var memberId in memberIds)
                {
                    await session.MuteGroupMemberAsync(groupId, memberId, seconds);
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReplyFriendErrorAsync异常");
            }
        }





    }
}
