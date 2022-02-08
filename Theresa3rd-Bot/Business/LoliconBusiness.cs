using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Lolicon;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class LoliconBusiness
    {
        public async Task sendGeneralLoliconImageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(args.Sender.Group.Id, args.Sender.Id);//请求处理中

                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.Lolicon.ProcessingMsg) == false)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Lolicon.ProcessingMsg, null);
                    await Task.Delay(1000);
                }

                LoliconResultV2 loliconResult=null;
                string tagNames = message.splitKeyWord(BotConfig.SetuConfig.Lolicon.Command) ?? "";
                if (string.IsNullOrEmpty(tagNames))
                {
                    loliconResult = getLoliconResult(null);
                }
                else
                {
                    string[] tagArr = tagNames.Split(new char[] { ' ', ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                    if (BusinessHelper.CheckSTCustomEnableAsync(session, args).Result == false) return;
                    loliconResult = getLoliconResult(tagArr);
                }

                if (loliconResult == null|| loliconResult.data.Count==0)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Lolicon.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LoliconDataV2 loliconData = loliconResult.data.First();
                int todayLeftCount = BusinessHelper.GetSTLeftToday(session, args);
                FileInfo fileInfo = downImg(loliconData);

                int groupMsgId = 0;
                string template = BotConfig.SetuConfig.Lolicon.Template;
                List<IChatMessage> chatList = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(template))
                {
                    StringBuilder warnBuilder = new StringBuilder();
                    if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(args.Sender.Group.Id) == false)
                    {
                        if (warnBuilder.Length > 0) warnBuilder.Append("，");
                        warnBuilder.Append($"{BotConfig.SetuConfig.MemberCD}秒后再来哦");
                    }
                    if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(args.Sender.Group.Id) == false)
                    {
                        if (warnBuilder.Length > 0) warnBuilder.Append("，");
                        warnBuilder.Append($"今天剩余使用次数{todayLeftCount}次");
                    }
                    if (BotConfig.SetuConfig.RevokeInterval > 0)
                    {
                        if (warnBuilder.Length > 0) warnBuilder.Append("，");
                        warnBuilder.Append($"本消息将在{BotConfig.SetuConfig.RevokeInterval}秒后撤回，尽快保存哦");
                    }

                    chatList.Add(new PlainMessage(warnBuilder.ToString()));
                    chatList.Add(new PlainMessage(getDefaultWorkInfo(loliconData, fileInfo, startDateTime)));
                }
                else
                {
                    chatList.Add(new PlainMessage(getWorkInfo(loliconData, fileInfo, startDateTime, todayLeftCount, template)));
                }

                try
                {
                    //发送群消息
                    List<IChatMessage> groupList = new List<IChatMessage>(chatList);
                    if (fileInfo == null)
                    {
                        groupList.AddRange(session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg).Result);
                    }
                    else if (loliconData.isR18() == false)
                    {
                        groupList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                    }
                    groupMsgId = await session.SendMessageWithAtAsync(args, groupList);
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "sendGeneralLoliconImageAsync群消息发送失败");
                    throw;
                }


                if (BotConfig.SetuConfig.SendPrivate)
                {
                    try
                    {
                        //发送临时会话
                        List<IChatMessage> memberList = new List<IChatMessage>(chatList);
                        if (fileInfo == null)
                        {
                            memberList.AddRange(session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, UploadTarget.Friend).Result);
                        }
                        if (loliconData.isR18() == false && fileInfo != null)
                        {
                            memberList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Friend, fileInfo.FullName));
                        }
                        await session.SendFriendMessageAsync(args.Sender.Id, memberList.ToArray());
                        await Task.Delay(1000);
                    }
                    catch (Exception)
                    {
                    }
                }

                //进入CD状态
                CoolingCache.SetMemberSTCooling(args.Sender.Group.Id, args.Sender.Id);
                if (groupMsgId == 0 || BotConfig.SetuConfig.RevokeInterval == 0) return;

                try
                {
                    //等待撤回
                    await Task.Delay(BotConfig.SetuConfig.RevokeInterval * 1000);
                    await session.RevokeMessageAsync(groupMsgId);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "sendGeneralLoliconImageAsync消息撤回失败");
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "sendGeneralLoliconImageAsync异常");
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Pixiv.ErrorMsg, " 获取图片出错了，再试一次吧~");
            }
            finally
            {
                CoolingCache.SetHandFinish(args.Sender.Group.Id, args.Sender.Id);//请求处理完成
            }
        }

        public string getWorkInfo(LoliconDataV2 loliconData, FileInfo fileInfo, DateTime startTime, int todayLeft, string template = "")
        {
            if (string.IsNullOrWhiteSpace(template)) return getDefaultWorkInfo(loliconData, fileInfo, startTime);
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            double sizeMB = fileInfo == null ? 0 : MathHelper.getMbWithByte(fileInfo.Length);
            template = template.Replace("{TodayLeft}", todayLeft.ToString());
            template = template.Replace("{MemberCD}", BotConfig.SetuConfig.MemberCD.ToString());
            template = template.Replace("{RevokeInterval}", BotConfig.SetuConfig.RevokeInterval.ToString());
            template = template.Replace("{IllustTitle}", loliconData.title);
            template = template.Replace("{UserName}", loliconData.author);
            template = template.Replace("{UserId}", loliconData.uid);
            template = template.Replace("{SizeMB}", sizeMB.ToString());
            template = template.Replace("{CostSecond}", costSecond.ToString());
            template = template.Replace("{Tags}", string.Join('，', loliconData.tags ?? new string[] { }));
            template = template.Replace("{Urls}", getProxyUrl(loliconData.urls.original));
            return template;
        }

        public string getDefaultWorkInfo(LoliconDataV2 loliconData, FileInfo fileInfo, DateTime startTime)
        {
            StringBuilder workInfoStr = new StringBuilder();
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            double sizeMB = fileInfo == null ? 0 : MathHelper.getMbWithByte(fileInfo.Length);
            workInfoStr.AppendLine($"标题：{loliconData.title}，画师：{loliconData.author}，画师id：{loliconData.uid}，大小：{sizeMB}MB，耗时：{costSecond}s");
            workInfoStr.AppendLine($"标签：{string.Join('，', loliconData.tags ?? new string[] { })}");
            workInfoStr.Append(getProxyUrl(loliconData.urls.original));
            return workInfoStr.ToString();
        }

        private LoliconResultV2 getLoliconResult(string[] tags)
        {
            LoliconParamV2 param = new LoliconParamV2(false, 1, "i.pixiv.re", tags == null || tags.Length == 0 ? null : tags);
            string httpUrl = HttpUrl.getLoliconApiV2Url();
            string postJson = JsonConvert.SerializeObject(param);
            string json = HttpHelper.HttpPostJson(httpUrl, postJson);
            return JsonConvert.DeserializeObject<LoliconResultV2>(json);
        }

        private string getOriginalUrl(string imgUrl)
        {
            imgUrl = imgUrl.Replace("https://i.pixiv.cat", "https://i.pximg.net");
            imgUrl = imgUrl.Replace("https://i.pixiv.re", "https://i.pximg.net");
            return imgUrl;
        }

        private string getProxyUrl(string imgUrl)
        {
            imgUrl = imgUrl.Replace("https://i.pximg.net", BotConfig.GeneralConfig.PixivProxy);
            imgUrl = imgUrl.Replace("https://i.pixiv.cat", BotConfig.GeneralConfig.PixivProxy);
            return imgUrl;
        }

        public FileInfo downImg(LoliconDataV2 loliconData)
        {
            try
            {
                string fullFileName = $"{loliconData.pid}.jpg";
                string fullImageSavePath = Path.Combine(FilePath.getDownImgSavePath(), fullFileName);
                string imgUrl = loliconData.urls.original;
                if (BotConfig.GeneralConfig.DownWithProxy)
                {
                    imgUrl = getProxyUrl(imgUrl);
                    return HttpHelper.downImg(imgUrl, fullImageSavePath);
                }
                else
                {
                    imgUrl = getOriginalUrl(imgUrl);
                    string imgReferer = HttpUrl.getPixivArtworksReferer(loliconData.pid.ToString());
                    return HttpHelper.downImg(imgUrl, fullImageSavePath, imgReferer, BotConfig.WebsiteConfig.Pixiv.Cookie);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "LoliconBusiness.downImg下载图片失败");
                return null;
            }
        }

        



    }
}
