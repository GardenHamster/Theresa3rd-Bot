using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Mode;
using TheresaBot.Main.Model.Cache;
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
            PixivRankingMode rankingMode = PixivRankingMode.Daily;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Daily;
            await replyRankingInfo(command, rankingMode, rankingItem);
        }

        public async Task sendDailyAIRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.DailyAI;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.DailyAI;
            await replyRankingInfo(command, rankingMode, rankingItem);
        }

        public async Task sendMaleRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Male;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Male;
            await replyRankingInfo(command, rankingMode, rankingItem);
        }

        public async Task sendWeeklyRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Weekly;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Weekly;
            await replyRankingInfo(command, rankingMode, rankingItem);
        }

        public async Task sendMonthlyRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Monthly;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Monthly;
            await replyRankingInfo(command, rankingMode, rankingItem);
        }

        public async Task handleRankingSubscribeAsync(PixivRankingTimer rankingTimer, PixivRankingItem rankingItem, PixivRankingMode rankingMode)
        {
            try
            {
                CoolingCache.SetPixivRankingHanding();
                PixivRankingInfo rankingInfo = await rankingBusiness.getRankingInfo(rankingItem, rankingMode, String.Empty);
                PixivRankingCache.AddCache(rankingMode, rankingInfo, String.Empty);
                if (rankingInfo.RankingDetails.Count == 0) return;
                await sendPreviewFileAsync(rankingTimer, rankingInfo, rankingMode);
                await Task.Delay(2000);
                await sendSetuDetailAsync(rankingTimer, rankingInfo, rankingMode);
            }
            catch (Exception ex)
            {
                string errMsg = $"handleRankingSubscribeAsync异常";
                LogHelper.Error(ex, errMsg);
                Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetPixivRankingHandFinish();
            }
        }

        private async Task sendPreviewFileAsync(PixivRankingTimer rankingTimer, PixivRankingInfo pixivRankingInfo, PixivRankingMode rankingMode)
        {
            try
            {
                string template = BotConfig.PixivRankingConfig.Template;
                string templateMsg = rankingBusiness.getRankingMsg(pixivRankingInfo.RankingDate, rankingMode.Name, template);

                List<string> PreviewFilePaths = pixivRankingInfo.PreviewFilePaths;
                if (PreviewFilePaths is null || PreviewFilePaths.IsFilesExists() == false)
                {
                    PreviewFilePaths = createPreviewImg(pixivRankingInfo);
                    pixivRankingInfo.PreviewFilePaths = PreviewFilePaths;
                }

                List<SetuContent> setuContents = new List<SetuContent>();
                setuContents.AddRange(PreviewFilePaths.Select(o => new SetuContent(new FileInfo(o))));

                foreach (var groupId in rankingTimer.Groups)
                {
                    await Session.SendGroupMessageAsync(groupId, templateMsg);
                    await Task.Delay(1000);
                    await Session.SendGroupSetuAsync(setuContents, groupId, BotConfig.PixivRankingConfig.SendMerge);
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                string errMsg = $"sendPreviewFileAsync异常";
                LogHelper.Error(ex, errMsg);
                Reporter.SendError(ex, errMsg);
            }
        }

        private async Task sendSetuDetailAsync(PixivRankingTimer rankingTimer, PixivRankingInfo pixivRankingInfo, PixivRankingMode rankingMode)
        {
            try
            {
                if (rankingTimer.SendDetail <= 0) return;
                List<SetuContent> setuContents = new List<SetuContent>();
                for (int i = 0; i < pixivRankingInfo.RankingDetails.Count && i < rankingTimer.SendDetail; i++)
                {
                    PixivRankingDetail detail = pixivRankingInfo.RankingDetails[i];
                    bool isR18Img = detail.WorkInfo.IsR18;
                    bool isDownImg = rankingTimer.Groups.IsDownSetuImg(isR18Img);
                    List<FileInfo> setuFiles = isDownImg ? await downPixivImgsAsync(detail.WorkInfo) : new();
                    string workMsg = pixivBusiness.getWorkInfo(detail.WorkInfo, BotConfig.PixivConfig.Template);
                    setuContents.Add(new SetuContent(workMsg, setuFiles));
                }
                List<long> groupIds = rankingTimer.Groups;
                foreach (var groupId in groupIds)
                {
                    bool isShowImg = groupId.IsShowSetuImg(false);
                    var sendContents = setuContents.Select(o => isShowImg ? o with { } : o with { SetuImages = new() }).ToList();
                    await Session.SendGroupSetuAsync(sendContents, groupId, BotConfig.PixivRankingConfig.SendMerge);
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                string errMsg = $"sendSetuDetailAsync异常";
                LogHelper.Error(ex, errMsg);
                Reporter.SendError(ex, errMsg);
            }
        }

        private async Task replyRankingInfo(GroupCommand command, PixivRankingMode rankingMode, PixivRankingItem rankingItem)
        {
            try
            {
                string search_date = command.KeyWord;
                CoolingCache.SetPixivRankingHanding();
                CoolingCache.SetHanding(command.GroupId, command.MemberId);
                if (string.IsNullOrWhiteSpace(BotConfig.PixivRankingConfig.ProcessingMsg) == false)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.PixivRankingConfig.ProcessingMsg);
                }

                PixivRankingInfo pixivRankingInfo = PixivRankingCache.GetCache(rankingMode, search_date);
                if (pixivRankingInfo == null)
                {
                    pixivRankingInfo = await rankingBusiness.getRankingInfo(rankingItem, rankingMode, search_date);
                    PixivRankingCache.AddCache(rankingMode, pixivRankingInfo, search_date);
                }

                string template = BotConfig.PixivRankingConfig.Template;
                string templateMsg = rankingBusiness.getRankingMsg(pixivRankingInfo.RankingDate, rankingMode.Name, template);

                List<string> PreviewFilePaths = pixivRankingInfo.PreviewFilePaths;
                if (PreviewFilePaths is null || PreviewFilePaths.IsFilesExists() == false)
                {
                    PreviewFilePaths = createPreviewImg(pixivRankingInfo);
                    pixivRankingInfo.PreviewFilePaths = PreviewFilePaths;
                }

                List<SetuContent> setuContents = new List<SetuContent>();
                setuContents.AddRange(PreviewFilePaths.Select(o => new SetuContent(new FileInfo(o))));

                await command.ReplyGroupMessageWithAtAsync(templateMsg);
                await Task.Delay(1000);
                await Session.SendGroupSetuAsync(setuContents, command.GroupId, BotConfig.PixivRankingConfig.SendMerge);
                await Task.Delay(1000);

                CoolingCache.SetGroupPixivRankingCooling(rankingMode.Type, command.GroupId);
            }
            catch (Exception ex)
            {
                string errMsg = $"replyRankingInfo异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetPixivRankingHandFinish();
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);
            }
        }

        private List<string> createPreviewImg(PixivRankingInfo pixivRankingInfo)
        {
            int startIndex = 0;
            int maxInPage = BotConfig.PixivRankingConfig.MaxInPage;
            if (maxInPage <= 0) maxInPage = 30;
            List<string> fileInfos = new List<string>();
            List<PixivRankingDetail> details = pixivRankingInfo.RankingDetails;
            if (details.Count == 0) return fileInfos;
            PixivRankingMode rankingMode = pixivRankingInfo.RankingMode;
            while (startIndex < details.Count)
            {
                string fileName = $"{rankingMode.Code}_preview_{pixivRankingInfo.RankingDate}_{startIndex}_{startIndex + maxInPage}.jpg";
                string savePath = Path.Combine(FilePath.GetDownFileSavePath(), fileName);
                var partList = details.Skip(startIndex).Take(maxInPage).ToList();
                var previewFile = createPreviewImg(partList, savePath);
                if (previewFile is not null) fileInfos.Add(previewFile.FullName);
                startIndex += maxInPage;
            }
            return fileInfos;
        }

        private FileInfo createPreviewImg(List<PixivRankingDetail> datas, string savePath)
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
