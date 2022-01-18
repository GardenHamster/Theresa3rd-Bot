using System;

namespace Theresa3rd_Bot.Model.Cache
{
    public class GroupCoolingInfo
    {
        public long GroupId { get; set; }

        public DateTime? LastGetSTTime { get; set; }

        public DateTime? LastSaucenaoTime { get; set; }

        public GroupCoolingInfo(long groupId)
        {
            this.GroupId = groupId;
        }

    }
}
