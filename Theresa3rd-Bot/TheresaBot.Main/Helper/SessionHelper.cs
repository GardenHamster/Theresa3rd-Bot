using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Result;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Helper
{
    public static class SessionHelper
    {
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

        public static async Task<BaseResult> SendGroupMergeSetuAsync(this BaseSession session, List<SetuContent> setuContents, List<BaseContent> headerContents, long groupId)
        {
            List<BaseContent[]> sendContents = new List<BaseContent[]>();
            sendContents.AddRange(headerContents.ToArray());
            sendContents.AddRange(setuContents.ToBaseContents().SetDefaultImage());
            return await session.SendGroupMergeAsync(groupId, sendContents);
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



    }
}
