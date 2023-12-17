using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Dao
{
    public class RequestRecordDao : DbContext<RequestRecordPO>
    {
        public int GetUsedCountToday(long groupId, long memberId, params CommandType[] commandTypeArr)
        {
            DateTime todayStart = DateTimeHelper.GetDayStart();
            DateTime todayEnd = DateTimeHelper.GetDayEnd();
            return Db.Queryable<RequestRecordPO>().Where(o => o.GroupId == groupId && o.MemberId == memberId && commandTypeArr.Contains(o.CommandType) && o.CreateDate >= todayStart && o.CreateDate <= todayEnd).Count();
        }

        public RequestRecordPO GetLastRecord(long groupId, long memberId, CommandType commandType)
        {
            return Db.Queryable<RequestRecordPO>().Where(o => o.GroupId == groupId && o.MemberId == memberId && o.CommandType == commandType).OrderBy(o => o.CreateDate, SqlSugar.OrderByType.Desc).First();
        }


    }
}
