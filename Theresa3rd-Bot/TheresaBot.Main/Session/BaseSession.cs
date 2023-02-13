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

        public abstract Task<int> SendGroupMergeMessageAsync(long groupId, params List<BaseContent>[] contentLists);

        public abstract Task<int> SendGroupMergeMessageAsync(long groupId, List<SetuContent> setuContents);

        public abstract Task<int[]> SendGroupSetuAsync(SetuContent setuContent, long groupId);

        public abstract Task<int[]> SendGroupSetuAsync(List<SetuContent> setuContents, long groupId, bool sendMerge);

        public abstract Task<int[]> SendGroupMergeSetuAsync(List<SetuContent> setuContents, List<SetuContent> headerContents, long groupId, int eachPage);

        public abstract Task<int[]> SendGroupSetuAsync(List<BaseContent> workMsgs, List<FileInfo> setuFiles, long groupId);

        public abstract Task<int> SendFriendMessageAsync(long memberId, string message);

        public abstract Task<int> SendFriendMessageAsync(long memberId, params BaseContent[] contents);

        public abstract Task<int> SendFriendMessageAsync(long memberId, List<BaseContent> contents);
    }
}
