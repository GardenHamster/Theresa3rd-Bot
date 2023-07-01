using TheresaBot.Main.Model.Content;

namespace TheresaBot.Main.Session
{
    public abstract class BaseSession
    {
        public abstract Task<long> SendGroupMessageAsync(long groupId, string message);

        public abstract Task<long> SendGroupMessageAsync(long groupId, params BaseContent[] contents);

        public abstract Task<long> SendGroupMessageAsync(long groupId, List<BaseContent> contents);

        public abstract Task<long> SendGroupMessageAsync(long groupId, List<BaseContent> contents, bool isAtAll = false);

        public abstract Task<long> SendGroupMessageAsync(long groupId, List<BaseContent> contents, List<long> atMembers, bool isAtAll = false);

        public abstract Task<long> SendGroupMergeAsync(long groupId, params List<BaseContent>[] contentLists);

        public abstract Task<long> SendFriendMessageAsync(long memberId, string message);

        public abstract Task<long> SendFriendMessageAsync(long memberId, params BaseContent[] contents);

        public abstract Task<long> SendFriendMessageAsync(long memberId, List<BaseContent> contents);
    }
}
