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
    public class SaucenaoHandler
    {
        private PixivBusiness pixivBusiness;
        private SaucenaoBusiness saucenaoBusiness;

        public SaucenaoHandler()
        {
            pixivBusiness = new PixivBusiness();
            saucenaoBusiness = new SaucenaoBusiness();
        }

        public async Task saucenaoSearch(IMiraiHttpSession session, IGroupMessageEventArgs args)
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
                    if (await stepInfo.StartStep(session, args) == false) return;
                    imgList = imgStep.Args.Chain.Where(o => o is ImageMessage).Select(o => (ImageMessage)o).ToList();
                    imgArgs = imgStep.Args;
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

                for (int i = 0; i < imgList.Count; i++) await searchSource(session, args, imgList[i], i + 1);

                if (BotConfig.SaucenaoConfig.RevokeSearched)
                {
                    await session.RevokeMessageAsync(imgArgs);
                    await Task.Delay(1000);
                }
                CoolingCache.SetMemberSaucenaoCooling(groupId, memberId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "searchSource异常");
                await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.ErrorMsg, " 出了点小问题，任务结束~");
            }
            finally
            {
                CoolingCache.SetHandFinish(args.Sender.Group.Id, args.Sender.Id);//请求处理完成
            }
        }

        private async Task<bool> CheckImageSourceAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            List<ImageMessage> imgList = args.Chain.Where(o => o is ImageMessage).Select(o => (ImageMessage)o).ToList();
            if (imgList == null || imgList.Count == 0) return await Task.FromResult(false);
            return await Task.FromResult(true);
        }

        private async Task searchSource(IMiraiHttpSession session, IGroupMessageEventArgs args, ImageMessage imageMessage, int index)
        {
            try
            {
                SaucenaoResult saucenaoResult = await saucenaoBusiness.getSaucenaoResultAsync(imageMessage.Url);
                if (saucenaoResult == null || saucenaoResult.Items.Count == 0)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.NotFoundMsg, $" 找不到与第{index}张图片相似的图");
                    return;
                }

                if (BotConfig.SaucenaoConfig.PullOrigin == false)
                {
                    SaucenaoItem firstItem = saucenaoResult.Items[0];
                    List<IChatMessage> warnList = getWarnMessage(session, args, saucenaoResult, firstItem);
                    List<IChatMessage> chatList = getSingleMessageAsync(firstItem, warnList);
                    SaucenaoMessage saucenaoMessage = new SaucenaoMessage(firstItem, chatList, chatList);
                    Task sendTask = sendAndRevokeMessage(session, args, saucenaoMessage);
                    return;
                }

                SaucenaoItem saucenaoItem = await saucenaoBusiness.getBestMatchAsync(saucenaoResult);
                if (saucenaoItem == null)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.NotFoundMsg, $" 找不到与第{index}张图片相似的图");
                    return;
                }

                if (saucenaoItem.SourceType == SaucenaoSourceType.Pixiv)
                {
                    List<IChatMessage> warnList = getWarnMessage(session, args, saucenaoResult, saucenaoItem);
                    FileInfo fileInfo = await pixivBusiness.downImgAsync(saucenaoItem.PixivWorkInfo);
                    List<IChatMessage> groupMsgs = await getPixivMessageAsync(session, args, warnList, saucenaoResult, saucenaoItem, fileInfo, UploadTarget.Group);
                    List<IChatMessage> tempMsgs = await getPixivMessageAsync(session, args, warnList, saucenaoResult, saucenaoItem, fileInfo, UploadTarget.Temp);
                    SaucenaoMessage saucenaoMessage = new SaucenaoMessage(saucenaoItem, groupMsgs, tempMsgs);
                    Task sendTask = sendAndRevokeMessage(session, args, saucenaoMessage);
                }
                else
                {
                    List<IChatMessage> warnList = getWarnMessage(session, args, saucenaoResult, saucenaoItem);
                    List<IChatMessage> chatList = getSingleMessageAsync(saucenaoItem, warnList);
                    SaucenaoMessage saucenaoMessage = new SaucenaoMessage(saucenaoItem, chatList, chatList);
                    Task sendTask = sendAndRevokeMessage(session, args, saucenaoMessage);
                }
            }
            catch (BaseException ex)
            {
                LogHelper.Error(ex, $"原图功能异常，url={imageMessage.Url}，{ex.Message}");
                await session.SendMessageWithAtAsync(args, new PlainMessage($" 查找第{index}张图片失败，{ex.Message}，再试一次吧~"));
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"原图功能异常，url={imageMessage.Url}");
                await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.ErrorMsg, $" 查找第{index}张图片失败，再试一次吧~");
            }
        }

        public List<IChatMessage> getWarnMessage(IMiraiHttpSession session, IGroupMessageEventArgs args, SaucenaoResult saucenaoResult, SaucenaoItem saucenaoItem)
        {
            StringBuilder warnBuilder = new StringBuilder();
            warnBuilder.Append($" 共找到 {saucenaoResult.MatchCount} 条匹配信息，相似度：{saucenaoItem.Similarity}%，来源：{Enum.GetName(typeof(SaucenaoSourceType), saucenaoItem.SourceType)}");
            if (BotConfig.SaucenaoConfig.MaxDaily > 0)
            {
                if (warnBuilder.Length > 0) warnBuilder.Append("，");
                warnBuilder.Append($"今天剩余使用次数{BusinessHelper.GetSaucenaoLeftToday(session, args)}次");
            }
            if (BotConfig.SaucenaoConfig.RevokeInterval > 0)
            {
                if (warnBuilder.Length > 0) warnBuilder.Append("，");
                warnBuilder.Append($"本消息将在{BotConfig.SaucenaoConfig.RevokeInterval}秒后撤回");
            }
            if (BotConfig.SaucenaoConfig.MemberCD > 0)
            {
                if (warnBuilder.Length > 0) warnBuilder.Append("，");
                warnBuilder.Append($"CD{BotConfig.SetuConfig.MemberCD}秒");
            }
            if (warnBuilder.Length > 0)
            {
                warnBuilder.Append("\r\n");
            }
            return new List<IChatMessage>() {
                new PlainMessage(warnBuilder.ToString())
            };
        }


        public List<IChatMessage> getSingleMessageAsync(SaucenaoItem saucenaoItem, List<IChatMessage> warnList)
        {
            List<IChatMessage> chatList = new List<IChatMessage>(warnList);
            chatList.Add(new PlainMessage($"链接：{saucenaoItem.SourceUrl}"));
            return chatList;
        }

        public async Task<List<IChatMessage>> getPixivMessageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, List<IChatMessage> warnList, SaucenaoResult saucenaoResult, SaucenaoItem saucenaoItem, FileInfo fileInfo, UploadTarget target)
        {
            PixivWorkInfo pixivWorkInfo = saucenaoItem.PixivWorkInfo.body;
            List<IChatMessage> chatList = new List<IChatMessage>(warnList);
            if (pixivWorkInfo.IsImproper())
            {
                chatList.Add(new PlainMessage($"该作品含有被屏蔽的标签，不显示相关内容"));
                return chatList;
            }

            if (pixivWorkInfo.isR18())
            {
                chatList.Add(new PlainMessage($"该作品为R-18作品，不显示相关内容，如需显示请在配置文件中修改权限"));
                return chatList;
            }

            chatList.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivWorkInfo, fileInfo, saucenaoResult.StartDateTime)));
            if (fileInfo == null)
            {
                chatList.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, target));
            }
            else if (pixivWorkInfo.isR18() == false)
            {
                chatList.Add((IChatMessage)await session.UploadPictureAsync(target, fileInfo.FullName));
            }
            else if (pixivWorkInfo.isR18() && args.Sender.Group.Id.IsShowR18Img())
            {
                chatList.Add((IChatMessage)await session.UploadPictureAsync(target, fileInfo.FullName));
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

    }
}
