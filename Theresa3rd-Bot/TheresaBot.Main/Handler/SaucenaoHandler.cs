using System.Text;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Ascii2d;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Saucenao;
using TheresaBot.Main.Model.Step;
using TheresaBot.Main.Relay;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class SaucenaoHandler : SetuHandler
    {
        private PixivBusiness pixivBusiness;
        private SaucenaoBusiness saucenaoBusiness;
        private Ascii2dBusiness ascii2dBusiness;

        public SaucenaoHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivBusiness = new PixivBusiness();
            saucenaoBusiness = new SaucenaoBusiness();
            ascii2dBusiness = new Ascii2dBusiness();
        }

        public async Task searchResult(GroupCommand command)
        {
            try
            {
                long revokeMsgId = command.MsgId;
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中
                List<string> imgList = command.GetImageUrls();

                if (imgList is null || imgList.Count == 0)
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(command);
                    if (stepInfo is null) return;
                    StepDetail imgStep = new StepDetail(60, "请在60秒内发送要查找的图片", CheckImageSourceAsync);
                    stepInfo.AddStep(imgStep);
                    if (await stepInfo.HandleStep() == false) return;
                    imgList = imgStep.Relay.GetImageUrls();
                    revokeMsgId = imgStep.Relay.MsgId;
                }

                if (imgList is null || imgList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync($"没有接收到图片，请重新发送指令开始操作");
                    return;
                }

                await HandleSearch(command, imgList, revokeMsgId);
                CoolingCache.SetMemberSaucenaoCooling(command.GroupId, command.MemberId);
            }
            catch (Exception ex)
            {
                string errMsg = $"searchResult异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        public async Task searchResult(GroupQuoteCommand command)
        {
            try
            {
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中
                long quoteId = command.GetQuoteMessageId();
                List<ImageRecordPO> imgRecords = quoteId > 0 ? recordBusiness.GetImageRecord(command.PlatformType, quoteId, command.GroupId) : new();
                List<string> imgList = imgRecords.Select(o => o.HttpUrl).ToList();
                if (imgList is null || imgList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync($"没能获取到引用消息中的图片，请尝试使用搜图指令搜索");
                    return;
                }

                long revokeMsgId = command.MsgId;
                await HandleSearch(command, imgList, revokeMsgId);
                CoolingCache.SetMemberSaucenaoCooling(command.GroupId, command.MemberId);
            }
            catch (Exception ex)
            {
                string errMsg = $"searchResult异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        private async Task HandleSearch(GroupCommand command, List<string> imgList, long revokeMsgId)
        {
            if (string.IsNullOrWhiteSpace(BotConfig.SaucenaoConfig.ProcessingMsg) == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.SaucenaoConfig.ProcessingMsg, null);
                await Task.Delay(1000);
            }

            if (BotConfig.SaucenaoConfig.MaxReceive > 0 && imgList.Count > BotConfig.SaucenaoConfig.MaxReceive)
            {
                imgList = imgList.Take(BotConfig.SaucenaoConfig.MaxReceive).ToList();
                await command.ReplyGroupMessageWithAtAsync($"总共接收到了{imgList.Count}张图片，只查找前{BotConfig.SaucenaoConfig.MaxReceive}张哦~");
                await Task.Delay(1000);
            }

            List<string> notFoundList = new List<string>();
            for (int i = 0; i < imgList.Count; i++)
            {
                bool isFound = await searchWithSaucenao(command, imgList[i]);
                if (isFound == false) notFoundList.Add(imgList[i]);
            }

            if (imgList.Count == notFoundList.Count && BotConfig.SaucenaoConfig.ContinueAscii2d == YNAType.No)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.SaucenaoConfig.NotFoundMsg, "找不到相似的图，换一张完整的图片试试吧~");
            }
            else if (notFoundList.Count > 0 && await CheckContinueAscii2d(command, notFoundList))
            {
                await command.ReplyGroupMessageAsync($"Saucenao共有{notFoundList.Count}张图片搜索失败，正在通过ascii2d尝试搜索剩余图片...");
                for (int i = 0; i < notFoundList.Count; i++) await searchWithAscii2d(command, notFoundList[i]);
            }
            else if (notFoundList.Count > 0)
            {
                await command.ReplyGroupMessageAsync($"Saucenao搜索完毕，共有{notFoundList.Count}张图片搜索失败");
            }

            if (BotConfig.SaucenaoConfig.RevokeSearched)
            {
                await Task.Delay(1000);
                await command.RevokeGroupMessageAsync(revokeMsgId, command.GroupId);
            }
        }


        private async Task<bool> CheckContinueAscii2d(GroupCommand command, List<string> notFoundList)
        {
            YNAType ynaType = BotConfig.SaucenaoConfig.ContinueAscii2d;
            if (ynaType == YNAType.Yes) return true;
            if (ynaType == YNAType.No) return false;
            StepInfo stepInfo = await StepCache.CreateStepAsync(command, false);
            if (stepInfo is null) return false;

            StringBuilder questionBuilder = new StringBuilder();
            questionBuilder.AppendLine($"共有{notFoundList.Count}张图片搜索失败，是否使用Ascii2d继续搜索剩余的图片？");
            questionBuilder.AppendLine($"请在30秒内发送 1：是，0：否");
            foreach (string imgUrl in notFoundList) questionBuilder.AppendLine(imgUrl);
            StepDetail askStep = new StepDetail(30, questionBuilder.ToString(), null);
            stepInfo.AddStep(askStep);
            if (await stepInfo.HandleStep() == false) return false;
            return askStep.Answer == "1";
        }

        private async Task<bool> CheckImageSourceAsync(GroupCommand command, GroupRelay relay)
        {
            List<string> imgList = relay.GetImageUrls();
            if (imgList is null || imgList.Count == 0) return await Task.FromResult(false);
            return await Task.FromResult(true);
        }

        private async Task<bool> searchWithSaucenao(GroupCommand command, string imgUrl)
        {
            try
            {
                SaucenaoResult saucenaoResult = await saucenaoBusiness.getSaucenaoResultAsync(imgUrl);
                if (saucenaoResult is null || saucenaoResult.Items.Count == 0) return false;
                List<SaucenaoItem> sortList = saucenaoBusiness.sortSaucenaoItem(saucenaoResult.Items);

                if (BotConfig.SaucenaoConfig.PullOrigin == false)
                {
                    List<BaseContent> workMsgs = new List<BaseContent>();
                    workMsgs.AddRange(getRemindMessage(saucenaoResult, command.GroupId, command.MemberId));
                    workMsgs.AddRange(getSimpleMessage(sortList.Take(BotConfig.SaucenaoConfig.SaucenaoReadCount).ToList()));
                    Task sendSimpleTask = sendAndRevoke(command, workMsgs);
                    return true;
                }

                decimal maxSimilarity = sortList.Max(o => o.Similarity);
                decimal singlePriority = BotConfig.SaucenaoConfig.SinglePriority;
                int readCount = BotConfig.SaucenaoConfig.SaucenaoReadCount;
                int maxShow = maxSimilarity >= singlePriority ? 1 : readCount;
                var saucenaoItems = await saucenaoBusiness.getBestMatchAsync(sortList, maxShow);
                if (saucenaoItems is null || saucenaoItems.Count == 0) return false;

                List<SetuContent> setuContents = new List<SetuContent>();
                setuContents.Add(new SetuContent(getRemindMessage(saucenaoResult, command.GroupId, command.MemberId)));
                for (int i = 0; i < saucenaoItems.Count; i++)
                {
                    setuContents.Add(await getSaucenaoContentAsync(command, saucenaoItems[i]));
                }

                Task sendDetailTask = sendAndRevoke(command, setuContents);
                return true;
            }
            catch (Exception ex)
            {
                string errMsg = $"searchWithSaucenao异常，url={imgUrl}";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
                return true;
            }
        }

        public List<BaseContent> getSimpleMessage(List<Ascii2dItem> ascii2dItems)
        {
            return ascii2dItems.Select(o => getSimpleMessage(o)).ToList();
        }

        public List<BaseContent> getSimpleMessage(List<SaucenaoItem> saucenaoItems)
        {
            return saucenaoItems.Select(o => getSimpleMessage(o)).ToList();
        }

        public BaseContent getSourceMessage(SaucenaoItem saucenaoItem)
        {
            return new PlainContent($"相似度：{saucenaoItem.Similarity}%，来源：{Enum.GetName(typeof(SetuSourceType), saucenaoItem.SourceType)}");
        }

        public BaseContent getSimpleMessage(SaucenaoItem saucenaoItem)
        {
            return new PlainContent($"相似度：{saucenaoItem.Similarity}%，来源:{Enum.GetName(typeof(SetuSourceType), saucenaoItem.SourceType)}，链接：{saucenaoItem.SourceUrl}");
        }

        public BaseContent getSimpleMessage(Ascii2dItem ascii2dItem)
        {
            return new PlainContent($"来源:{Enum.GetName(typeof(SetuSourceType), ascii2dItem.SourceType)}，链接：{ascii2dItem.SourceUrl}");
        }

        private async Task<SetuContent> getSaucenaoContentAsync(GroupCommand command, SaucenaoItem saucenaoItem)
        {
            decimal minSimilarity = BotConfig.SaucenaoConfig.ImagePriority;
            if (saucenaoItem.SourceType == SetuSourceType.Pixiv)
            {
                bool isShowR18 = command.GroupId.IsShowR18Saucenao();
                PixivWorkInfo pixivWorkInfo = saucenaoItem.PixivWorkInfo;
                string notSendableMsg = IsSetuSendable(command, saucenaoItem.PixivWorkInfo, isShowR18);
                if (string.IsNullOrWhiteSpace(notSendableMsg) == false)
                {
                    List<BaseContent> notSendableContent = new() { getSourceMessage(saucenaoItem), new PlainContent(notSendableMsg) };
                    return new(notSendableContent);
                }
                List<BaseContent> workMsgs = new List<BaseContent>();
                List<FileInfo> setuFiles = saucenaoItem.Similarity < minSimilarity ? new() : await GetSetuFilesAsync(pixivWorkInfo, command.GroupId);
                workMsgs.Add(getSourceMessage(saucenaoItem));
                workMsgs.AddRange(getPixivMessageAsync(saucenaoItem));
                return new PixivSetuContent(workMsgs, setuFiles, pixivWorkInfo);
            }
            else
            {
                List<BaseContent> workMsgs = new List<BaseContent>();
                workMsgs.Add(getSimpleMessage(saucenaoItem));
                return new(workMsgs);
            }
        }

        public List<BaseContent> getPixivMessageAsync(SaucenaoItem saucenaoItem)
        {
            List<BaseContent> msgList = new List<BaseContent>();
            PixivWorkInfo pixivWorkInfo = saucenaoItem.PixivWorkInfo;
            msgList.Add(new PlainContent(pixivBusiness.getWorkInfo(pixivWorkInfo, BotConfig.PixivConfig.Template)));
            return msgList;
        }

        public List<BaseContent> getRemindMessage(SaucenaoResult saucenaoResult, long groupId, long memberId)
        {
            List<BaseContent> msgList = new List<BaseContent>();
            string remindTemplate = BotConfig.SaucenaoConfig.Template;
            long todayLeft = GetSaucenaoLeftToday(groupId, memberId);
            if (string.IsNullOrWhiteSpace(remindTemplate))
            {
                msgList.Add(new PlainContent(saucenaoBusiness.getDefaultRemindMessage(saucenaoResult, todayLeft)));
            }
            else
            {
                msgList.Add(new PlainContent(saucenaoBusiness.getSaucenaoRemindMessage(saucenaoResult, remindTemplate, todayLeft)));
            }
            return msgList;
        }

        private async Task searchWithAscii2d(GroupCommand command, string imgUrl)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                Ascii2dResult ascii2dResult = await ascii2dBusiness.getAscii2dResultAsync(imgUrl);
                if (ascii2dResult is null || ascii2dResult.Items.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("ascii2d中找不到相似的图片");
                    return;
                }

                int readCount = BotConfig.SaucenaoConfig.Ascii2dReadCount <= 0 ? ascii2dResult.Items.Count : BotConfig.SaucenaoConfig.Ascii2dReadCount;
                List<Ascii2dItem> ascii2dItems = ascii2dResult.Items.Take(readCount).ToList();

                if (BotConfig.SaucenaoConfig.PullOrigin == false)
                {
                    List<BaseContent> simpleList = new List<BaseContent>();
                    simpleList.Add(new PlainContent($"ascii2d中搜索到的前{readCount}条结果如下："));
                    simpleList.AddRange(getSimpleMessage(ascii2dItems));
                    Task sendSimpleTask = sendAndRevoke(command, simpleList);
                    return;
                }

                List<Ascii2dItem> matchList = await ascii2dBusiness.getBestMatchAsync(ascii2dItems);
                if (matchList is null || matchList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("ascii2d中找不到相似的图片");
                    return;
                }

                StringBuilder resultBuilder = new StringBuilder();
                resultBuilder.AppendLine($"ascii2d中搜索到的前{readCount}条结果如下");
                foreach (Ascii2dItem ascii2dItem in matchList)
                {
                    if (ascii2dItem.SourceType == SetuSourceType.Pixiv)
                    {
                        PixivWorkInfo workInfo = ascii2dItem.PixivWorkInfo;
                        resultBuilder.AppendLine($"来源：Pixiv，标题：{workInfo.illustTitle}，pid：{workInfo.illustId}，链接：{workInfo.urls.original.ToOriginProxyUrl()}");
                    }
                    else if (ascii2dItem.SourceType == SetuSourceType.Twitter)
                    {
                        resultBuilder.AppendLine($"来源：Twitter，链接：{ascii2dItem.SourceUrl}");
                    }
                    else
                    {
                        resultBuilder.AppendLine($"来源：其他，链接：{ascii2dItem.SourceUrl}");
                    }
                }

                List<BaseContent> workMsgs = new List<BaseContent>();
                workMsgs.Add(new PlainContent(resultBuilder.ToString()));
                Task sendTask = sendAndRevoke(command, workMsgs);
            }
            catch (Exception ex)
            {
                string errMsg = $"searchWithAscii2d异常，url={imgUrl}";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
            }
        }

        /// <summary>
        /// 发送并撤回消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="saucenaoMessage"></param>
        /// <returns></returns>
        private async Task sendAndRevoke(GroupCommand command, List<BaseContent> contentList)
        {
            await command.ReplyAndRevokeAsync(contentList, BotConfig.SaucenaoConfig.RevokeInterval, true);
            if (BotConfig.SaucenaoConfig.SendPrivate)
            {
                await Task.Delay(1000);
                await command.SendTempMessageAsync(contentList);
            }
        }

        /// <summary>
        /// 发送并撤回消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="saucenaoMessage"></param>
        /// <returns></returns>
        private async Task sendAndRevoke(GroupCommand command, List<SetuContent> setuContents)
        {
            long? msgId = await command.ReplyGroupSetuAsync(setuContents, BotConfig.SaucenaoConfig.RevokeInterval, true);
            Task recordTask = recordBusiness.AddPixivRecord(setuContents, command.PlatformType, msgId, command.GroupId);
            if (BotConfig.SaucenaoConfig.SendPrivate)
            {
                await Task.Delay(1000);
                await command.SendTempSetuAsync(setuContents);
            }
        }

    }
}
