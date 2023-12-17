using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Services
{
    internal class RequestRecordService
    {
        private RequestRecordDao requestRecordDao;

        public RequestRecordService()
        {
            requestRecordDao = new RequestRecordDao();
        }

        public int GetUsedCountToday(long groupId, long memberId, params CommandType[] commandTypeArr)
        {
            return requestRecordDao.GetUsedCountToday(groupId, memberId, commandTypeArr);
        }

        public RequestRecordPO InsertRecord(long groupId, long memberId, CommandType commandType, string sendWord)
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
