using TheresaBot.Core.Cache;
using TheresaBot.Core.Command;
using TheresaBot.Core.Common;
using TheresaBot.Core.Datas;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Config;
using TheresaBot.Core.Model.Content;
using TheresaBot.Core.Model.Lolicon;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Services;
using TheresaBot.Core.Session;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Handler
{
    internal class LoliconHandler : SetuHandler
    {
        private LoliconService loliconService;

        public LoliconHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            loliconService = new LoliconService();
        }

        public async Task LoliconSearchAsync(GroupCommand command)
        {
            try
            {
                List<LoliconDataV2> dataList;
                var tagStr = command.KeyWord;
                var isShowAI = command.GroupId.IsShowAISetu();
                var isShowR18 = command.GroupId.IsShowR18();
                var r18Mode = isShowR18 ? 2 : 0;
                var excludeAI = isShowAI == false;
                var setuConfig = BotConfig.SetuConfig;
                var pixivConfig = BotConfig.PixivConfig;

                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中
                if (await CheckSetuTagEnableAsync(command, tagStr) == false) return;
                await command.ReplyProcessingMessageAsync(setuConfig.ProcessingMsg);

                if (string.IsNullOrEmpty(tagStr))
                {
                    dataList = await loliconService.FetchDatasAsync(r18Mode, excludeAI, 1);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;

                    dataList = await loliconService.FetchDatasAsync(r18Mode, excludeAI, 1, ToLoliconTagArr(tagStr.ToActualPixivTags()));
                }

                if (dataList.Count == 0)
                {
                    await command.ReplyGroupTemplateWithQuoteAsync(setuConfig.NotFoundMsg, "找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LoliconDataV2 loliconData = dataList.First();
                if (await CheckSetuSendable(command, loliconData, isShowR18) == false) return;

                long todayLeftCount = GetSetuLeftToday(command.GroupId, command.MemberId);
                List<FileInfo> setuFiles = await DownSetuFilesAsync(loliconData, command.GroupId);

                string template = setuConfig.Lolicon.Template;
                List<BaseContent> workMsgs = new List<BaseContent>();
                workMsgs.Add(new PlainContent(loliconService.GetWorkInfo(loliconData, todayLeftCount, template)));

                PixivSetuContent setuContent = new PixivSetuContent(workMsgs, setuFiles, loliconData);
                var results = await command.ReplyGroupSetuAsync(setuContent, setuConfig.RevokeInterval, pixivConfig.SendImgBehind);
                var msgIds = results.Select(o => o.MessageId).ToArray();
                var recordTask = recordService.InsertPixivRecord(setuContent, Session.PlatformType, msgIds, command.GroupId);
                if (setuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = command.SendPrivateSetuAsync(setuContent, pixivConfig.SendImgBehind);
                }

                if (setuConfig.SendPrivate && setuConfig.SendPrivateOrigin && pixivConfig.ImgSize != PixivImageSize.Original)
                {
                    await Task.Delay(1000);
                    Task task = SendPrivateOriginSetuAsync(loliconData, command.GroupId, command.MemberId);
                }

                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "Lolicon涩图功能异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        public async Task SendTimingSetuAsync(TimingSetuTimer timingSetuTimer, long groupId)
        {
            try
            {
                int margeEachPage = 5;
                bool sendMerge = timingSetuTimer.SendMerge;
                int r18Mode = groupId.IsShowR18() ? 2 : 0;
                bool excludeAI = groupId.IsShowAISetu() == false;
                string tagStr = RandomHelper.RandomItem(timingSetuTimer.Tags);
                string[] tagArr = string.IsNullOrWhiteSpace(tagStr) ? new string[0] : ToLoliconTagArr(tagStr);
                int quantity = timingSetuTimer.Quantity > 20 ? 20 : timingSetuTimer.Quantity;
                List<LoliconDataV2> dataList = await loliconService.FetchDatasAsync(r18Mode, excludeAI, quantity, tagArr);
                List<SetuContent> setuContents = await GetSetuContent(dataList, groupId);
                await sendTimingSetuMessageAsync(timingSetuTimer, tagStr, groupId);
                await Task.Delay(2000);
                await SendGroupSetuAsync(setuContents, groupId, sendMerge, margeEachPage);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "定时涩图异常");
                await Reporter.SendError(ex, "定时涩图异常");
            }
        }

        private async Task<List<SetuContent>> GetSetuContent(List<LoliconDataV2> datas, long groupId)
        {
            List<SetuContent> setuContents = new List<SetuContent>();
            foreach (var data in datas) setuContents.Add(await GetSetuContent(data, groupId));
            return setuContents;
        }

        private async Task<SetuContent> GetSetuContent(LoliconDataV2 data, long groupId)
        {
            string setuInfo = loliconService.GetDefaultWorkInfo(data);
            List<FileInfo> setuFiles = await DownSetuFilesAsync(data, groupId);
            return new SetuContent(setuInfo, setuFiles);
        }

    }
}
