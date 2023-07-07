using TheresaBot.Main.Dao;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

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

        public List<ImageRecordPO> GetImageRecord(PlatformType platformType, long msgId, long groupId)
        {
            return imageRecordDao.getRecord(platformType, msgId, groupId);
        }

        public async Task AddImageRecord(List<string> imgUrls, PlatformType platformType, long msgId, long groupId, long memberId)
        {
            foreach (var imgUrl in imgUrls) await AddImageRecord(platformType, imgUrl, msgId, groupId, memberId);
        }

        public async Task AddPixivRecord(SetuContent setucontent, PlatformType platformType, long[] msgIds, long groupId)
        {
            foreach (var msgId in msgIds) await AddPixivRecord(setucontent, platformType, msgId, groupId);
        }

        public async Task AddPixivRecord(List<SetuContent> setucontents, PlatformType platformType, long msgId, long groupId)
        {
            foreach (var setucontent in setucontents) await AddPixivRecord(setucontent, platformType, msgId, groupId);
        }

        public async Task AddPixivRecord(SetuContent setucontent, PlatformType platformType, long msgId, long groupId)
        {
            try
            {
                if (msgId == 0) return;
                if (setucontent is not PixivSetuContent pixivContent) return;
                PixivRecordPO pixivRecord = new PixivRecordPO();
                pixivRecord.MessageId = msgId;
                pixivRecord.PlatformType = platformType;
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

        public async Task AddImageRecord(PlatformType platformType, string imgUrl, long msgId, long groupId, long memberId)
        {
            try
            {
                if (msgId == 0) return;
                if (string.IsNullOrWhiteSpace(imgUrl)) return;
                ImageRecordPO imageRecord = new ImageRecordPO();
                imageRecord.MessageId = msgId;
                imageRecord.PlatformType = platformType;
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

        public async Task AddMessageRecord(PlatformType platformType, string plainMessage, long msgId, long groupId, long memberId)
        {
            try
            {
                if (msgId == 0) return;
                if (string.IsNullOrWhiteSpace(plainMessage)) return;
                MessageRecordPO messageRecord = new MessageRecordPO();
                messageRecord.MessageId = msgId;
                messageRecord.PlatformType = platformType;
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
