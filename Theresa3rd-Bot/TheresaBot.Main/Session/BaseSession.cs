using TheresaBot.Main.Model.Content;

namespace TheresaBot.Main.Session
{
    public abstract class BaseSession
    {
        public abstract Task<int> SendGroupMessageAsync(long groupId, string message);

        public abstract Task<int> SendGroupMessageAsync(long groupId, params BaseContent[] contents);

        public abstract Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents);

        public abstract Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents, bool isAtAll = false);

        public abstract Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents, List<long> atMembers, bool isAtAll = false);

        public abstract Task SendGroupSetuAsync(List<BaseContent> workMsgs, List<FileInfo> setuFiles, long groupId, bool isShowImg);

        public abstract Task<int> SendFriendMessageAsync(long memberId, string message);

        public abstract Task<int> SendFriendMessageAsync(long memberId, params BaseContent[] contents);

        public abstract Task<int> SendFriendMessageAsync(long memberId, List<BaseContent> contents);
    }
}
