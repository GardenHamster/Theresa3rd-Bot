using TheresaBot.Core.Dao;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Content;
using TheresaBot.Core.Model.PO;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Services
{
    public class RecordService
    {
        private MessageRecordDao messageRecordDao;
        private PixivRecordDao pixivRecordDao;
        private ImageRecordDao imageRecordDao;

        public RecordService()
        {
            messageRecordDao = new MessageRecordDao();
            pixivRecordDao = new PixivRecordDao();
            imageRecordDao = new ImageRecordDao();
        }

        public List<PixivRecordPO> GetPixivRecord(PlatformType platformType, long msgId, long groupId)
        {
            return pixivRecordDao.GetRecords(platformType, msgId, groupId);
        }

        public List<ImageRecordPO> GetImageRecord(PlatformType platformType, long msgId, long groupId)
        {
            return imageRecordDao.GetRecords(platformType, msgId, groupId);
        }

        public async Task InsertImageRecord(List<string> imgUrls, PlatformType platformType, long msgId, long groupId, long memberId)
        {
            foreach (var imgUrl in imgUrls) await InsertImageRecord(platformType, imgUrl, msgId, groupId, memberId);
        }

        public async Task InsertPixivRecord(SetuContent setucontent, PlatformType platformType, long[] msgIds, long groupId)
        {
            foreach (var msgId in msgIds) await InsertPixivRecord(setucontent, platformType, msgId, groupId);
        }

        public async Task InsertPixivRecord(List<SetuContent> setucontents, PlatformType platformType, long msgId, long groupId)
        {
            foreach (var setucontent in setucontents) await InsertPixivRecord(setucontent, platformType, msgId, groupId);
        }

        public async Task InsertPixivRecord(SetuContent setucontent, PlatformType platformType, long msgId, long groupId)
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
                pixivRecord.Title = pixivContent.WorkInfo.Title?.FilterEmoji()?.CutString(100);
                pixivRecord.UserId = pixivContent.WorkInfo.UserId;
                pixivRecord.UserName = pixivContent.WorkInfo.UserName?.FilterEmoji()?.CutString(100);
                pixivRecord.CreateDate = DateTime.Now;
                pixivRecordDao.Insert(pixivRecord);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        public async Task InsertImageRecord(PlatformType platformType, string imgUrl, long msgId, long groupId, long memberId)
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
                imageRecord.HttpUrl = imgUrl?.Trim();
                imageRecord.CreateDate = DateTime.Now;
                imageRecordDao.Insert(imageRecord);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        public async Task InsertMessageRecord(PlatformType platformType, string plainMessage, long msgId, long groupId, long memberId)
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
                messageRecord.MessageText = plainMessage?.FilterEmoji()?.CutString(1000);
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
