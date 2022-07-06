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
                if (BusinessHelper.IsPixivCookieAvailable() == false)
                {
                    LogHelper.Info("Pixiv Cookie过期或不可用，已停止扫描pixiv画师最新作品，请更新Cookie...");
                    return;
                }
                LogHelper.Info("开始扫描pixiv画师最新作品...");
                PixivBusiness pixivBusiness = new PixivBusiness();
                if (BotConfig.SubscribeConfig.PixivUser.ScanMode == PixivScanMode.ScanSubscribe)
                {
                    SendWithSubscribe(pixivBusiness).Wait();
                }
                else
                {
                    SendWithFollow(pixivBusiness).Wait();
                }
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

        private static async Task SendWithSubscribe(PixivBusiness pixivBusiness)
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
                    List<PixivSubscribe> pixivSubscribeList = await pixivBusiness.getPixivUserSubscribeAsync(subscribeTask);
                    if (pixivSubscribeList == null || pixivSubscribeList.Count == 0) continue;
                    await sendGroupSubscribeAsync(pixivBusiness, pixivSubscribeList, subscribeTask.GroupIdList, startTime);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"推送pixiv用户[{subscribeTask.SubscribeCode}]订阅时异常");
                }
                finally
                {
                    await Task.Delay(2000);
                }
            }
        }


        private static async Task SendWithFollow(PixivBusiness pixivBusiness)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                List<PixivSubscribe> pixivFollowLatestList = await pixivBusiness.getPixivFollowLatestAsync();
                if (pixivFollowLatestList == null || pixivFollowLatestList.Count == 0) return;
                await sendGroupSubscribeAsync(pixivBusiness, pixivFollowLatestList, BotConfig.PermissionsConfig.SubscribeGroups, startTime);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"推送pixiv关注用户最新作品时异常");
            }
        }


        private static async Task sendGroupSubscribeAsync(PixivBusiness pixivBusiness, List<PixivSubscribe> pixivSubscribeList, List<long> groupIds, DateTime startTime)
        {
            foreach (PixivSubscribe pixivSubscribe in pixivSubscribeList)
            {
                if (pixivSubscribe.PixivWorkInfoDto.body.IsImproper()) continue;
                if (groupIds == null || groupIds.Count == 0) continue;

                string template = BotConfig.SubscribeConfig.PixivUser.Template;
                FileInfo fileInfo = await pixivBusiness.downImgAsync(pixivSubscribe.PixivWorkInfoDto);

                foreach (long groupId in groupIds)
                {
                    try
                    {
                        if (pixivSubscribe.PixivWorkInfoDto.body.isR18() && groupId.IsShowR18Setu() == false) continue;

                        List<IChatMessage> chailList = new List<IChatMessage>();
                        if (string.IsNullOrWhiteSpace(template))
                        {
                            chailList.Add(new PlainMessage($"pixiv画师[{pixivSubscribe.PixivWorkInfoDto.body.userName}]发布了新作品："));
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
                        else if (pixivSubscribe.PixivWorkInfoDto.body.isR18() && groupId.IsShowR18SetuImg())
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
