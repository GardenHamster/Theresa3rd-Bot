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

        public async Task searchSource(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            List<ImageMessage> imgList = args.Chain.Where(o => o is ImageMessage).Select(o => (ImageMessage)o).ToList();

            if (imgList == null || imgList.Count == 0)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 没有接收到你要找的图片哦~"));
                return;
            }

            if (BotConfig.SaucenaoConfig.MaxReceive > 0 && imgList.Count > BotConfig.SaucenaoConfig.MaxReceive)
            {
                imgList = imgList.Take(BotConfig.SaucenaoConfig.MaxReceive).ToList();
                await session.SendMessageWithAtAsync(args, new PlainMessage($" 总共接收到了{imgList.Count}张图片，只查找前{BotConfig.SaucenaoConfig.MaxReceive}张哦~"));
                await Task.Delay(500);
            }

            foreach (var item in imgList)
            {
                try
                {
                    SaucenaoSearch saucenaoSearch = await saucenaoBusiness.getSearchResultAsync(item.Url);
                    if (saucenaoSearch == null)
                    {
                        await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.NotFoundMsg, " 找不到相似的图哦~");
                        return;
                    }

                    decimal similar = 0;
                    string similarStr = string.IsNullOrEmpty(saucenaoSearch.Similarity) ? "" : saucenaoSearch.Similarity.Trim().Replace("%", "");
                    decimal.TryParse(similarStr, out similar);

                    if (saucenaoSearch.SourceType == SaucenaoSourceType.Pixiv)
                    {
                        List<IChatMessage> warnList = getWarnMessage(session, args, saucenaoSearch, "Pixiv");
                        FileInfo fileInfo = await pixivBusiness.downImgAsync(saucenaoSearch.pixivWorkInfoDto);
                        List<IChatMessage> groupMsgs = await getPixivMessageAsync(session, args, fileInfo, saucenaoSearch, warnList, UploadTarget.Group);
                        List<IChatMessage> tempMsgs = await getPixivMessageAsync(session, args, fileInfo, saucenaoSearch, warnList, UploadTarget.Temp);
                        SaucenaoMessage saucenaoMessage = new SaucenaoMessage(groupMsgs, tempMsgs, similar);
                        Task sendTask = sendAndRevokeMessage(session, args, saucenaoMessage);
                    }
                    if (saucenaoSearch.SourceType == SaucenaoSourceType.Twitter)
                    {
                        List<IChatMessage> warnList = getWarnMessage(session, args, saucenaoSearch, "Twitter");
                        List<IChatMessage> chatList = getSingleMessageAsync(saucenaoSearch, warnList);
                        SaucenaoMessage saucenaoMessage = new SaucenaoMessage(chatList, chatList, similar);
                        Task sendTask = sendAndRevokeMessage(session, args, saucenaoMessage);
                    }
                    if (saucenaoSearch.SourceType == SaucenaoSourceType.FanBox)
                    {
                        List<IChatMessage> warnList = getWarnMessage(session, args, saucenaoSearch, "FanBox");
                        List<IChatMessage> chatList = getSingleMessageAsync(saucenaoSearch, warnList);
                        SaucenaoMessage saucenaoMessage = new SaucenaoMessage(chatList, chatList, similar);
                        Task sendTask = sendAndRevokeMessage(session, args, saucenaoMessage);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"原图功能异常，url={item.Url}");
                    await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.ErrorMsg, " 出了点小问题，再试一次吧~");
                }
            }

            CoolingCache.SetMemberSaucenaoCooling(args.Sender.Group.Id, args.Sender.Id);
        }

        public List<IChatMessage> getWarnMessage(IMiraiHttpSession session, IGroupMessageEventArgs args, SaucenaoSearch saucenaoSearch, string source)
        {
            StringBuilder warnBuilder = new StringBuilder();
            warnBuilder.Append($"共找到 {saucenaoSearch.MatchCount} 条匹配信息，相似度：{saucenaoSearch.Similarity}，来源：{source}");
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
            return new List<IChatMessage>() {
                new PlainMessage(warnBuilder.ToString())
            };
        }


        public List<IChatMessage> getSingleMessageAsync(SaucenaoSearch saucenaoSearch, List<IChatMessage> warnList)
        {
            List<IChatMessage> chatList = new List<IChatMessage>(warnList);
            chatList.Add(new PlainMessage($"链接：{saucenaoSearch.SourceUrl}"));
            return chatList;
        }

        public async Task<List<IChatMessage>> getPixivMessageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, FileInfo fileInfo, SaucenaoSearch saucenaoSearch, List<IChatMessage> warnList, UploadTarget target)
        {
            List<IChatMessage> chatList = new List<IChatMessage>(warnList);
            PixivWorkInfo pixivWorkInfo = saucenaoSearch.pixivWorkInfoDto.body;

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

            chatList.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivWorkInfo, fileInfo, saucenaoSearch.StartDateTime)));
            return chatList;
        }

        private async Task sendAndRevokeMessage(IMiraiHttpSession session, IGroupMessageEventArgs args, SaucenaoMessage saucenaoMessage)
        {
            long groupId = args.Sender.Group.Id;
            long memberId = args.Sender.Id;
            int groupMsgId = await session.SendMessageWithAtAsync(args, saucenaoMessage.GroupMsgs);
            await Task.Delay(1000);
            await session.SendTempMessageAsync(memberId, groupId, saucenaoMessage.TempMsgs.ToArray());
            await Task.Delay(1000);

            if (saucenaoMessage.Similar > BotConfig.SaucenaoConfig.WithdrawOver)
            {
                await session.RevokeMessageAsync(Convert.ToInt32(args.Chain.First().ToString()));
                await Task.Delay(1000);
            }
            if (BotConfig.SaucenaoConfig.RevokeInterval > 0)
            {
                await Task.Delay(BotConfig.SaucenaoConfig.RevokeInterval * 1000);
                await session.RevokeMessageAsync(groupMsgId);
            }
        }

    }
}
