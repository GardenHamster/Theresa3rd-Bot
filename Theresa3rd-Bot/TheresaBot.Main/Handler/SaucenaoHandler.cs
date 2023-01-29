using System.Text;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.Saucenao;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    public class SaucenaoHandler : SetuHandler
    {
        private PixivBusiness pixivBusiness;
        private SaucenaoBusiness saucenaoBusiness;
        private Ascii2dBusiness ascii2dBusiness;

        public SaucenaoHandler(BaseSession session) : base(session)
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
                List<string> imgList = command.GetImageUrls();

                if (imgList is null || imgList.Count == 0)
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(command);
                    if (stepInfo is null) return;
                    StepDetail imgStep = new StepDetail(60, " 请在60秒内发送要查找的图片", CheckImageSourceAsync);
                    stepInfo.AddStep(imgStep);
                    if (await stepInfo.HandleStep() == false) return;
                    imgList = imgStep.Args.Chain.Where(o => o is ImageMessage).Select(o => (ImageMessage)o).ToList();
                }

                if (imgList is null || imgList.Count == 0)
                {
                    await session.ReplyGroupMessageWithAtAsync(args, $"没有接收到图片，请重新发送指令开始操作");
                    return;
                }

                if (string.IsNullOrWhiteSpace(BotConfig.SaucenaoConfig.ProcessingMsg) == false)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.ProcessingMsg, null);
                    await Task.Delay(1000);
                }

                if (BotConfig.SaucenaoConfig.MaxReceive > 0 && imgList.Count > BotConfig.SaucenaoConfig.MaxReceive)
                {
                    imgList = imgList.Take(BotConfig.SaucenaoConfig.MaxReceive).ToList();
                    await session.ReplyGroupMessageWithAtAsync(args, $"总共接收到了{imgList.Count}张图片，只查找前{BotConfig.SaucenaoConfig.MaxReceive}张哦~");
                    await Task.Delay(1000);
                }

                List<ImageMessage> notFoundList = new List<ImageMessage>();
                for (int i = 0; i < imgList.Count; i++)
                {
                    bool isNotFound = await searchWithSaucenao(session, args, imgList[i], i + 1);
                    if (isNotFound) notFoundList.Add(imgList[i]);
                }

                if (BotConfig.SaucenaoConfig.RevokeSearched)
                {
                    await Task.Delay(1000);
                    await session.RevokeMessageAsync(imgArgs);
                }

                if (notFoundList.Count > 0 && await CheckContinueAscii2d(session, args, notFoundList))
                {
                    await Task.Delay(1000);
                    await session.SendGroupMessageAsync(args, "正在通过ascii2d尝试搜索原图...");
                    for (int i = 0; i < imgList.Count; i++) await searchWithAscii2d(session, args, imgList[i]);
                }

                CoolingCache.SetMemberSaucenaoCooling(groupId, memberId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "searchResult异常");
                await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.ErrorMsg, " 出了点小问题，任务结束~");
                ReportHelper.SendError(ex, "searchResult异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(args.Sender.Group.Id, args.Sender.Id);//请求处理完成
            }
        }

        private async Task<bool> CheckContinueAscii2d(GroupCommand command)
        {
            YNAType ynaType = BotConfig.SaucenaoConfig.ContinueAscii2d;
            if (ynaType == YNAType.Yes) return true;
            if (ynaType == YNAType.No) return false;
            StepInfo stepInfo = await StepCache.CreateStepAsync(command, false);
            if (stepInfo is null) return false;
            StepDetail askStep = new StepDetail(30, $"是否使用Ascii2d继续搜索剩余的图片？请在30秒内发送\r\n1：是，0：否");
            stepInfo.AddStep(askStep);
            if (await stepInfo.HandleStep() == false) return false;
            return askStep.Answer == "1";
        }

        private async Task<bool> CheckImageSourceAsync(GroupCommand command, string value)
        {
            List<ImageMessage> imgList = args.Chain.Where(o => o is ImageMessage).Select(o => (ImageMessage)o).ToList();
            if (imgList is null || imgList.Count == 0) return await Task.FromResult(false);
            return await Task.FromResult(true);
        }

        private async Task<bool> searchWithSaucenao(GroupCommand command, ImageMessage imageMessage, int index)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                SaucenaoResult saucenaoResult = await saucenaoBusiness.getSaucenaoResultAsync(imageMessage.Url);
                if (saucenaoResult is null || saucenaoResult.Items.Count == 0)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.SaucenaoConfig.NotFoundMsg, $" 找不到与第{index}张图片相似的图");
                    return true;
                }

                if (BotConfig.SaucenaoConfig.PullOrigin == false)
                {
                    SaucenaoItem firstItem = saucenaoResult.Items[0];
                    List<IChatMessage> workMsgs = new List<IChatMessage>();
                    workMsgs.AddRange(getRemindMessage(saucenaoResult, firstItem, groupId, memberId));
                    workMsgs.AddRange(getSimpleMessage(firstItem));
                    Task sendTask = sendAndRevokeMessage(session, args, workMsgs);
                    return false;
                }

                SaucenaoItem saucenaoItem = await saucenaoBusiness.getBestMatchAsync(saucenaoResult);
                if (saucenaoItem is null)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.NotFoundMsg, $" 找不到与第{index}张图片相似的图");
                    return true;
                }

                if (saucenaoItem.SourceType == SetuSourceType.Pixiv)
                {
                    PixivWorkInfo pixivWorkInfo = saucenaoItem.PixivWorkInfo;
                    bool isShowImg = groupId.IsShowSaucenaoImg(pixivWorkInfo.IsR18);
                    List<FileInfo> setuFiles = isShowImg ? await pixivBusiness.downPixivImgsAsync(pixivWorkInfo) : null;
                    List<IChatMessage> workMsgs = new List<IChatMessage>();
                    workMsgs.AddRange(getRemindMessage(saucenaoResult, saucenaoItem, groupId, memberId));
                    workMsgs.AddRange(getPixivMessageAsync(session, args, saucenaoItem, startTime));
                    Task sendTask = sendAndRevokeMessage(session, args, workMsgs, setuFiles);
                    return false;
                }
                else
                {
                    List<IChatMessage> workMsgs = new List<IChatMessage>();
                    workMsgs.AddRange(getRemindMessage(saucenaoResult, saucenaoItem, groupId, memberId));
                    workMsgs.AddRange(getSimpleMessage(saucenaoItem));
                    Task sendTask = sendAndRevokeMessage(session, args, workMsgs);
                    return false;
                }
            }
            catch (Exception ex)
            {
                string errMsg = $"searchWithSaucenao异常，url={imageMessage.Url}";
                LogHelper.Error(ex, errMsg);
                await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.ErrorMsg, $"查找第{index}张图片失败，再试一次吧~");
                ReportHelper.SendError(ex, errMsg);
                return false;
            }
        }

        private async Task searchWithAscii2d(GroupCommand command, ImageMessage imageMessage)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                Ascii2dResult ascii2dResult = await ascii2dBusiness.getAscii2dResultAsync(imageMessage.Url);
                if (ascii2dResult is null || ascii2dResult.Items.Count == 0)
                {
                    await session.ReplyGroupMessageWithAtAsync(args, "ascii2d中找不到相似的图片");
                    return;
                }

                int readCount = BotConfig.SaucenaoConfig.Ascii2dReadCount <= 0 ? ascii2dResult.Items.Count : BotConfig.SaucenaoConfig.Ascii2dReadCount;
                List<Ascii2dItem> ascii2dItems = ascii2dResult.Items.Take(readCount).ToList();

                if (BotConfig.SaucenaoConfig.PullOrigin == false)
                {
                    List<IChatMessage> simpleList = new List<IChatMessage>();
                    simpleList.Add(new PlainMessage($" ascii2d中搜索到的前{readCount}条结果如下：\r\n"));
                    simpleList.AddRange(getSimpleMessage(ascii2dItems));
                    Task sendSimpleTask = sendAndRevokeMessage(session, args, simpleList);
                    return;
                }

                List<Ascii2dItem> matchList = await ascii2dBusiness.getBestMatchAsync(ascii2dItems);
                if (matchList is null || matchList.Count == 0)
                {
                    await session.ReplyGroupMessageWithAtAsync(args, "ascii2d中找不到相似的图片");
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

                List<IChatMessage> workMsgs = new List<IChatMessage>();
                workMsgs.Add(new PlainMessage(resultBuilder.ToString()));
                Task sendTask = sendAndRevokeMessage(session, args, workMsgs);
            }
            catch (Exception ex)
            {
                string errMsg = $"searchWithAscii2d异常，url={imageMessage.Url}";
                LogHelper.Error(ex, errMsg);
                await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.ErrorMsg, $"查找图片失败，再试一次吧~");
                ReportHelper.SendError(ex, errMsg);
            }
        }

        public List<IChatMessage> getRemindMessage(SaucenaoResult saucenaoResult, SaucenaoItem saucenaoItem, long groupId, long memberId)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            string remindTemplate = BotConfig.SaucenaoConfig.Template;
            long todayLeft = GetSaucenaoLeftToday(groupId, memberId);
            if (string.IsNullOrWhiteSpace(remindTemplate))
            {
                msgList.Add(new PlainMessage(saucenaoBusiness.getDefaultRemindMessage(saucenaoResult, saucenaoItem, todayLeft)));
            }
            else
            {
                msgList.Add(new PlainMessage(saucenaoBusiness.getSaucenaoRemindMessage(saucenaoResult, saucenaoItem, remindTemplate, todayLeft)));
            }
            return msgList;
        }

        public List<IChatMessage> getSimpleMessage(SaucenaoItem saucenaoItem)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new PlainMessage($"链接：{saucenaoItem.SourceUrl}"));
            return msgList;
        }

        public List<IChatMessage> getSimpleMessage(List<Ascii2dItem> saucenaoItems)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            foreach (var saucenaoItem in saucenaoItems)
            {
                msgList.Add(new PlainMessage($"来源:{Enum.GetName(typeof(SetuSourceType), saucenaoItem.SourceType)}，链接：{saucenaoItem.SourceUrl}"));
            }
            return msgList;
        }

        public List<IChatMessage> getPixivMessageAsync(GroupCommand command, SaucenaoItem saucenaoItem, DateTime startTime)
        {
            long groupId = args.Sender.Group.Id;
            string template = BotConfig.PixivConfig.Template;
            PixivWorkInfo pixivWorkInfo = saucenaoItem.PixivWorkInfo;
            List<IChatMessage> msgList = new List<IChatMessage>();

            if (pixivWorkInfo.IsImproper)
            {
                msgList.Add(new PlainMessage($" 该作品含有R18G等内容，不显示相关内容"));
                return msgList;
            }

            string banTagStr = pixivWorkInfo.hasBanTag();
            if (banTagStr != null)
            {
                msgList.Add(new PlainMessage($" 该作品含有被屏蔽的标签【{banTagStr}】，不显示相关内容"));
                return msgList;
            }

            if (pixivWorkInfo.IsR18 && groupId.IsShowR18Saucenao() == false)
            {
                msgList.Add(new PlainMessage($" 该作品为R-18作品，不显示相关内容，如需显示请在配置文件中修改权限"));
                return msgList;
            }

            if (string.IsNullOrWhiteSpace(template))
            {
                msgList.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivWorkInfo, startTime)));
            }
            else
            {
                msgList.Add(new PlainMessage(pixivBusiness.getWorkInfo(pixivWorkInfo, startTime, template)));
            }

            return msgList;
        }

        /// <summary>
        /// 发送并撤回消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="saucenaoMessage"></param>
        /// <returns></returns>
        private async Task sendAndRevokeMessage(GroupCommand command, List<IChatMessage> workMsgs, List<FileInfo> setuFiles = null)
        {
            Task sendGroupTask = session.SendGroupSetuAndRevokeAsync(args, workMsgs, setuFiles, BotConfig.SaucenaoConfig.RevokeInterval, true);
            if (BotConfig.SaucenaoConfig.SendPrivate)
            {
                await Task.Delay(1000);
                Task sendTempTask = session.SendTempSetuAsync(args, workMsgs, setuFiles);
            }
        }

    }
}
