using TheresaBot.Core.Cache;
using TheresaBot.Core.Command;
using TheresaBot.Core.Common;
using TheresaBot.Core.Exceptions;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Ascii2d;
using TheresaBot.Core.Model.Content;
using TheresaBot.Core.Model.PO;
using TheresaBot.Core.Model.Saucenao;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Services;
using TheresaBot.Core.Session;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Handler
{
    internal class SaucenaoHandler : SetuHandler
    {
        private PixivService pixivService;
        private SaucenaoService saucenaoService;
        private Ascii2dService ascii2dService;

        public SaucenaoHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivService = new PixivService();
            saucenaoService = new SaucenaoService();
            ascii2dService = new Ascii2dService();
        }

        public async Task SearchSource(GroupCommand command)
        {
            try
            {
                var revokeMsgId = command.MessageId;
                var imgList = command.GetImageUrls();
                CoolingCache.SetHanding(command.GroupId, command.MemberId);

                if (imgList.Count == 0)
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var imgStep = processInfo.CreateStep("请在60秒内发送需要查找的图片", WaitImageAsync);
                    await processInfo.StartProcessing();
                    imgList = imgStep.Relay.GetImageUrls();
                    revokeMsgId = imgStep.Relay.MsgId;
                }
                if (imgList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync($"没有接收到图片，请重新发送指令进行操作");
                    return;
                }

                await HandleSearch(command, imgList, revokeMsgId);
                CoolingCache.SetMemberSaucenaoCooling(command.GroupId, command.MemberId);
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "原图搜索功能异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        public async Task SearchSource(GroupQuoteCommand command)
        {
            try
            {
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中
                long quoteId = command.GetQuoteMessageId();
                List<ImageRecordPO> imgRecords = quoteId == 0 ? new() : recordService.GetImageRecord(quoteId, command.GroupId);
                List<string> imgList = imgRecords.Select(o => o.HttpUrl).ToList();
                if (imgList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync($"没能获取到引用消息中的图片，请尝试使用搜图指令搜索");
                    return;
                }
                long revokeMsgId = command.MessageId;
                await HandleSearch(command, imgList, revokeMsgId);
                CoolingCache.SetMemberSaucenaoCooling(command.GroupId, command.MemberId);
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "原图搜索功能异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        private async Task HandleSearch(GroupCommand command, List<string> imgList, long revokeMsgId)
        {
            await command.ReplyProcessingMessageAsync(BotConfig.SaucenaoConfig.ProcessingMsg);
            if (BotConfig.SaucenaoConfig.MaxReceive > 0 && imgList.Count > BotConfig.SaucenaoConfig.MaxReceive)
            {
                imgList = imgList.Take(BotConfig.SaucenaoConfig.MaxReceive).ToList();
                string remindMsg = $"总共接收到了 {imgList.Count} 张图片，只查找前{BotConfig.SaucenaoConfig.MaxReceive}张哦~";
                await command.ReplyGroupMessageWithAtAsync(remindMsg);
                await Task.Delay(1000);
            }

            List<string> notFoundList = new List<string>();
            for (int i = 0; i < imgList.Count; i++)
            {
                bool isFound = await SearchWithSaucenao(command, imgList[i]);
                if (isFound == false) notFoundList.Add(imgList[i]);
            }

            if (imgList.Count == notFoundList.Count && BotConfig.SaucenaoConfig.ContinueAscii2d == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.SaucenaoConfig.NotFoundMsg, "找不到相似的图，换一张完整的图片试试吧~");
            }
            else if (notFoundList.Count > 0 && BotConfig.SaucenaoConfig.ContinueAscii2d)
            {
                await command.ReplyGroupMessageWithAtAsync($"Saucenao共有{notFoundList.Count}张图片搜索失败，正在通过ascii2d尝试搜索剩余图片...");
                for (int i = 0; i < notFoundList.Count; i++) await SearchWithAscii2d(command, notFoundList[i]);
            }
            else if (notFoundList.Count > 0)
            {
                await command.ReplyGroupMessageWithAtAsync($"Saucenao搜索完毕，共有{notFoundList.Count}张图片搜索失败");
            }

            if (BotConfig.SaucenaoConfig.RevokeSearched)
            {
                await Task.Delay(1000);
                await command.RevokeGroupMessageAsync(revokeMsgId, command.GroupId);
            }
        }

        private async Task<bool> SearchWithSaucenao(GroupCommand command, string imgUrl)
        {
            try
            {
                var readCount = BotConfig.SaucenaoConfig.SaucenaoReadCount;
                var saucenaoResult = await saucenaoService.SearchResultAsync(imgUrl);
                var filterList = saucenaoService.FilterItems(saucenaoResult.Items);
                if (filterList.Count == 0) return false;

                var sortList = saucenaoService.SortItems(filterList);
                var sendList = new List<SaucenaoItem>();
                var maxSimilarity = sortList.Max(o => o.Similarity);
                var singlePriority = BotConfig.SaucenaoConfig.SinglePriority;

                if (maxSimilarity >= singlePriority)
                {
                    sendList = sortList.Where(o => o.SourceType == SetuSourceType.Pixiv).Take(1).ToList();
                }
                if (sendList.Count == 0)
                {
                    sendList = sortList.Take(readCount).ToList();
                }

                if (BotConfig.SaucenaoConfig.PullOrigin == false)
                {
                    List<BaseContent> workMsgs = new List<BaseContent>();
                    workMsgs.AddRange(saucenaoService.GetRemindContent(saucenaoResult, command.GroupId, command.MemberId));
                    workMsgs.AddRange(sendList.Select(o => o.GetSimpleContent()));
                    Task sendSimpleTask = ReplyAndRevoke(command, workMsgs);
                    return true;
                }

                await saucenaoService.FetchOrigin(sortList);
                var setuContents = new List<SetuContent>()
                {
                    new(saucenaoService.GetRemindContent(saucenaoResult, command.GroupId, command.MemberId))
                };

                for (int i = 0; i < sendList.Count; i++)
                {
                    var content = await GetSaucenaoContentAsync(command, sendList[i]);
                    setuContents.Add(content);
                }
                await ReplyAndRevoke(command, setuContents);
                var firstPixivItem = sortList.Where(o => o.SourceType == SetuSourceType.Pixiv).FirstOrDefault();
                if (firstPixivItem.Similarity > singlePriority && firstPixivItem.PixivWorkInfo is not null && BotConfig.SaucenaoConfig.SendPrivate && BotConfig.SaucenaoConfig.SendPrivateOrigin && BotConfig.PixivConfig.ImgSize != PixivImageSize.Original)
                {
                    await Task.Delay(1000);
                    Task task = SendPrivateOriginSetuAsync(firstPixivItem.PixivWorkInfo, command.GroupId, command.MemberId);
                }
                return true;
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "SearchWithSaucenao异常");
                return true;
            }
        }

        private async Task<SetuContent> GetSaucenaoContentAsync(GroupCommand command, SaucenaoItem saucenaoItem)
        {
            if (saucenaoItem.SourceType == SetuSourceType.Pixiv && saucenaoItem.PixivWorkInfo is not null)
            {
                return await GetPixivSetuContentAsync(command, saucenaoItem);
            }
            else
            {
                return new(saucenaoItem.GetSourceContent());
            }
        }

        private async Task<SetuContent> GetPixivSetuContentAsync(GroupCommand command, SaucenaoItem saucenaoItem)
        {
            var groupId = command.GroupId;
            var isShowR18 = command.GroupId.IsShowR18Saucenao();
            var imagePriority = BotConfig.SaucenaoConfig.ImagePriority;
            var pixivWorkInfo = saucenaoItem.PixivWorkInfo;
            var notSendableMsg = CheckSendable(pixivWorkInfo, isShowR18);
            if (string.IsNullOrWhiteSpace(notSendableMsg) == false)
            {
                return new(saucenaoItem.GetSimpleContent(), new PlainContent(notSendableMsg));
            }
            var setuFiles = new List<FileInfo>();
            if (saucenaoItem.Similarity >= imagePriority)
            {
                setuFiles = await DownSetuFilesAsync(pixivWorkInfo, groupId);
            }
            var workInfo = pixivService.GetWorkInfo(pixivWorkInfo);
            var workInfoContent = new PlainContent(workInfo);
            var sourceContent = saucenaoItem.GetSimpleContent();
            var workMsgs = new List<BaseContent>() { sourceContent, workInfoContent };
            return new PixivSetuContent(workMsgs, setuFiles, pixivWorkInfo);
        }

        private async Task SearchWithAscii2d(GroupCommand command, string imgUrl)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                Ascii2dResult ascii2dResult = await ascii2dService.SearchResultAsync(imgUrl);
                if (ascii2dResult.Items.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("ascii2d中找不到相似的图片");
                    return;
                }
                var readCount = BotConfig.SaucenaoConfig.Ascii2dReadCount;
                var sendItems = ascii2dResult.Items.Take(readCount).ToList();
                if (BotConfig.SaucenaoConfig.PullOrigin)
                {
                    await ascii2dService.FetchOrigin(sendItems);
                    List<BaseContent> contentList = new List<BaseContent>();
                    contentList.Add(new PlainContent($"ascii2d中搜索到的前{readCount}条结果如下"));
                    contentList.AddRange(sendItems.Select(o => o.GetSourceContent()));
                    Task sendTask = ReplyAndRevoke(command, contentList);
                }
                else
                {
                    List<BaseContent> contentList = new List<BaseContent>();
                    contentList.Add(new PlainContent($"ascii2d中搜索到的前{readCount}条结果如下："));
                    contentList.AddRange(sendItems.Select(o => o.GetSimpleContent()));
                    Task sendSimpleTask = ReplyAndRevoke(command, contentList);
                }
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "SearchWithAscii2d异常");
            }
        }

        private async Task ReplyAndRevoke(GroupCommand command, List<BaseContent> contentList)
        {
            await command.ReplyAndRevokeAsync(contentList, BotConfig.SaucenaoConfig.RevokeInterval);
            if (BotConfig.SaucenaoConfig.SendPrivate)
            {
                await Task.Delay(1000);
                await command.SendPrivateMessageAsync(contentList);
            }
        }

        private async Task ReplyAndRevoke(GroupCommand command, List<SetuContent> setuContents)
        {
            var result = await command.ReplyGroupSetuAsync(setuContents, BotConfig.SaucenaoConfig.RevokeInterval);
            var recordTask = recordService.InsertPixivRecord(setuContents, Session.PlatformType, result.MessageId, command.GroupId);
            if (BotConfig.SaucenaoConfig.SendPrivate)
            {
                await Task.Delay(1000);
                await command.SendPrivateSetuAsync(setuContents);
            }
        }

    }
}
