using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Helper
{
    public static class SessionHelper
    {
        public static async Task<int[]> SendGroupMessageAsync(this BaseSession session, long groupId, SetuContent setuContent, bool sendImgBehind)
        {
            if (setuContent is null) return new int[0];
            List<BaseContent> setuInfos = setuContent.SetuInfos ?? new();
            List<FileInfo> setuFiles = setuContent.SetuImages ?? new();
            if (sendImgBehind)
            {
                int workMsgId = await session.SendGroupMessageAsync(groupId, setuInfos);
                await Task.Delay(1000);
                int imgMsgId = await session.SendGroupMessageAsync(groupId, setuFiles.ToBaseContent());
                return new int[] { workMsgId, imgMsgId };
            }
            else
            {
                List<BaseContent> msgList = setuInfos.Concat(setuFiles.ToBaseContent()).ToList();
                int msgId = await session.SendGroupMessageAsync(groupId, msgList.ToArray());
                return new int[] { msgId };
            }
        }

        public static async Task<int> SendGroupMergeAsync(this BaseSession session, long groupId, List<SetuContent> setuContents)
        {
            if (setuContents.Count == 0) return 0;
            return await session.SendGroupMergeAsync(groupId, setuContents.ToBaseContents());
        }


    }
}
