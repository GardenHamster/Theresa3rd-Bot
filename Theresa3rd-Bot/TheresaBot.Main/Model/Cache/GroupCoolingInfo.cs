using System;

namespace TheresaBot.Main.Model.Cache
{
    public class GroupCoolingInfo
    {
        public long GroupId { get; set; }

        public DateTime? LastGetSetuTime { get; set; }

        public DateTime? LastSaucenaoTime { get; set; }

        public GroupCoolingInfo(long groupId)
        {
            this.GroupId = groupId;
        }

    }
}
