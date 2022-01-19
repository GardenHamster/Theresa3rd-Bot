using Mirai.CSharp.HttpApi.Models.EventArgs;
using System;
using Theresa3rd_Bot.Dao;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Business
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

        public RequestRecordPO getLastRecord(long groupId, long memberId, CommandType commandType)
        {
            lock (this) return requestRecordDao.getLastRecord(groupId, memberId, commandType);
        }

        public RequestRecordPO addRecord(IGroupMessageEventArgs args, CommandType commandType, string sendWord)
        {
            long groupId = args.Sender.Group.Id;
            long memberId = args.Sender.Id;
            return addRecord(groupId, memberId, commandType, sendWord);
        }

        public RequestRecordPO addRecord(IFriendMessageEventArgs args, CommandType commandType, string sendWord)
        {
            long memberId = args.Sender.Id;
            return addRecord(0, memberId, commandType, sendWord);
        }

        public RequestRecordPO addRecord(long groupId, long memberId, CommandType commandType, string sendWord)
        {
            if (sendWord == null) sendWord = "";
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
