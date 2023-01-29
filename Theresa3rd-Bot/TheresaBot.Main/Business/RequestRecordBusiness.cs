using System;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Business
{
    public class RequestRecordBusiness
    {
        private RequestRecordDao requestRecordDao;

        public RequestRecordBusiness()
        {
            this.requestRecordDao = new RequestRecordDao();
        }

        public int getUsedCountToday(long groupId, long memberId, params CommandType[] commandTypeArr)
        {
            lock (this) return requestRecordDao.getUsedCountToday(groupId, memberId, commandTypeArr);
        }

        public RequestRecordPO addRecord(long groupId, long memberId, CommandType commandType, string sendWord)
        {
            if (sendWord is null) sendWord = "";
            if (sendWord.Length > 100) sendWord = sendWord.Substring(0, 100);
            RequestRecordPO requestRecord = new RequestRecordPO();
            requestRecord.GroupId = groupId;
            requestRecord.MemberId = memberId;
            requestRecord.CommandType = commandType;
            requestRecord.SendWord = sendWord;
            requestRecord.CreateDate = DateTime.Now;
            return requestRecordDao.Insert(requestRecord);
        }


    }
}
