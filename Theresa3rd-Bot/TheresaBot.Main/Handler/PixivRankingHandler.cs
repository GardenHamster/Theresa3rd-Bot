using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Drawer;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Mode;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
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
            await replyRanking(command, rankingMode, rankingItem);
        }

        public async Task sendDailyAIRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.DailyAI;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.DailyAI;
            await replyRanking(command, rankingMode, rankingItem);
        }

        public async Task sendMaleRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Male;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Male;
            await replyRanking(command, rankingMode, rankingItem);
        }

        public async Task sendWeeklyRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Weekly;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Weekly;
            await replyRanking(command, rankingMode, rankingItem);
        }

        public async Task sendMonthlyRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Monthly;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Monthly;
            await replyRanking(command, rankingMode, rankingItem);
        }

        public async Task sendDailyR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Daily_R18;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Daily;
            await replyRanking(command, rankingMode, rankingItem);
        }

        public async Task sendDailyAIR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.DailyAI_R18;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.DailyAI;
            await replyRanking(command, rankingMode, rankingItem);
        }

        public async Task sendMaleR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Male_R18;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Male;
            await replyRanking(command, rankingMode, rankingItem);
        }

        public async Task sendWeeklyR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Weekly_R18;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Weekly;
            await replyRanking(command, rankingMode, rankingItem);
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
                await Reporter.SendError(ex, errMsg);
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
                    PreviewFilePaths = await createPreviewImgAsync(pixivRankingInfo);
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
                }

                foreach (var groupId in rankingTimer.Groups)
                {
                    if (rankingMode.IsR18 && groupId.IsShowR18SetuImg() == false) continue;
                    await SendGroupSetuAsync(setuContents, groupId, true);
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                string errMsg = $"sendPreviewFileAsync异常";
                LogHelper.Error(ex, errMsg);
                await Reporter.SendError(ex, errMsg);
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
                    msgContent.Add(new PlainContent($"#{rc.rank} {workInfo.likeRate.toPercent()}/{workInfo.bookmarkRate.toPercent()}"));
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
                await Reporter.SendError(ex, errMsg);
            }
        }

        private async Task replyRanking(GroupCommand command, PixivRankingMode rankingMode, PixivRankingItem rankingItem)
        {
            var dateStr = string.Empty;
            var idStr = string.Empty;

            var paramArr = command.Params;
            if (paramArr.Length > 1)
            {
                dateStr = paramArr[0];
                idStr = paramArr[1];
            }
            else if (paramArr.Length > 0)
            {
                idStr = paramArr[0];
            }

            if (string.IsNullOrEmpty(idStr))
            {
                await replyRankingPreview(command, rankingMode, rankingItem, dateStr);
            }
            else
            {
                await replyRankingDetail(command, rankingMode, rankingItem, dateStr, idStr);
            }
        }

        private async Task replyRankingPreview(GroupCommand command, PixivRankingMode rankingMode, PixivRankingItem rankingItem, string search_date)
        {
            try
            {
                CoolingCache.SetPixivRankingHanding();
                CoolingCache.SetHanding(command.GroupId, command.MemberId);

                PixivRankingInfo pixivRankingInfo = PixivRankingCache.GetCache(rankingMode, search_date);
                if (pixivRankingInfo == null)
                {
                    await command.ReplyProcessingMessageAsync(BotConfig.PixivRankingConfig.ProcessingMsg);
                    pixivRankingInfo = await rankingBusiness.getRankingInfo(rankingItem, rankingMode, search_date);
                    PixivRankingCache.AddCache(rankingMode, pixivRankingInfo, search_date);
                }

                string template = BotConfig.PixivRankingConfig.Template;
                string templateMsg = rankingBusiness.getRankingMsg(pixivRankingInfo.RankingDate, rankingMode.Name, template);

                List<string> PreviewFilePaths = pixivRankingInfo.PreviewFilePaths;
                if (PreviewFilePaths is null || PreviewFilePaths.IsFilesExists() == false)
                {
                    PreviewFilePaths = await createPreviewImgAsync(pixivRankingInfo);
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
                await Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetPixivRankingHandFinish();
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);
            }
        }

        private async Task replyRankingDetail(GroupCommand command, PixivRankingMode rankingMode, PixivRankingItem rankingItem, string search_date, string idStr)
        {
            var idList = idStr.SplitToIdList();
            if (idList.Count == 0)
            {
                await command.ReplyGroupMessageWithAtAsync($"没有检测到序号，你可以使用【#日榜 20230601 11,45,14】的格式获取指定序号的作品详情");
                return;
            }

            PixivRankingInfo pixivRankingInfo = PixivRankingCache.GetCache(rankingMode, search_date);
            if (pixivRankingInfo == null)
            {
                await command.ReplyProcessingMessageAsync(BotConfig.PixivRankingConfig.ProcessingMsg);
                pixivRankingInfo = await rankingBusiness.getRankingInfo(rankingItem, rankingMode, search_date);
                PixivRankingCache.AddCache(rankingMode, pixivRankingInfo, search_date);
            }

            var rankingDetails = pixivRankingInfo.RankingDetails.Where(o => idList.Contains(o.RankingContent.rank)).ToList();
            if (rankingDetails.Count == 0)
            {
                await command.ReplyGroupMessageWithAtAsync($"当前榜单中没有不存在指定序号的作品");
                return;
            }

            string template = BotConfig.PixivConfig.Template;
            var setuContents = new List<SetuContent>();
            foreach (var rankingDetail in rankingDetails)
            {
                var pixivWorkInfo = rankingDetail.WorkInfo;
                var setuInfo = new PlainContent(pixivBusiness.getWorkInfo(pixivWorkInfo, template));
                var setuFiles = await GetSetuFilesAsync(pixivWorkInfo, command.GroupId);
                SetuContent setuContent = new PixivSetuContent(setuInfo, setuFiles, pixivWorkInfo);
                setuContents.Add(setuContent);
            }

            long msgId = await Session.SendGroupMergeAsync(command.GroupId, setuContents);
            Task recordTask = recordBusiness.AddPixivRecord(setuContents, msgId, command.GroupId);
        }

        private async Task<List<string>> createPreviewImgAsync(PixivRankingInfo rankingInfo)
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
                var previewFile = await createPreviewImgAsync(rankingInfo, partList, fullSavePath);
                if (previewFile is not null) fileInfos.Add(previewFile.FullName);
                startIndex += previewInPage;
            }
            return fileInfos;
        }

        private async Task<FileInfo> createPreviewImgAsync(PixivRankingInfo rankingInfo, List<PixivRankingDetail> details, string fullSavePath)
        {
            try
            {
                return await new PixivRankingDrawer().DrawPreview(rankingInfo, details, fullSavePath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "日榜一览图合成失败");
                await Reporter.SendError(ex, "日榜一览图合成失败");
                return null;
            }
        }


    }
}
