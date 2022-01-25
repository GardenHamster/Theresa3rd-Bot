using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
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
            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandlerMethod);
            timer.Enabled = true;
        }

        private static void HandlerMethod(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                timer.Enabled = false;
                LogHelper.Info("开始扫描pixiv标签最新作品...");
                SubscribeMethodAsync(source,e).Wait();
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

        private static async Task SubscribeMethodAsync(object source, System.Timers.ElapsedEventArgs e)
        {
            PixivBusiness pixivBusiness = new PixivBusiness();
            SubscribeType subscribeType = SubscribeType.P站标签;
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType];
            if (subscribeTaskList == null || subscribeTaskList.Count == 0) return;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    DateTime startTime = DateTime.Now;
                    List<PixivSubscribe> pixivSubscribeList = pixivBusiness.getPixivTagSubscribeWork(subscribeTask.SubscribeInfo.SubscribeCode, subscribeTask.SubscribeInfo.SubscribeId);
                    if (pixivSubscribeList == null || pixivSubscribeList.Count == 0) continue;
                    await sendGroupSubscribeAsync(subscribeTask, pixivSubscribeList, startTime);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex,$"获取pixiv标签[{subscribeTask.SubscribeInfo.SubscribeCode}]订阅失败");
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
        }

        private static async Task sendGroupSubscribeAsync(SubscribeTask subscribeTask, List<PixivSubscribe> pixivSubscribeList, DateTime startTime)
        {
            PixivBusiness pixivBusiness = new PixivBusiness();
            foreach (PixivSubscribe pixivSubscribe in pixivSubscribeList)
            {
                FileInfo fileInfo = pixivBusiness.downImg(pixivSubscribe.PixivWorkInfoDto);
                foreach (long groupId in subscribeTask.GroupIdList)
                {
                    string tagName = subscribeTask.SubscribeInfo.SubscribeName;
                    string template = BotConfig.SubscribeConfig.PixivTag.Template;
                    List<IChatMessage> chailList = new List<IChatMessage>();
                    if (string.IsNullOrWhiteSpace(template))
                    {
                        chailList.Add(new PlainMessage($"pixiv标签[{tagName}]发布了新作品："));
                        chailList.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivSubscribe.PixivWorkInfoDto.body, fileInfo, startTime)));
                    }
                    else
                    {
                        chailList.Add(new PlainMessage(pixivBusiness.getWorkInfoWithTag(pixivSubscribe.PixivWorkInfoDto.body, fileInfo, startTime, tagName, template)));
                    }

                    if (fileInfo == null)
                    {
                        chailList.AddRange(MiraiHelper.Session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg).Result);
                    }
                    else
                    {
                        chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                    }
                    await MiraiHelper.Session.SendGroupMessageAsync(groupId, chailList.ToArray());
                }
            }
        }



    }
}
