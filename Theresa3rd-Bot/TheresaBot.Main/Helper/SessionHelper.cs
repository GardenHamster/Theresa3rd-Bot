using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Result;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Helper
{
    public static class SessionHelper
    {
        public static async Task<BaseResult[]> SendGroupMessageAsync(this BaseSession session, long groupId, SetuContent setuContent, bool sendImgBehind)
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

        public static async Task<BaseResult> SendGroupMergeAsync(this BaseSession session, long groupId, List<SetuContent> setuContents)
        {
            return await session.SendGroupMergeAsync(groupId, setuContents.ToBaseContents().SetDefaultImage());
        }

        

    }
}
