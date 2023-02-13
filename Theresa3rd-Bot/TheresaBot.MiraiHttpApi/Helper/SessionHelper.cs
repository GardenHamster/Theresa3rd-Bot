using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Session;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheresaBot.MiraiHttpApi.Helper
{
    public static class SessionHelper
    {
        public static async Task<int> SendGroupMessageWithoutImageAsync(this IMiraiHttpSession session, long groupId, List<IChatMessage> chainList)
        {
            IChatMessage[] msgArr = chainList.Where(o => o is not IImageMessage).ToArray();
            if (msgArr.Length == 0) return 0;
            return await session.SendGroupMessageAsync(groupId, msgArr);
        }

    }
}
