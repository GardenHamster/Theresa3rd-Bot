using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Helper
{
    public static class SessionHelper
    {
        public static async Task<long?[]> SendGroupMessageAsync(this BaseSession session, long groupId, SetuContent setuContent, bool sendImgBehind)
        {
            List<BaseContent> msgContents = setuContent.SetuInfos ?? new();
            List<BaseContent> imgContents = setuContent.SetuImages.ToBaseContent().SetDefaultImage().ToList();
            if (sendImgBehind)
            {
                long? workMsgId = await session.SendGroupMessageAsync(groupId, msgContents);
                await Task.Delay(1000);
                long? imgMsgId = await session.SendGroupMessageAsync(groupId, imgContents);
                return new long?[] { workMsgId, imgMsgId };
            }
            else
            {
                List<BaseContent> msgList = msgContents.Concat(imgContents).ToList();
                long msgId = await session.SendGroupMessageAsync(groupId, msgList.ToArray());
                return new long?[] { msgId };
            }
        }

        public static async Task<long?> SendGroupMergeAsync(this BaseSession session, long groupId, List<SetuContent> setuContents)
        {
            if (setuContents.Count == 0) return 0;
            return await session.SendGroupMergeAsync(groupId, setuContents.ToBaseContents().SetDefaultImage());
        }

        

    }
}
