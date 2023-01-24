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
                LogHelper.Info($"开始扫描pixiv标签最新作品...");
                PixivTagScanReport report = SubscribeMethodAsync(new PixivBusiness()).Result;
                LogHelper.Info($"pixiv标签作品扫描完毕，扫描标签{report.ScanTag}个，错误{report.ErrorTag}个; 扫描作品{report.ScanWork}个，错误{report.ErrorWork}个;");
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

        private static async Task<PixivTagScanReport> SubscribeMethodAsync(PixivBusiness pixivBusiness)
        {
            int maxScan = BotConfig.SubscribeConfig.PixivTag.MaxScan;
            SubscribeType subscribeType = SubscribeType.P站标签;
            PixivTagScanReport scanReport = new PixivTagScanReport();
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return scanReport;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType];
            if (subscribeTaskList == null || subscribeTaskList.Count == 0) return scanReport;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    scanReport.ScanTag++;
                    List<PixivSubscribe> pixivSubscribeList = await pixivBusiness.getPixivTagSubscribeAsync(subscribeTask, scanReport, maxScan);
                    if (pixivSubscribeList == null || pixivSubscribeList.Count == 0) continue;
                    await sendGroupSubscribeAsync(pixivBusiness, subscribeTask, pixivSubscribeList);
                }
                catch (Exception ex)
                {
                    scanReport.ErrorTag++;
                    LogHelper.Error(ex, $"扫描pixiv标签[{subscribeTask.SubscribeCode}]订阅失败");
                }
                finally
                {
                    await Task.Delay(2000);
                }
            }
            return scanReport;
        }

        private static async Task sendGroupSubscribeAsync(PixivBusiness pixivBusiness, SubscribeTask subscribeTask, List<PixivSubscribe> pixivSubscribeList)
        {
            foreach (PixivSubscribe pixivSubscribe in pixivSubscribeList)
            {
                DateTime startTime = DateTime.Now;
                List<long> groupIds = subscribeTask.GroupIdList;
                PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfo;
                if (pixivWorkInfo == null || pixivWorkInfo.IsImproper || pixivWorkInfo.hasBanTag() != null) continue;
                if (groupIds == null || groupIds.Count == 0) continue;
                if (pixivWorkInfo.IsAI && groupIds.IsShowAISetu() == false) continue;

                bool isAISetu = pixivWorkInfo.IsAI;
                bool isR18Img = pixivWorkInfo.IsR18;
                bool isDownImg = groupIds.IsDownSetuImg(isR18Img);
                string tagName = subscribeTask.SubscribeName;
                string remindTemplate = BotConfig.SubscribeConfig.PixivTag.Template;
                string pixivTemplate = BotConfig.PixivConfig.Template;
                List<FileInfo> setuFiles = isDownImg ? await pixivBusiness.downPixivImgsAsync(pixivWorkInfo) : null;

                List<IChatMessage> workMsgs = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(remindTemplate))
                {
                    workMsgs.Add(new PlainMessage($"pixiv标签[{tagName}]发布了新作品："));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getTagPushRemindMsg(remindTemplate, tagName)));
                }

                if (string.IsNullOrWhiteSpace(pixivTemplate))
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivWorkInfo, startTime)));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getWorkInfo(pixivWorkInfo, startTime, pixivTemplate)));
                }

                foreach (long groupId in groupIds)
                {
                    try
                    {
                        if (isR18Img && groupId.IsShowR18Setu() == false) continue;
                        if (isAISetu && groupId.IsShowAISetu() == false) continue;
                        bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                        await MiraiHelper.Session.SendGroupSetuAsync(workMsgs, setuFiles, groupId, isShowImg);
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
