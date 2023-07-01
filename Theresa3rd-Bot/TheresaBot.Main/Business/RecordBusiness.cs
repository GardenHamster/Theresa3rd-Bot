using TheresaBot.Main.Command;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Business
{
    public class RecordBusiness
    {
        private MessageRecordDao messageRecordDao;
        private PixivRecordDao pixivRecordDao;
        private ImageRecordDao imageRecordDao;

        public RecordBusiness()
        {
            this.messageRecordDao = new MessageRecordDao();
            this.pixivRecordDao = new PixivRecordDao();
            this.imageRecordDao = new ImageRecordDao();
        }

        public List<ImageRecordPO> GetImageRecord(long msgId, long groupId)
        {
            return imageRecordDao.getRecord(msgId, groupId);
        }

        public async Task AddImageRecord(List<string> imgUrls, long msgId, long groupId, long memberId)
        {
            foreach (var imgUrl in imgUrls) await AddImageRecord(imgUrl, msgId, groupId, memberId);
        }

        public async Task AddPixivRecord(SetuContent setucontent, long[] msgIds, long groupId)
        {
            foreach (var msgId in msgIds) await AddPixivRecord(setucontent, msgId, groupId);
        }

        public async Task AddPixivRecord(List<SetuContent> setucontents, long msgId, long groupId)
        {
            foreach (var setucontent in setucontents) await AddPixivRecord(setucontent, msgId, groupId);
        }

        public async Task AddPixivRecord(SetuContent setucontent, long msgId, long groupId)
        {
            try
            {
                if (msgId <= 0) return;
                if (setucontent is not PixivSetuContent) return;
                PixivSetuContent pixivContent = (PixivSetuContent)setucontent;
                PixivRecordPO pixivRecord = new PixivRecordPO();
                pixivRecord.MessageId = msgId;
                pixivRecord.GroupId = groupId;
                pixivRecord.PixivId = pixivContent.WorkInfo.PixivId;
                pixivRecord.Title = pixivContent.WorkInfo.Title;
                pixivRecord.UserName = pixivContent.WorkInfo.UserName;
                pixivRecord.CreateDate = DateTime.Now;
                pixivRecordDao.Insert(pixivRecord);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        public async Task AddImageRecord(string imgUrl, long msgId, long groupId, long memberId)
        {
            try
            {
                if (msgId <= 0) return;
                if (string.IsNullOrWhiteSpace(imgUrl)) return;
                ImageRecordPO imageRecord = new ImageRecordPO();
                imageRecord.MessageId = msgId;
                imageRecord.GroupId = groupId;
                imageRecord.MemberId = memberId;
                imageRecord.HttpUrl = imgUrl.Trim();
                imageRecord.CreateDate = DateTime.Now;
                imageRecordDao.Insert(imageRecord);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        public async Task AddMessageRecord(string plainMessage, long msgId, long groupId, long memberId)
        {
            try
            {
                if (msgId <= 0) return;
                if (string.IsNullOrWhiteSpace(plainMessage)) return;
                MessageRecordPO messageRecord = new MessageRecordPO();
                messageRecord.MessageId = msgId;
                messageRecord.GroupId = groupId;
                messageRecord.MemberId = memberId;
                messageRecord.MessageText = plainMessage;
                messageRecord.CreateDate = DateTime.Now;
                messageRecordDao.Insert(messageRecord);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }



    }
}
