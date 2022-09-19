using AngleSharp.Html.Dom;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Exceptions;
using Theresa3rd_Bot.Model.Cache;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Model.Saucenao;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class SaucenaoHandler : BaseHandler
    {
        private PixivBusiness pixivBusiness;
        private SaucenaoBusiness saucenaoBusiness;
        private Ascii2dBusiness ascii2dBusiness;

        public SaucenaoHandler()
        {
            pixivBusiness = new PixivBusiness();
            saucenaoBusiness = new SaucenaoBusiness();
            ascii2dBusiness = new Ascii2dBusiness();
        }

        public async Task searchResult(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                DateTime startDateTime = DateTime.Now;
                IGroupMessageEventArgs imgArgs = args;
                CoolingCache.SetHanding(groupId, memberId);//请求处理中
                List<ImageMessage> imgList = args.Chain.Where(o => o is ImageMessage).Select(o => (ImageMessage)o).ToList();

                if (imgList == null || imgList.Count == 0)
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo == null) return;
                    StepDetail imgStep = new StepDetail(60, " 请在60秒内发送要查找的图片", CheckImageSourceAsync);
                    stepInfo.AddStep(imgStep);
                    if (await stepInfo.HandleStep(session, args) == false) return;
                    imgList = imgStep.Args.Chain.Where(o => o is ImageMessage).Select(o => (ImageMessage)o).ToList();
                    imgArgs = imgStep.Args;
                }

                if (imgList == null || imgList.Count == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 没有接收到图片，请重新发送指令开始操作"));
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
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 总共接收到了{imgList.Count}张图片，只查找前{BotConfig.SaucenaoConfig.MaxReceive}张哦~"));
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
                    await session.SendGroupMessageAsync(groupId, new PlainMessage(" 正在通过ascii2d尝试搜索原图..."));
                    for (int i = 0; i < imgList.Count; i++) await searchWithAscii2d(session, args, imgList[i]);
                }

                CoolingCache.SetMemberSaucenaoCooling(groupId, memberId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "searchResult异常");
                await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.ErrorMsg, " 出了点小问题，任务结束~");
            }
            finally
            {
                CoolingCache.SetHandFinish(args.Sender.Group.Id, args.Sender.Id);//请求处理完成
            }
        }

        private async Task<bool> CheckContinueAscii2d(IMiraiHttpSession session, IGroupMessageEventArgs args, List<ImageMessage> notFoundList)
        {
            YNAType ynaType = BotConfig.SaucenaoConfig.ContinueAscii2d;
            if (ynaType == YNAType.Yes) return true;
            if (ynaType == YNAType.No) return false;
            StepInfo stepInfo = await StepCache.CreateStepAsync(session, args, false);
            if (stepInfo == null) return false;
            StepDetail askStep = new StepDetail(30, $" 是否使用Ascii2d继续搜索剩余的图片？请在30秒内发送\r\n1：是，0：否");
            stepInfo.AddStep(askStep);
            if (await stepInfo.HandleStep(session, args) == false) return false;
            return askStep.Answer == "1";
        }

        private async Task<bool> CheckImageSourceAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            List<ImageMessage> imgList = args.Chain.Where(o => o is ImageMessage).Select(o => (ImageMessage)o).ToList();
            if (imgList == null || imgList.Count == 0) return await Task.FromResult(false);
            return await Task.FromResult(true);
        }

        private async Task<bool> searchWithSaucenao(IMiraiHttpSession session, IGroupMessageEventArgs args, ImageMessage imageMessage, int index)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                DateTime startTime = DateTime.Now;
                SaucenaoResult saucenaoResult = await saucenaoBusiness.getSaucenaoResultAsync(imageMessage.Url);
                if (saucenaoResult == null || saucenaoResult.Items.Count == 0)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.NotFoundMsg, $" 找不到与第{index}张图片相似的图");
                    return true;
                }

                if (BotConfig.SaucenaoConfig.PullOrigin == false)
                {
                    SaucenaoItem firstItem = saucenaoResult.Items[0];
                    List<IChatMessage> chatList = new List<IChatMessage>();
                    chatList.AddRange(getRemindMessage(saucenaoResult, firstItem, groupId, memberId));
                    chatList.AddRange(getSimpleMessage(firstItem));
                    SaucenaoMessage saucenaoMessage = new SaucenaoMessage(firstItem, chatList, chatList);
                    Task sendTask = sendAndRevokeMessage(session, args, saucenaoMessage);
                    return false;
                }

                SaucenaoItem saucenaoItem = await saucenaoBusiness.getBestMatchAsync(saucenaoResult);
                if (saucenaoItem == null)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.NotFoundMsg, $" 找不到与第{index}张图片相似的图");
                    return true;
                }

                if (saucenaoItem.SourceType == SetuSourceType.Pixiv)
                {
                    bool isShowImg = groupId.IsShowSaucenaoImg(saucenaoItem.PixivWorkInfo.body.isR18());
                    FileInfo fileInfo = isShowImg ? await pixivBusiness.downImgAsync(saucenaoItem.PixivWorkInfo.body) : null;
                    List<IChatMessage> remindMsgs = getRemindMessage(saucenaoResult, saucenaoItem, groupId, memberId);
                    List<IChatMessage> groupMsgs = new List<IChatMessage>(remindMsgs);
                    List<IChatMessage> tempMsgs = new List<IChatMessage>(remindMsgs);
                    groupMsgs.AddRange(await getPixivMessageAsync(session, args, saucenaoItem, UploadTarget.Group, startTime, fileInfo, isShowImg));
                    tempMsgs.AddRange(await getPixivMessageAsync(session, args, saucenaoItem, UploadTarget.Temp, startTime, fileInfo, isShowImg));
                    SaucenaoMessage saucenaoMessage = new SaucenaoMessage(saucenaoItem, groupMsgs, tempMsgs);
                    Task sendTask = sendAndRevokeMessage(session, args, saucenaoMessage);
                    return false;
                }
                else
                {
                    List<IChatMessage> chatList = new List<IChatMessage>();
                    chatList.AddRange(getRemindMessage(saucenaoResult, saucenaoItem, groupId, memberId));
                    chatList.AddRange(getSimpleMessage(saucenaoItem));
                    SaucenaoMessage saucenaoMessage = new SaucenaoMessage(saucenaoItem, chatList, chatList);
                    Task sendTask = sendAndRevokeMessage(session, args, saucenaoMessage);
                    return false;
                }
            }
            catch (BaseException ex)
            {
                LogHelper.Error(ex, $"searchWithSaucenao异常，url={imageMessage.Url}，{ex.Message}");
                await session.SendMessageWithAtAsync(args, new PlainMessage($" 查找第{index}张图片失败，{ex.Message}，再试一次吧~"));
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"searchWithSaucenao异常，url={imageMessage.Url}");
                await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.ErrorMsg, $" 查找第{index}张图片失败，再试一次吧~");
                return false;
            }
        }

        private async Task searchWithAscii2d(IMiraiHttpSession session, IGroupMessageEventArgs args, ImageMessage imageMessage)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                DateTime startTime = DateTime.Now;
                Ascii2dResult ascii2dResult = await ascii2dBusiness.getAscii2dResultAsync(imageMessage.Url);
                if (ascii2dResult == null || ascii2dResult.Items.Count == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" ascii2d中找不到相似的图片"));
                    return;
                }

                int readCount = BotConfig.SaucenaoConfig.Ascii2dReadCount <= 0 ? ascii2dResult.Items.Count : BotConfig.SaucenaoConfig.Ascii2dReadCount;
                List<Ascii2dItem> ascii2dItems = ascii2dResult.Items.Take(readCount).ToList();

                if (BotConfig.SaucenaoConfig.PullOrigin == false)
                {
                    List<IChatMessage> chatList = new List<IChatMessage>();
                    chatList.Add(new PlainMessage($" ascii2d中搜索到的前{readCount}条结果如下：\r\n"));
                    chatList.AddRange(getSimpleMessage(ascii2dItems));
                    Task sendSimpleTask = sendAndRevokeMessage(session, args, chatList);
                    return;
                }

                List<Ascii2dItem> matchList = await ascii2dBusiness.getBestMatchAsync(ascii2dItems);
                if (matchList == null || matchList.Count == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" ascii2d中找不到相似的图片"));
                    return;
                }

                StringBuilder resultBuilder = new StringBuilder();
                resultBuilder.AppendLine($" ascii2d中搜索到的前{readCount}条结果如下");
                foreach (Ascii2dItem ascii2dItem in matchList)
                {
                    if (ascii2dItem.SourceType == SetuSourceType.Pixiv)
                    {
                        PixivWorkInfo workInfo = ascii2dItem.PixivWorkInfo.body;
                        resultBuilder.AppendLine($"来源：Pixiv，标题：{workInfo.illustTitle}，pid：{workInfo.illustId}，链接：{workInfo.urls.original.ToProxyUrl()}");
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

                Task sendDetailTask = sendAndRevokeMessage(session, args, new List<IChatMessage>() {
                    new PlainMessage(resultBuilder.ToString())
                });
            }
            catch (BaseException ex)
            {
                LogHelper.Error(ex, $"searchWithAscii2d异常，url={imageMessage.Url}，{ex.Message}");
                await session.SendMessageWithAtAsync(args, new PlainMessage($" 查找图片失败，{ex.Message}，再试一次吧~"));
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"searchWithAscii2d异常，url={imageMessage.Url}");
                await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.ErrorMsg, $" 查找图片失败，再试一次吧~");
            }
        }

        public List<IChatMessage> getRemindMessage(SaucenaoResult saucenaoResult, SaucenaoItem saucenaoItem, long groupId, long memberId)
        {
            List<IChatMessage> chatList = new List<IChatMessage>();
            string remindTemplate = BotConfig.SaucenaoConfig.Template;
            long todayLeft = GetSaucenaoLeftToday(groupId, memberId);
            if (string.IsNullOrWhiteSpace(remindTemplate))
            {
                chatList.Add(new PlainMessage(saucenaoBusiness.getDefaultRemindMessage(saucenaoResult, saucenaoItem, todayLeft)));
            }
            else
            {
                chatList.Add(new PlainMessage(saucenaoBusiness.getSaucenaoRemindMessage(saucenaoResult, saucenaoItem, remindTemplate, todayLeft)));
            }
            return chatList;
        }

        public List<IChatMessage> getSimpleMessage(SaucenaoItem saucenaoItem)
        {
            List<IChatMessage> chatList = new List<IChatMessage>();
            chatList.Add(new PlainMessage($"链接：{saucenaoItem.SourceUrl}"));
            return chatList;
        }

        public List<IChatMessage> getSimpleMessage(List<Ascii2dItem> saucenaoItems)
        {
            List<IChatMessage> chatList = new List<IChatMessage>();
            foreach (var saucenaoItem in saucenaoItems)
            {
                chatList.Add(new PlainMessage($"来源:{Enum.GetName(typeof(SetuSourceType), saucenaoItem.SourceType)}，链接：{saucenaoItem.SourceUrl}"));
            }
            return chatList;
        }

        public async Task<List<IChatMessage>> getPixivMessageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, SaucenaoItem saucenaoItem, UploadTarget target, DateTime startTime, FileInfo fileInfo, bool isShowImg)
        {
            long groupId = args.Sender.Group.Id;
            string template = BotConfig.GeneralConfig.PixivTemplate;
            PixivWorkInfo pixivWorkInfo = saucenaoItem.PixivWorkInfo.body;
            List<IChatMessage> chatList = new List<IChatMessage>();
            if (pixivWorkInfo.IsImproper())
            {
                chatList.Add(new PlainMessage($" 该作品含有R18G等内容，不显示相关内容"));
                return chatList;
            }

            string banTagStr = pixivWorkInfo.hasBanTag();
            if (banTagStr != null)
            {
                chatList.Add(new PlainMessage($" 该作品含有被屏蔽的标签【{banTagStr}】，不显示相关内容"));
                return chatList;
            }

            if (pixivWorkInfo.isR18() && groupId.IsShowR18Saucenao() == false)
            {
                chatList.Add(new PlainMessage($" 该作品为R-18作品，不显示相关内容，如需显示请在配置文件中修改权限"));
                return chatList;
            }

            if (string.IsNullOrWhiteSpace(template))
            {
                chatList.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivWorkInfo, fileInfo, startTime)));
            }
            else
            {
                chatList.Add(new PlainMessage(pixivBusiness.getWorkInfo(pixivWorkInfo, fileInfo, startTime, template)));
            }

            if (isShowImg && fileInfo != null)
            {
                chatList.Add((IChatMessage)await session.UploadPictureAsync(target, fileInfo.FullName));
            }
            else if (isShowImg && fileInfo == null)
            {
                chatList.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, target));
            }

            return chatList;
        }

        /// <summary>
        /// 发送并撤回消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="saucenaoMessage"></param>
        /// <returns></returns>
        private async Task sendAndRevokeMessage(IMiraiHttpSession session, IGroupMessageEventArgs args, SaucenaoMessage saucenaoMessage)
        {
            long groupId = args.Sender.Group.Id;
            long memberId = args.Sender.Id;
            int groupMsgId = await session.SendMessageWithAtAsync(args, saucenaoMessage.GroupMsgs);
            await Task.Delay(1000);
            await session.SendTempMessageAsync(memberId, groupId, saucenaoMessage.TempMsgs.ToArray());
            await Task.Delay(1000);
            if (BotConfig.SaucenaoConfig.RevokeInterval > 0)
            {
                await Task.Delay(BotConfig.SaucenaoConfig.RevokeInterval * 1000);
                await session.RevokeMessageAsync(groupMsgId);
            }
        }

        /// <summary>
        /// 发送并撤回消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="saucenaoMessage"></param>
        /// <returns></returns>
        private async Task sendAndRevokeMessage(IMiraiHttpSession session, IGroupMessageEventArgs args, List<IChatMessage> chatList)
        {
            long groupId = args.Sender.Group.Id;
            long memberId = args.Sender.Id;
            int groupMsgId = await session.SendMessageWithAtAsync(args, chatList);
            await Task.Delay(1000);
            await session.SendTempMessageAsync(memberId, groupId, chatList.ToArray());
            await Task.Delay(1000);
            if (BotConfig.SaucenaoConfig.RevokeInterval > 0)
            {
                await Task.Delay(BotConfig.SaucenaoConfig.RevokeInterval * 1000);
                await session.RevokeMessageAsync(groupMsgId);
            }
        }

    }
}
