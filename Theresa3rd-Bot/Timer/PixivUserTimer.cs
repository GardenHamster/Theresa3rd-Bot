using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
{
    public static class PixivUserTimer
    {
        private static System.Timers.Timer timer;

        public static void init()
        {
            timer = new System.Timers.Timer();
            timer.Interval = BotConfig.SubscribeConfig.PixivUser.ScanInterval * 1000;
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandlerMethod);
            timer.Enabled = true;
        }

        private static void HandlerMethod(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                timer.Enabled = false;
                SubscribeMethodAsync(source,e).Wait();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "PixivUserTimer.HandlerMethod方法异常");
            }
            finally
            {
                timer.Enabled = true;
            }
        }

        private static async Task SubscribeMethodAsync(object source, System.Timers.ElapsedEventArgs e)
        {
            PixivBusiness pixivBusiness = new PixivBusiness();
            SubscribeType subscribeType = SubscribeType.P站画师;
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType];
            if (subscribeTaskList == null || subscribeTaskList.Count == 0) return;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    List<PixivSubscribe> pixivSubscribeList = pixivBusiness.getPixivUserSubscribeWork(subscribeTask.SubscribeInfo.SubscribeCode, subscribeTask.SubscribeInfo.SubscribeId);
                    if (pixivSubscribeList == null || pixivSubscribeList.Count == 0) continue;
                    await sendGroupSubscribeAsync(subscribeTask, pixivSubscribeList);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex,$"获取pixiv用户[{subscribeTask.SubscribeInfo.SubscribeCode}]订阅失败");
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
        }

        private static async Task sendGroupSubscribeAsync(SubscribeTask subscribeTask, List<PixivSubscribe> pixivSubscribeList)
        {
            foreach (PixivSubscribe pixivSubscribe in pixivSubscribeList)
            {
                int shelfLife = BotConfig.SubscribeConfig.PixivUser.ShelfLife;
                if (pixivSubscribe.PixivWorkInfoDto.body.isR18()) continue;
                if (shelfLife > 0 && pixivSubscribe.PixivWorkInfoDto.body.createDate < DateTime.Now.AddSeconds(-1 * shelfLife)) continue;
                foreach (long groupId in subscribeTask.GroupIdList)
                {
                    List<IChatMessage> chailList = new List<IChatMessage>();
                    chailList.Add(new PlainMessage($"pixiv画师[{subscribeTask.SubscribeInfo.SubscribeName}]发布了新作品："));
                    chailList.Add(new PlainMessage(pixivSubscribe.WorkInfo));
                    if (pixivSubscribe.WorkFileInfo == null)
                    {
                        chailList.AddRange(MiraiHelper.Session.SplitToChainAsync(BotConfig.SubscribeConfig.PixivUser.DownErrorImg).Result);
                    }
                    else
                    {
                        chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, pixivSubscribe.WorkFileInfo.FullName));
                    }
                    await MiraiHelper.Session.SendGroupMessageAsync(groupId, chailList.ToArray());
                }
            }
        }



    }
}
