using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
{
    public static class PixivTagTimer
    {
        private static System.Timers.Timer timer;

        public static void init()
        {
            timer = new System.Timers.Timer();
            timer.Interval = BotConfig.SubscribeConfig.PixivTag.ScanInterval * 1000;
            timer.AutoReset = true;
            timer.Elapsed += new ElapsedEventHandler(HandlerMethod);
            timer.Enabled = true;
        }

        private static void HandlerMethod(object source, ElapsedEventArgs e)
        {
            try
            {
                timer.Enabled = false;
                if (BusinessHelper.IsPixivCookieAvailable() == false)
                {
                    LogHelper.Info("Pixiv Cookie过期或不可用，已停止扫描pixiv标签最新作品，请更新Cookie...");
                    return;
                }
                LogHelper.Info("开始扫描pixiv标签最新作品...");
                SubscribeMethodAsync(new PixivBusiness()).Wait();
                LogHelper.Info("pixiv标签作品扫描完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "PixivTagTimer.HandlerMethod方法异常");
            }
            finally
            {
                timer.Enabled = true;
            }
        }

        private static async Task SubscribeMethodAsync(PixivBusiness pixivBusiness)
        {
            int maxScan = BotConfig.SubscribeConfig.PixivTag.MaxScan;
            SubscribeType subscribeType = SubscribeType.P站标签;
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType];
            if (subscribeTaskList == null || subscribeTaskList.Count == 0) return;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    DateTime startTime = DateTime.Now;
                    List<PixivSubscribe> pixivSubscribeList = await pixivBusiness.getPixivTagSubscribeAsync(subscribeTask, maxScan);
                    if (pixivSubscribeList == null || pixivSubscribeList.Count == 0) continue;
                    await sendGroupSubscribeAsync(pixivBusiness, subscribeTask, pixivSubscribeList, startTime);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"推送pixiv标签[{subscribeTask.SubscribeCode}]订阅时异常");
                }
                finally
                {
                    await Task.Delay(2000);
                }
            }
        }

        private static async Task sendGroupSubscribeAsync(PixivBusiness pixivBusiness, SubscribeTask subscribeTask, List<PixivSubscribe> pixivSubscribeList, DateTime startTime)
        {
            foreach (PixivSubscribe pixivSubscribe in pixivSubscribeList)
            {
                List<long> groupIds = subscribeTask.GroupIdList;
                PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfoDto.body;
                if (pixivWorkInfo == null || pixivWorkInfo.IsImproper()) continue;
                if (groupIds == null || groupIds.Count == 0) continue;

                string tagName = subscribeTask.SubscribeName;
                bool isR18Img = pixivWorkInfo.isR18();
                bool isDownImg = groupIds.IsDownImg(isR18Img);
                string remindTemplate = BotConfig.SubscribeConfig.PixivTag.Template;
                string pixivTemplate = BotConfig.GeneralConfig.PixivTemplate;
                FileInfo fileInfo = isDownImg ? await pixivBusiness.downImgAsync(pixivWorkInfo) : null;

                foreach (long groupId in groupIds)
                {
                    try
                    {
                        if (isR18Img && groupId.IsShowR18Setu() == false) continue;
                        bool isShowImg = groupId.IsShowSetuImg(isR18Img);

                        List<IChatMessage> chailList = new List<IChatMessage>();
                        if (string.IsNullOrWhiteSpace(remindTemplate))
                        {
                            chailList.Add(new PlainMessage($"pixiv标签[{tagName}]发布了新作品："));
                        }
                        else
                        {
                            chailList.Add(new PlainMessage(pixivBusiness.getTagPushRemindMsg(remindTemplate, tagName)));
                        }

                        if (string.IsNullOrWhiteSpace(pixivTemplate))
                        {
                            chailList.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivWorkInfo, fileInfo, startTime)));
                        }
                        else
                        {
                            chailList.Add(new PlainMessage(pixivBusiness.getWorkInfo(pixivWorkInfo, fileInfo, startTime, pixivTemplate)));
                        }

                        if (isShowImg && fileInfo != null)
                        {
                            chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                        }
                        else if (isShowImg && fileInfo == null)
                        {
                            chailList.AddRange(await MiraiHelper.Session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, UploadTarget.Group));
                        }

                        await MiraiHelper.Session.SendGroupMessageAsync(groupId, chailList.ToArray());
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, "pixiv标签订阅消息发送失败");
                    }
                    finally
                    {
                        await Task.Delay(1000);
                    }
                }
            }
        }



    }
}
