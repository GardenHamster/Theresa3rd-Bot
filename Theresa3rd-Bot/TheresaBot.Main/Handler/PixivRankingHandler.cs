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
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    internal class PixivRankingHandler : SetuHandler
    {
        private const int DetailEachPage = 10;
        private PixivService pixivService;
        private PixivRankingService rankingService;

        public PixivRankingHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivService = new PixivService();
            rankingService = new PixivRankingService();
        }
        public async Task SendDailyRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Daily;
            PixivRankingSafeItem rankingItem = BotConfig.PixivRankingConfig.Daily;
            await ReplyRanking(command, rankingMode, rankingItem);
        }

        public async Task SendDailyAIRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.DailyAI;
            PixivRankingSafeItem rankingItem = BotConfig.PixivRankingConfig.DailyAI;
            await ReplyRanking(command, rankingMode, rankingItem);
        }

        public async Task SendMaleRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Male;
            PixivRankingSafeItem rankingItem = BotConfig.PixivRankingConfig.Male;
            await ReplyRanking(command, rankingMode, rankingItem);
        }

        public async Task SendWeeklyRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Weekly;
            PixivRankingSafeItem rankingItem = BotConfig.PixivRankingConfig.Weekly;
            await ReplyRanking(command, rankingMode, rankingItem);
        }

        public async Task SendMonthlyRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Monthly;
            PixivRankingSafeItem rankingItem = BotConfig.PixivRankingConfig.Monthly;
            await ReplyRanking(command, rankingMode, rankingItem);
        }

        public async Task SendDailyR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Daily_R18;
            PixivRankingSafeItem rankingItem = BotConfig.PixivRankingConfig.Daily;
            await ReplyRanking(command, rankingMode, rankingItem);
        }

        public async Task SendDailyAIR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.DailyAI_R18;
            PixivRankingSafeItem rankingItem = BotConfig.PixivRankingConfig.DailyAI;
            await ReplyRanking(command, rankingMode, rankingItem);
        }

        public async Task SendMaleR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Male_R18;
            PixivRankingSafeItem rankingItem = BotConfig.PixivRankingConfig.Male;
            await ReplyRanking(command, rankingMode, rankingItem);
        }

        public async Task SendWeeklyR18Ranking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Weekly_R18;
            PixivRankingSafeItem rankingItem = BotConfig.PixivRankingConfig.Weekly;
            await ReplyRanking(command, rankingMode, rankingItem);
        }

        public async Task HandleRankingSubscribeAsync(PixivRankingTimer rankingTimer, PixivRankingSafeItem rankingItem, PixivRankingMode rankingMode)
        {
            try
            {
                CoolingCache.SetPixivRankingHanding();
                if (rankingMode.IsR18 && rankingTimer.Groups.IsShowR18SetuImg() == false) return;
                PixivRankingInfo rankingInfo = await rankingService.getRankingInfo(rankingItem, rankingMode, String.Empty);
                PixivRankingCache.AddCache(rankingMode, rankingInfo, String.Empty);
                if (rankingInfo.RankingDetails.Count == 0) return;
                await SendPreviewFileAsync(rankingTimer, rankingInfo, rankingMode);
                await Task.Delay(2000);
                await SendSetuDetailAsync(rankingInfo, rankingMode, rankingTimer.Groups, rankingTimer.SendDetail);
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

        private async Task SendPreviewFileAsync(PixivRankingTimer rankingTimer, PixivRankingInfo pixivRankingInfo, PixivRankingMode rankingMode)
        {
            try
            {
                string template = BotConfig.PixivRankingConfig.Template;
                string templateMsg = rankingService.getRankingMsg(pixivRankingInfo.RankingDate, rankingMode.Name, template);

                List<string> PreviewFilePaths = pixivRankingInfo.PreviewFilePaths;
                if (PreviewFilePaths is null || PreviewFilePaths.IsFilesExists() == false)
                {
                    PreviewFilePaths = await CreatePreviewImgAsync(pixivRankingInfo);
                    pixivRankingInfo.PreviewFilePaths = PreviewFilePaths;
                }

                BaseContent[] titleContents = new BaseContent[]
                {
                    new PlainContent(templateMsg)
                };
                BaseContent[] tipContents = new BaseContent[]
                {
                    new PlainContent($"使用 【#日榜 1,2,3...】或者【#日榜 {pixivRankingInfo.RankingDate} 1,2,3...】格式可以获取指定排名的作品详情")
                };

                List<SetuContent> setuContents = new List<SetuContent>();
                setuContents.AddRange(PreviewFilePaths.Select(o => new SetuContent(new FileInfo(o))));
                setuContents.AddRange(rankingService.getRankAndPids(pixivRankingInfo, 10));

                foreach (var groupId in rankingTimer.Groups.ToSendableGroups())
                {
                    if (rankingMode.IsR18 && groupId.IsShowR18SetuImg() == false) continue;
                    await Session.SendGroupMessageAsync(groupId, templateMsg);
                    await Task.Delay(1000);
                }

                foreach (var groupId in rankingTimer.Groups.ToSendableGroups())
                {
                    if (rankingMode.IsR18 && groupId.IsShowR18SetuImg() == false) continue;
                    await SendGroupMergeSetuAsync(setuContents, new() { titleContents, tipContents }, groupId);
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

        private async Task SendSetuDetailAsync(PixivRankingInfo pixivRankingInfo, PixivRankingMode rankingMode, List<long> groupIds, int quantity)
        {
            try
            {
                if (quantity <= 0) return;
                if (groupIds is null || groupIds.Count == 0) return;

                BaseContent[] titleContents = new BaseContent[]
                {
                    new PlainContent($"{pixivRankingInfo.RankingDate} {pixivRankingInfo.RankingMode.Name}详情")
                };

                List<SetuContent> setuContents = new List<SetuContent>();
                for (int i = 0; i < pixivRankingInfo.RankingDetails.Count && i < quantity; i++)
                {
                    PixivRankingDetail detail = pixivRankingInfo.RankingDetails[i];
                    PixivRankingContent rc = detail.RankingContent;
                    PixivWorkInfo workInfo = detail.WorkInfo;
                    List<FileInfo> setuFiles = await GetSetuFilesAsync(workInfo, groupIds);
                    string workMsg = pixivService.getWorkInfo(detail.WorkInfo);
                    List<BaseContent> msgContent = new List<BaseContent>();
                    msgContent.Add(new PlainContent($"#{rc.rank} {workInfo.likeRate.ToPercent()}/{workInfo.bookmarkRate.ToPercent()}"));
                    msgContent.Add(new PlainContent(workMsg));
                    setuContents.Add(new SetuContent(msgContent, setuFiles));
                }
                foreach (var groupId in groupIds.ToSendableGroups())
                {
                    if (rankingMode.IsR18 && groupId.IsShowR18SetuImg() == false) continue;
                    bool isShowImg = groupId.IsShowSetuImg(false);
                    var sendContents = setuContents.Select(o => isShowImg ? o with { } : o with { SetuImages = new() }).ToList();
                    await SendGroupMergeSetuAsync(sendContents, new() { titleContents }, groupId, DetailEachPage);
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

        private async Task ReplyRanking(GroupCommand command, PixivRankingMode rankingMode, PixivRankingSafeItem rankingItem)
        {
            var dateStr = string.Empty;
            var idStr = string.Empty;

            var paramArr = command.Params;
            if (paramArr.Length > 1)
            {
                dateStr = paramArr[0];
                idStr = paramArr[1];
            }
            else if (paramArr.Length > 0 && paramArr[0].IsShortDateStr())
            {
                dateStr = paramArr[0];
            }
            else if (paramArr.Length > 0)
            {
                idStr = paramArr[0];
            }

            if (string.IsNullOrEmpty(idStr))
            {
                await ReplyRankingPreview(command, rankingMode, rankingItem, dateStr);
            }
            else
            {
                await ReplyRankingDetail(command, rankingMode, rankingItem, dateStr, idStr);
            }
        }

        private async Task ReplyRankingPreview(GroupCommand command, PixivRankingMode rankingMode, PixivRankingSafeItem rankingItem, string search_date)
        {
            try
            {
                CoolingCache.SetPixivRankingHanding();
                CoolingCache.SetHanding(command.GroupId, command.MemberId);

                PixivRankingInfo pixivRankingInfo = PixivRankingCache.GetCache(rankingMode, search_date);
                if (pixivRankingInfo == null)
                {
                    await command.ReplyProcessingMessageAsync(BotConfig.PixivRankingConfig.ProcessingMsg);
                    pixivRankingInfo = await rankingService.getRankingInfo(rankingItem, rankingMode, search_date);
                    PixivRankingCache.AddCache(rankingMode, pixivRankingInfo, search_date);
                }

                string template = BotConfig.PixivRankingConfig.Template;
                string templateMsg = rankingService.getRankingMsg(pixivRankingInfo.RankingDate, rankingMode.Name, template);

                List<string> PreviewFilePaths = pixivRankingInfo.PreviewFilePaths;
                if (PreviewFilePaths is null || PreviewFilePaths.IsFilesExists() == false)
                {
                    PreviewFilePaths = await CreatePreviewImgAsync(pixivRankingInfo);
                    pixivRankingInfo.PreviewFilePaths = PreviewFilePaths;
                }

                BaseContent[] titleContents = new BaseContent[]
                {
                    new PlainContent(templateMsg)
                };
                BaseContent[] tipContents = new BaseContent[]
                {
                    new PlainContent($"使用 【#日榜 1,2,3...】或者【#日榜 {pixivRankingInfo.RankingDate} 1,2,3...】格式可以获取指定排名的作品详情")
                };

                List<SetuContent> setuContents = new List<SetuContent>();
                setuContents.AddRange(PreviewFilePaths.Select(o => new SetuContent(new FileInfo(o))));
                setuContents.AddRange(rankingService.getRankAndPids(pixivRankingInfo, 10));

                await command.ReplyGroupMessageWithQuoteAsync(templateMsg);
                await Task.Delay(1000);
                await SendGroupMergeSetuAsync(setuContents, new() { titleContents, tipContents }, command.GroupId);
                await Task.Delay(1000);
                await SendSetuDetailAsync(pixivRankingInfo, rankingMode, new List<long>() { command.GroupId }, BotConfig.PixivRankingConfig.SendDetail);
                CoolingCache.SetGroupPixivRankingCooling(rankingMode.Type, command.GroupId);
            }
            catch (NoRankingException ex)
            {
                await command.ReplyGroupMessageWithQuoteAsync($"获取失败，api接口返回：{ex.Message}");
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, $"replyRankingInfo异常");
            }
            finally
            {
                CoolingCache.SetPixivRankingHandFinish();
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);
            }
        }

        private async Task ReplyRankingDetail(GroupCommand command, PixivRankingMode rankingMode, PixivRankingSafeItem rankingItem, string search_date, string idStr)
        {
            var idList = idStr.SplitToIdList();
            if (idList.Count == 0)
            {
                await command.ReplyGroupMessageWithQuoteAsync($"没有检测到序号，你可以使用【#日榜 20230601 11,45,14】的格式获取指定排名的作品详情");
                return;
            }

            if (idList.Any(o => o < 1 || o > 500))
            {
                await command.ReplyGroupMessageWithQuoteAsync($"检测到错误的序号，序号只能在1~500之间");
                return;
            }

            PixivRankingInfo pixivRankingInfo = PixivRankingCache.GetCache(rankingMode, search_date);
            if (pixivRankingInfo == null)
            {
                await command.ReplyProcessingMessageAsync(BotConfig.PixivRankingConfig.ProcessingMsg);
                pixivRankingInfo = await rankingService.getRankingInfo(rankingItem, rankingMode, search_date);
                PixivRankingCache.AddCache(rankingMode, pixivRankingInfo, search_date);
            }

            var rankingDetails = pixivRankingInfo.RankingDetails.Where(o => idList.Contains(o.RankingContent.rank)).ToList();
            if (rankingDetails.Count == 0)
            {
                await command.ReplyGroupMessageWithQuoteAsync($"当前榜单中不存在指定排名的作品");
                return;
            }

            var setuContents = new List<SetuContent>();
            foreach (var rankingDetail in rankingDetails)
            {
                var pixivWorkInfo = rankingDetail.WorkInfo;
                var setuInfo = new PlainContent(pixivService.getWorkInfo(pixivWorkInfo));
                var setuFiles = await GetSetuFilesAsync(pixivWorkInfo, command.GroupId);
                SetuContent setuContent = new PixivSetuContent(setuInfo, setuFiles, pixivWorkInfo);
                setuContents.Add(setuContent);
            }

            await SendGroupMergeSetuAsync(setuContents, new(), command.GroupId);
        }

        private async Task<List<string>> CreatePreviewImgAsync(PixivRankingInfo rankingInfo)
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
                string fullSavePath = Path.Combine(FilePath.GetPixivPreviewDirectory(), fileName);
                var partList = details.Skip(startIndex).Take(previewInPage).ToList();
                var previewFile = await CreatePreviewImgAsync(rankingInfo, partList, fullSavePath);
                if (previewFile is not null) fileInfos.Add(previewFile.FullName);
                startIndex += previewInPage;
            }
            return fileInfos;
        }

        private async Task<FileInfo> CreatePreviewImgAsync(PixivRankingInfo rankingInfo, List<PixivRankingDetail> details, string fullSavePath)
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
