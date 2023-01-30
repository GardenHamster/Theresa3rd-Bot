using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Session;

namespace TheresaBot.MiraiHttpApi.Session
{
    public class MiraiSession : BaseSession
    {
        public override Task<int> SendFriendMessageAsync(long memberId, string message)
        {

        }

        public override Task<int> SendFriendMessageAsync(long memberId, params BaseContent[] contents)
        {

        }

        public override Task<int> SendFriendMessageAsync(long memberId, List<BaseContent> contents)
        {

        }

        public override Task<int> SendGroupMessageAsync(long groupId, string message)
        {

        }

        public override Task<int> SendGroupMessageAsync(long groupId, params BaseContent[] contents)
        {

        }

        public override Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents)
        {

        }

        public override Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents, bool isAtAll = false)
        {

        }

        public override Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents, List<long> atMembers, bool isAtAll = false)
        {

        }

        public override Task SendGroupSetuAsync(List<BaseContent> workMsgs, List<FileInfo> setuFiles, long groupId, bool isShowImg)
        {

        }
    }
}
