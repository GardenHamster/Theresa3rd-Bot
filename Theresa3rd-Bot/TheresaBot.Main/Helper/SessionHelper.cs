using TheresaBot.Main.Business;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Helper
{
    public static class SessionHelper
    {
        public static async Task<long?[]> SendGroupMessageAsync(this BaseSession session, long groupId, SetuContent setuContent, bool sendImgBehind)
        {
            if (setuContent is null) return new long?[0];
            List<BaseContent> setuInfos = setuContent.SetuInfos ?? new();
            List<FileInfo> setuFiles = setuContent.SetuImages ?? new();
            if (sendImgBehind)
            {
                long? workMsgId = await session.SendGroupMessageAsync(groupId, setuInfos);
                await Task.Delay(1000);
                long? imgMsgId = await session.SendGroupMessageAsync(groupId, setuFiles.ToBaseContent());
                return new long?[] { workMsgId, imgMsgId };
            }
            else
            {
                List<BaseContent> msgList = setuInfos.Concat(setuFiles.ToBaseContent()).ToList();
                long msgId = await session.SendGroupMessageAsync(groupId, msgList.ToArray());
                return new long?[] { msgId };
            }
        }

        public static async Task<long?> SendGroupMergeAsync(this BaseSession session, long groupId, List<SetuContent> setuContents)
        {
            if (setuContents.Count == 0) return 0;
            return await session.SendGroupMergeAsync(groupId, setuContents.ToBaseContents());
        }

        

    }
}
