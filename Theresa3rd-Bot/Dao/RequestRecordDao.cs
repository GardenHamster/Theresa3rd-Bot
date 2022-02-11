using System;
using System.Linq;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Dao
{
    public class RequestRecordDao : DbContext<RequestRecordPO>
    {
        public int getUsedCountToday(long groupId, long memberId, params CommandType[] commandTypeArr)
        {
            DateTime todayStart = DateTimeHelper.GetTodayStart();
            DateTime todayEnd = DateTimeHelper.GetTodayEnd();
            return Db.Queryable<RequestRecordPO>().Where(o => o.GroupId == groupId && o.MemberId == memberId && commandTypeArr.Contains(o.CommandType) && o.CreateDate >= todayStart && o.CreateDate <= todayEnd).Count();
        }

        public RequestRecordPO getLastRecord(long groupId, long memberId, CommandType commandType)
        {
            return Db.Queryable<RequestRecordPO>().Where(o => o.GroupId == groupId && o.MemberId == memberId && o.CommandType == commandType).OrderBy(o => o.CreateDate, SqlSugar.OrderByType.Desc).First();
        }


    }
}
