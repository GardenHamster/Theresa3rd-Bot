using System.Text;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Lolisuki;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.Saucenao;
using TheresaBot.Main.Model.Step;
using TheresaBot.Main.Relay;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    public class SaucenaoHandler : SetuHandler
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
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中
                List<string> imgList = command.GetReplyImageUrls();
                int revokeMsgId = command.MsgId;

                if (imgList is null || imgList.Count == 0)
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(command);
                    if (stepInfo is null) return;
                    StepDetail imgStep = new StepDetail(60, "请在60秒内发送要查找的图片", CheckImageSourceAsync);
                    stepInfo.AddStep(imgStep);
                    if (await stepInfo.HandleStep() == false) return;
                    imgList = imgStep.Relay.GetReplyImageUrls();
                    revokeMsgId = imgStep.Relay.MsgId;
                }

                if (imgList is null || imgList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync($"没有接收到图片，请重新发送指令开始操作");
                    return;
                }

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
                    bool isNotFound = await searchWithSaucenao(command, imgList[i], i + 1);
                    if (isNotFound) notFoundList.Add(imgList[i]);
                }

                if (BotConfig.SaucenaoConfig.RevokeSearched)
                {
                    await Task.Delay(1000);
                    await command.RevokeGroupMessageAsync(revokeMsgId, command.GroupId);
                }

                if (notFoundList.Count > 0 && await CheckContinueAscii2d(command, notFoundList))
                {
                    await Task.Delay(1000);
                    await command.ReplyGroupMessageAsync("正在通过ascii2d尝试搜索原图...");
                    for (int i = 0; i < imgList.Count; i++) await searchWithAscii2d(command, imgList[i]);
                }

                CoolingCache.SetMemberSaucenaoCooling(command.GroupId, command.MemberId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "searchResult异常");
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, "searchResult异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
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
            questionBuilder.AppendLine($"下列图片搜索失败，是否使用Ascii2d继续搜索剩余的图片？");
            questionBuilder.AppendLine($"请在30秒内发送 1：是，0：否");
            foreach (string imgUrl in notFoundList) questionBuilder.AppendLine(imgUrl);
            StepDetail askStep = new StepDetail(30, questionBuilder.ToString(), null);
            stepInfo.AddStep(askStep);
            if (await stepInfo.HandleStep() == false) return false;
            return askStep.Answer == "1";
        }

        private async Task<bool> CheckImageSourceAsync(GroupCommand command, GroupRelay relay)
        {
            List<string> imgList = relay.GetReplyImageUrls();
            if (imgList is null || imgList.Count == 0) return await Task.FromResult(false);
            return await Task.FromResult(true);
        }

        private async Task<bool> searchWithSaucenao(GroupCommand command, string imgUrl, int index)
        {
            try
            {
                SaucenaoResult saucenaoResult = await saucenaoBusiness.getSaucenaoResultAsync(imgUrl);
                if (saucenaoResult is null || saucenaoResult.Items.Count == 0)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.SaucenaoConfig.NotFoundMsg, $" 找不到与第{index}张图片相似的图");
                    return true;
                }

                if (BotConfig.SaucenaoConfig.PullOrigin == false)
                {
                    SaucenaoItem firstItem = saucenaoResult.Items[0];
                    List<BaseContent> workMsgs = new List<BaseContent>();
                    workMsgs.AddRange(getRemindMessage(saucenaoResult, firstItem, command.GroupId, command.MemberId));
                    workMsgs.AddRange(getSimpleMessage(firstItem));
                    Task sendTask = sendAndRevokeMessage(command, workMsgs);
                    return false;
                }

                SaucenaoItem saucenaoItem = await saucenaoBusiness.getBestMatchAsync(saucenaoResult);
                if (saucenaoItem is null)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.SaucenaoConfig.NotFoundMsg, $" 找不到与第{index}张图片相似的图");
                    return true;
                }

                if (saucenaoItem.SourceType == SetuSourceType.Pixiv)
                {
                    PixivWorkInfo pixivWorkInfo = saucenaoItem.PixivWorkInfo;
                    List<FileInfo> setuFiles = await GetSetuFilesAsync(pixivWorkInfo, command.GroupId);
                    List<BaseContent> workMsgs = new List<BaseContent>();
                    workMsgs.AddRange(getRemindMessage(saucenaoResult, saucenaoItem, command.GroupId, command.MemberId));
                    workMsgs.AddRange(getPixivMessageAsync(command, saucenaoItem));
                    Task sendTask = sendAndRevokeMessage(command, workMsgs, setuFiles);
                    return false;
                }
                else
                {
                    List<BaseContent> workMsgs = new List<BaseContent>();
                    workMsgs.AddRange(getRemindMessage(saucenaoResult, saucenaoItem, command.GroupId, command.MemberId));
                    workMsgs.AddRange(getSimpleMessage(saucenaoItem));
                    Task sendTask = sendAndRevokeMessage(command, workMsgs);
                    return false;
                }
            }
            catch (Exception ex)
            {
                string errMsg = $"searchWithSaucenao异常，url={imgUrl}";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex, errMsg);
                await Task.Delay(1000);
                Reporter.SendError(ex, errMsg);
                return false;
            }
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
                    simpleList.Add(new PlainContent($" ascii2d中搜索到的前{readCount}条结果如下：\r\n"));
                    simpleList.AddRange(getSimpleMessage(ascii2dItems));
                    Task sendSimpleTask = sendAndRevokeMessage(command, simpleList);
                    return;
                }

                List<Ascii2dItem> matchList = await ascii2dBusiness.getBestMatchAsync(ascii2dItems);
                if (matchList is null || matchList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("ascii2d中找不到相似的图片");
                    return;
                }

                StringBuilder resultBuilder = new StringBuilder();
                resultBuilder.AppendLine($" ascii2d中搜索到的前{readCount}条结果如下");
                foreach (Ascii2dItem ascii2dItem in matchList)
                {
                    if (ascii2dItem.SourceType == SetuSourceType.Pixiv)
                    {
                        PixivWorkInfo workInfo = ascii2dItem.PixivWorkInfo;
                        resultBuilder.AppendLine($"来源：Pixiv，标题：{workInfo.illustTitle}，pid：{workInfo.illustId}，链接：{workInfo.urls.original.ToOrginProxyUrl()}");
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
                Task sendTask = sendAndRevokeMessage(command, workMsgs);
            }
            catch (Exception ex)
            {
                string errMsg = $"searchWithAscii2d异常，url={imgUrl}";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex, errMsg);
                await Task.Delay(1000);
                Reporter.SendError(ex, errMsg);
            }
        }

        public List<BaseContent> getRemindMessage(SaucenaoResult saucenaoResult, SaucenaoItem saucenaoItem, long groupId, long memberId)
        {
            List<BaseContent> msgList = new List<BaseContent>();
            string remindTemplate = BotConfig.SaucenaoConfig.Template;
            long todayLeft = GetSaucenaoLeftToday(groupId, memberId);
            if (string.IsNullOrWhiteSpace(remindTemplate))
            {
                msgList.Add(new PlainContent(saucenaoBusiness.getDefaultRemindMessage(saucenaoResult, saucenaoItem, todayLeft)));
            }
            else
            {
                msgList.Add(new PlainContent(saucenaoBusiness.getSaucenaoRemindMessage(saucenaoResult, saucenaoItem, remindTemplate, todayLeft)));
            }
            return msgList;
        }

        public List<BaseContent> getSimpleMessage(SaucenaoItem saucenaoItem)
        {
            return new() { new PlainContent($"链接：{saucenaoItem.SourceUrl}") };
        }

        public List<BaseContent> getSimpleMessage(List<Ascii2dItem> saucenaoItems)
        {
            List<BaseContent> msgList = new List<BaseContent>();
            foreach (var saucenaoItem in saucenaoItems)
            {
                msgList.Add(new PlainContent($"来源:{Enum.GetName(typeof(SetuSourceType), saucenaoItem.SourceType)}，链接：{saucenaoItem.SourceUrl}"));
            }
            return msgList;
        }

        public List<BaseContent> getPixivMessageAsync(GroupCommand command, SaucenaoItem saucenaoItem)
        {
            PixivWorkInfo pixivWorkInfo = saucenaoItem.PixivWorkInfo;
            List<BaseContent> msgList = new List<BaseContent>();

            if (pixivWorkInfo.IsImproper)
            {
                msgList.Add(new PlainContent($" 该作品含有R18G等内容，不显示相关内容"));
                return msgList;
            }

            string banTagStr = pixivWorkInfo.hasBanTag();
            if (banTagStr != null)
            {
                msgList.Add(new PlainContent($" 该作品含有被屏蔽的标签【{banTagStr}】，不显示相关内容"));
                return msgList;
            }

            if (pixivWorkInfo.IsR18 && command.GroupId.IsShowR18Saucenao() == false)
            {
                msgList.Add(new PlainContent($" 该作品为R-18作品，不显示相关内容，如需显示请在配置文件中修改权限"));
                return msgList;
            }

            msgList.Add(new PlainContent(pixivBusiness.getWorkInfo(pixivWorkInfo, BotConfig.PixivConfig.Template)));
            return msgList;
        }

        /// <summary>
        /// 发送并撤回消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="saucenaoMessage"></param>
        /// <returns></returns>
        private async Task sendAndRevokeMessage(GroupCommand command, List<BaseContent> workMsgs, List<FileInfo> setuFiles = null)
        {
            Task sendGroupTask = command.ReplyGroupSetuAndRevokeAsync(workMsgs, setuFiles, BotConfig.SaucenaoConfig.RevokeInterval, true);
            if (BotConfig.SaucenaoConfig.SendPrivate)
            {
                await Task.Delay(1000);
                Task sendTempTask = command.SendTempSetuAsync(workMsgs, setuFiles);
            }
        }

    }
}
