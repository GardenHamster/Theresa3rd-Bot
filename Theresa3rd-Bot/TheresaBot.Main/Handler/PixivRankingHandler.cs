using TheresaBot.Main.Business;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PixivRanking;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    public class PixivRankingHandler : SetuHandler
    {
        private PixivBusiness pixivBusiness;
        private PixivRankingBusiness rankingBusiness;

        public PixivRankingHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivBusiness = new PixivBusiness();
            rankingBusiness = new PixivRankingBusiness();
        }

        public async Task sendDailyRanking(GroupCommand command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BotConfig.PixivRankingConfig.ProcessingMsg) == false)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.PixivRankingConfig.ProcessingMsg);
                }
                PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Daily;
                await sendRankingPreview(command, rankingItem, "日榜", "daily");
            }
            catch (Exception ex)
            {
                string errMsg = $"sendDailyRanking异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.ErrorMsg, "出了点小问题，再试一次吧~");
                Reporter.SendError(ex, errMsg);
            }
        }

        public Task sendDailyAIRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        public Task sendDailyMaleRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        public Task sendWeeklyRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        public Task sendMonthlyRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        private async Task sendRankingPreview(GroupCommand command, PixivRankingItem rankingItem, string rankingName, string mode)
        {
            (List<PixivRankingContent> rankingContents, string date) = await rankingBusiness.getRankingDatas(rankingItem, mode);
            List<PixivRankingPreview> rankingPreviews = await rankingBusiness.getRankingPreviews(rankingContents);
            List<FileInfo> previewImgs = createPreviewImg(rankingPreviews, mode, date);
            List<SetuContent> setuContents = new List<SetuContent>();
            setuContents.AddRange(previewImgs.Select(o => new SetuContent(o)));

            string template = BotConfig.PixivRankingConfig.Template;
            string rankingInfo = rankingBusiness.getRankingInfo(date, rankingName, template);
            await command.ReplyGroupMessageWithAtAsync(rankingInfo);
            await Task.Delay(1000);
            await Session.SendGroupSetuAsync(setuContents, command.GroupId,BotConfig.PixivRankingConfig.SendMerge);
            await Task.Delay(1000);
        }

        private async Task<List<SetuContent>> getSetuContent(List<PixivWorkInfo> datas, PixivRankingItem rankingItem, long groupId, bool r18Content)
        {
            List<SetuContent> setuContents = new List<SetuContent>();
            foreach (var data in datas)
            {
                SetuContent setuContent = await getSetuContent(data, rankingItem, groupId, r18Content);
                setuContents.Add(setuContent);
            }
            return setuContents;
        }

        private async Task<SetuContent> getSetuContent(PixivWorkInfo data, PixivRankingItem rankingItem, long groupId, bool r18Content)
        {
            try
            {
                bool isR18Img = r18Content || data.IsR18;
                bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                string setuInfo = pixivBusiness.getWorkInfo(data, DateTime.Now, BotConfig.PixivConfig.Template);
                List<FileInfo> setuFiles = isShowImg ? await downPixivImgsAsync(data) : new();
                return new SetuContent(setuInfo, setuFiles);
            }
            catch (ApiException ex)
            {
                return new SetuContent(ex.Message, new List<FileInfo>());
            }
        }

        private List<FileInfo> createPreviewImg(List<PixivRankingPreview> datas, string mode, string date)
        {
            int startIndex = 0;
            List<FileInfo> fileInfos = new List<FileInfo>();
            int maxInPage = BotConfig.PixivRankingConfig.MaxInPage;
            if (maxInPage <= 0) maxInPage = 35;
            if (datas.Count == 0) return fileInfos;

            while (startIndex < datas.Count)
            {
                string fileName = $"{mode}_preview_{date}.jpg";
                string savePath = Path.Combine(FilePath.GetDownFileSavePath(), fileName);
                var partList = datas.Skip(startIndex).Take(maxInPage).ToList();
                var previewFile = createPreviewImg(partList, savePath);
                if (previewFile is not null) fileInfos.Add(previewFile);
                startIndex += maxInPage;
            }
            return fileInfos;
        }

        private FileInfo createPreviewImg(List<PixivRankingPreview> datas, string savePath)
        {
            try
            {
                return PixivRankingDrawHelper.DrawPreview(datas, savePath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "日榜一览图合成失败");
                Reporter.SendError(ex, "日榜一览图合成失败");
                return null;
            }
        }


    }
}
