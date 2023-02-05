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

            (List<PixivRankingContent> rankingContents, string date) = await rankingBusiness.getRankingData(rankingItem, mode);

            if (BotConfig.PixivRankingConfig.SendPreview && r18 == false)
            {
                string previewInfo = $"{date}日{rankingName}一览图";
                FileInfo previewFile = await createPreviewImg(rankingContents);
                previewContent = new SetuContent(previewInfo, previewFile);
            }
            if (BotConfig.PixivRankingConfig.SendDetail)
            {
                setuContents = await getSetuContent(rankingContents, rankingItem, command.GroupId, r18);
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

        private async Task<List<SetuContent>> getSetuContent(List<PixivRankingContent> datas, PixivRankingItem rankingItem, long groupId, bool r18Content)
        {
            List<SetuContent> setuContents = new List<SetuContent>();
            foreach (var data in datas)
            {
                SetuContent setuContent = await getSetuContent(data, rankingItem, groupId, r18Content);
                if (setuContent is not null) setuContents.Add(setuContent);
            }
            return setuContents;
        }

        private async Task<SetuContent> getSetuContent(PixivRankingContent data, PixivRankingItem rankingItem, long groupId, bool r18Content)
        {
            try
            {
                bool isR18Img = r18Content || data.isR18();
                bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                PixivWorkInfo workInfo = await pixivBusiness.getPixivWorkInfoAsync(data.illust_id.ToString());
                if (rankingBusiness.checkRankingWorkIsOk(rankingItem, workInfo) == false) return null;
                string setuInfo = pixivBusiness.getWorkInfo(workInfo, DateTime.Now, BotConfig.PixivConfig.Template);
                List<FileInfo> setuFiles = isShowImg ? await downPixivImgsAsync(workInfo) : new();
                return new SetuContent(setuInfo, setuFiles);
            }
            catch (ApiException ex)
            {
                return new SetuContent(ex.Message, new List<FileInfo>());
            }
        }

        private Task<FileInfo> createPreviewImg(List<PixivRankingContent> rankingContents)
        {
            try
            {
                return null;
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
