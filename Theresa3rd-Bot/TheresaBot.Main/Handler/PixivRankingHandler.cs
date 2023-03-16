using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
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
    internal class PixivRankingHandler : SetuHandler
    {
        private const int DetailEachPage = 10;
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

        public async Task sendDailyR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Daily_R18;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Daily;
            await replyRankingInfo(command, rankingMode, rankingItem);
        }

        public async Task sendDailyAIR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.DailyAI_R18;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.DailyAI;
            await replyRankingInfo(command, rankingMode, rankingItem);
        }

        public async Task sendMaleR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Male_R18;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Male;
            await replyRankingInfo(command, rankingMode, rankingItem);
        }

        public async Task sendWeeklyR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Weekly_R18;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Weekly;
            await replyRankingInfo(command, rankingMode, rankingItem);
        }

        public async Task handleRankingSubscribeAsync(PixivRankingTimer rankingTimer, PixivRankingItem rankingItem, PixivRankingMode rankingMode)
        {
            try
            {
                CoolingCache.SetPixivRankingHanding();
                if (rankingMode.IsR18 && rankingTimer.Groups.IsShowR18SetuImg() == false) return;
                PixivRankingInfo rankingInfo = await rankingBusiness.getRankingInfo(rankingItem, rankingMode, String.Empty);
                PixivRankingCache.AddCache(rankingMode, rankingInfo, String.Empty);
                if (rankingInfo.RankingDetails.Count == 0) return;
                await sendPreviewFileAsync(rankingTimer, rankingInfo, rankingMode);
                await Task.Delay(2000);
                await sendSetuDetailAsync(rankingInfo, rankingMode, rankingTimer.Groups, rankingTimer.SendDetail);
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
                setuContents.Add(new SetuContent(templateMsg));
                setuContents.AddRange(PreviewFilePaths.Select(o => new SetuContent(new FileInfo(o))));
                setuContents.AddRange(rankingBusiness.getRankAndPids(pixivRankingInfo, 10));
                foreach (var groupId in rankingTimer.Groups)
                {
                    if (rankingMode.IsR18 && groupId.IsShowR18SetuImg() == false) continue;
                    await Session.SendGroupMessageAsync(groupId, templateMsg);
                    await Task.Delay(1000);
                    await SendGroupSetuAsync(setuContents, groupId, true);
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

        private async Task sendSetuDetailAsync(PixivRankingInfo pixivRankingInfo, PixivRankingMode rankingMode, List<long> groupIds, int quantity)
        {
            try
            {
                if (quantity <= 0) return;
                if (groupIds is null || groupIds.Count == 0) return;
                List<SetuContent> setuContents = new List<SetuContent>();
                SetuContent headerContent = new($"{pixivRankingInfo.RankingDate} {pixivRankingInfo.RankingMode.Name}详情");
                for (int i = 0; i < pixivRankingInfo.RankingDetails.Count && i < quantity; i++)
                {
                    PixivRankingDetail detail = pixivRankingInfo.RankingDetails[i];
                    PixivRankingContent rc = detail.RankingContent;
                    PixivWorkInfo workInfo = detail.WorkInfo;
                    List<FileInfo> setuFiles = await GetSetuFilesAsync(workInfo, groupIds);
                    string workMsg = pixivBusiness.getWorkInfo(detail.WorkInfo, BotConfig.PixivConfig.Template);
                    List<BaseContent> msgContent = new List<BaseContent>();
                    msgContent.Add(new PlainContent($"#{rc.rank} {workInfo.likeRate.toPercent()}/{workInfo.bookmarkRate.toPercent()}\r\n"));
                    msgContent.Add(new PlainContent(workMsg));
                    setuContents.Add(new SetuContent(msgContent, setuFiles));
                }
                foreach (var groupId in groupIds)
                {
                    if (rankingMode.IsR18 && groupId.IsShowR18SetuImg() == false) continue;
                    bool isShowImg = groupId.IsShowSetuImg(false);
                    var sendContents = setuContents.Select(o => isShowImg ? o with { } : o with { SetuImages = new() }).ToList();
                    await SendGroupSetuAsync(sendContents, new() { headerContent }, groupId, DetailEachPage);
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
                setuContents.Add(new SetuContent(templateMsg));
                setuContents.AddRange(PreviewFilePaths.Select(o => new SetuContent(new FileInfo(o))));
                setuContents.AddRange(rankingBusiness.getRankAndPids(pixivRankingInfo, 10));

                await command.ReplyGroupMessageWithAtAsync(templateMsg);
                await Task.Delay(1000);
                await SendGroupSetuAsync(setuContents, command.GroupId, true);
                await Task.Delay(1000);
                await sendSetuDetailAsync(pixivRankingInfo, rankingMode, new List<long>() { command.GroupId }, BotConfig.PixivRankingConfig.SendDetail);
                CoolingCache.SetGroupPixivRankingCooling(rankingMode.Type, command.GroupId);
            }
            catch (NoRankingException ex)
            {
                await command.ReplyGroupMessageWithAtAsync($"获取失败，api接口返回：{ex.Message}");
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

        private List<string> createPreviewImg(PixivRankingInfo rankingInfo)
        {
            int startIndex = 0;
            int previewInPage = BotConfig.PixivRankingConfig.PreviewInPage;
            if (previewInPage <= 0) previewInPage = 30;
            List<string> fileInfos = new List<string>();
            List<PixivRankingDetail> details = rankingInfo.RankingDetails;
            if (details.Count == 0) return fileInfos;
            PixivRankingMode rankingMode = rankingInfo.RankingMode;
            while (startIndex < details.Count)
            {
                string fileName = $"{rankingMode.Code}_preview_{rankingInfo.RankingDate}_{startIndex}_{startIndex + previewInPage}.jpg";
                string fullSavePath = Path.Combine(FilePath.GetPixivPreviewSavePath(), fileName);
                var partList = details.Skip(startIndex).Take(previewInPage).ToList();
                var previewFile = createPreviewImg(rankingInfo, partList, fullSavePath);
                if (previewFile is not null) fileInfos.Add(previewFile.FullName);
                startIndex += previewInPage;
            }
            return fileInfos;
        }

        private FileInfo createPreviewImg(PixivRankingInfo rankingInfo, List<PixivRankingDetail> datas, string savePath)
        {
            try
            {
                return PixivRankingDrawHelper.DrawPreview(rankingInfo, datas, savePath);
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
