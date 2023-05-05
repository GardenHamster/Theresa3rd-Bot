
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Helper
{
    public static class SessionHelper
    {
        public static async Task<int[]> SendGroupMessageAsync(this BaseSession session, long groupId, SetuContent setuContent, bool sendImgBehind)
        {
            if (setuContent is null) return new int[0];
            List<int> msgIds = new List<int>();
            List<BaseContent> setuInfos = setuContent.SetuInfos ?? new();
            List<FileInfo> setuFiles = setuContent.SetuImages ?? new();
            if (sendImgBehind)
            {
                msgIds.Add(await session.SendGroupMessageAsync(groupId, setuInfos));
                await Task.Delay(1000);
                msgIds.Add(await session.SendGroupMessageAsync(groupId, setuFiles.ToBaseContent()));
            }
            else
            {
                List<BaseContent> msgList = setuInfos.Concat(setuFiles.ToBaseContent()).ToList();
                msgIds.Add(await session.SendGroupMessageAsync(groupId, msgList.ToArray()));
            }
            return msgIds.ToArray();
        }

        public static async Task<int> SendGroupMergeAsync(this BaseSession session, long groupId, List<SetuContent> setuContents)
        {
            if (setuContents.Count == 0) return 0;
            return await session.SendGroupMergeAsync(groupId, setuContents.ToBaseContents());
        }


    }
}
