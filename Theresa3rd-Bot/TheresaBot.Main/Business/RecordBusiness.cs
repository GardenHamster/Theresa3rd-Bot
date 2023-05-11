using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task AddPixivRecord(SetuContent setucontent, int[] msgIds)
        {
            foreach (var msgId in msgIds) await AddPixivRecord(setucontent, msgId);
        }

        public async Task AddPixivRecord(List<SetuContent> setucontents, int msgId)
        {
            foreach (var setucontent in setucontents) await AddPixivRecord(setucontent, msgId);
        }

        public async Task AddPixivRecord(SetuContent setucontent, int msgId)
        {
            try
            {
                if (msgId <= 0) return;
                if (setucontent is not PixivSetuContent) return;
                PixivSetuContent pixivContent = (PixivSetuContent)setucontent;
                PixivRecordPO pixivRecord = new PixivRecordPO();
                pixivRecord.MessageId = msgId;
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

    }
}
