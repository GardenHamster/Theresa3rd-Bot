using AngleSharp.Media;
using SkiaSharp;
using System.Drawing;
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
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.SetuConfig.ProcessingMsg);
                }
                PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Daily;
                await sendRanking(command, rankingItem, "日榜", "daily", false);
                if (BotConfig.PixivRankingConfig.IncludeR18 && command.GroupId.IsShowR18Setu())
                {
                    await Task.Delay(1000);
                    await sendRanking(command, rankingItem, "R18日榜", "daily_r18", true);
                }
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

        private async Task sendRanking(GroupCommand command, PixivRankingItem rankingItem, string rankingName, string mode, bool r18)
        {
            SetuContent previewContent = null;
            List<SetuContent> setuContents = null;

            (List<PixivRankingContent> rankingContents, string date) = await rankingBusiness.getRankingDatas(rankingItem, mode);

            if (BotConfig.PixivRankingConfig.SendPreview && r18 == false)
            {
                string previewInfo = $"{date}{rankingName}一览图";
                List<PixivRankingPreview> rankingPreviews = await rankingBusiness.getRankingPreviews(rankingContents);
                FileInfo previewImg = createPreviewImg(rankingPreviews);
                previewContent = new SetuContent(previewInfo, previewImg);
            }

            if (BotConfig.PixivRankingConfig.SendDetail)
            {
                List<PixivWorkInfo> workInfos = await rankingBusiness.getRankingWorks(rankingItem, rankingContents);
                setuContents = await getSetuContent(workInfos, rankingItem, command.GroupId, r18);
            }

            string template = BotConfig.PixivRankingConfig.Template;
            string rankingInfo = rankingBusiness.getRankingInfo(date, rankingName, template);
            await command.ReplyGroupMessageWithAtAsync(rankingInfo);
            await Task.Delay(1000);

            if (previewContent is not null)
            {
                await Session.SendGroupSetuAsync(previewContent, command.GroupId);
                await Task.Delay(1000);
            }
            if (setuContents is not null)
            {
                bool sendMerge = BotConfig.PixivRankingConfig.SendMerge;
                await Session.SendGroupSetuAsync(setuContents, command.GroupId, sendMerge);
                await Task.Delay(1000);
            }
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

        public FileInfo createPreviewImg(List<PixivRankingPreview> previewFiles)
        {
            try
            {
                return DrawHelper.DrawPixivRankingPreview(previewFiles);
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
