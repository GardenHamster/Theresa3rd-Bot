using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
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
            timer.Elapsed += new ElapsedEventHandler(HandlerMethod);
            timer.Enabled = true;
        }

        private static void HandlerMethod(object source, ElapsedEventArgs e)
        {
            try
            {
                timer.Enabled = false;
                LogHelper.Info("开始扫描pixiv画师最新作品...");
                SubscribeMethodAsync(new PixivBusiness()).Wait();
                LogHelper.Info("pixiv画师作品扫描完毕...");
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

        private static async Task SubscribeMethodAsync(PixivBusiness pixivBusiness)
        {
            SubscribeType subscribeType = SubscribeType.P站画师;
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType];
            if (subscribeTaskList == null || subscribeTaskList.Count == 0) return;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    DateTime startTime = DateTime.Now;
                    List<PixivSubscribe> pixivSubscribeList = await pixivBusiness.getPixivUserSubscribeAsync(subscribeTask.SubscribeCode, subscribeTask.SubscribeId);
                    if (pixivSubscribeList == null || pixivSubscribeList.Count == 0) continue;
                    await sendGroupSubscribeAsync(pixivBusiness, subscribeTask, pixivSubscribeList, startTime);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"获取pixiv用户[{subscribeTask.SubscribeCode}]订阅失败");
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
        }

        private static async Task sendGroupSubscribeAsync(PixivBusiness pixivBusiness, SubscribeTask subscribeTask, List<PixivSubscribe> pixivSubscribeList, DateTime startTime)
        {
            foreach (PixivSubscribe pixivSubscribe in pixivSubscribeList)
            {
                string template = BotConfig.SubscribeConfig.PixivUser.Template;

                foreach (long groupId in subscribeTask.GroupIdList)
                {
                    try
                    {
                        if (pixivSubscribe.PixivWorkInfoDto.body.IsImproper()) continue;
                        if (pixivSubscribe.PixivWorkInfoDto.body.isR18() && groupId.IsShowR18() == false) continue;

                        FileInfo fileInfo = await pixivBusiness.downImgAsync(pixivSubscribe.PixivWorkInfoDto);

                        List<IChatMessage> chailList = new List<IChatMessage>();
                        if (string.IsNullOrWhiteSpace(template))
                        {
                            chailList.Add(new PlainMessage($"pixiv画师[{subscribeTask.SubscribeName}]发布了新作品："));
                            chailList.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivSubscribe.PixivWorkInfoDto.body, fileInfo, startTime)));
                        }
                        else
                        {
                            chailList.Add(new PlainMessage(pixivBusiness.getWorkInfo(pixivSubscribe.PixivWorkInfoDto.body, fileInfo, startTime, template)));
                        }

                        if (fileInfo == null)
                        {
                            chailList.AddRange(await MiraiHelper.Session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg));
                        }
                        else if (pixivSubscribe.PixivWorkInfoDto.body.isR18() == false)
                        {
                            chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                        }
                        else if (pixivSubscribe.PixivWorkInfoDto.body.isR18() && groupId.IsShowR18Img())
                        {
                            chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                        }

                        await MiraiHelper.Session.SendGroupMessageAsync(groupId, chailList.ToArray());
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, "pixiv画师订阅消息发送失败");
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
