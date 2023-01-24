using Mirai.CSharp.Framework.Models.General;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using MySqlX.XDevAPI;
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

                PixivUserScanReport report = null;
                LogHelper.Info($"开始扫描pixiv画师最新作品...");
                PixivBusiness pixivBusiness = new PixivBusiness();
                if (BotConfig.SubscribeConfig.PixivUser.ScanMode == PixivScanMode.ScanSubscribe)
                {
                    report = HandleWithSubscribe(pixivBusiness).Result;
                }
                else
                {
                    report = HandleWithFollow(pixivBusiness).Result;
                }
                LogHelper.Info($"pixiv画师作品扫描完毕，扫描画师{report.ScanUser}个，错误{report.ErrorUser}个; 扫描作品{report.ScanWork}个，错误{report.ErrorWork}个;");
            }
            catch (Exception ex)
            {
                string message = $"PixivUserTimer.HandlerMethod方法异常";
                LogHelper.Error(ex, message);
                ReportHelper.SendError(ex, message);
            }
            finally
            {
                timer.Enabled = true;
            }
        }

        private static async Task<PixivUserScanReport> HandleWithSubscribe(PixivBusiness pixivBusiness)
        {
            SubscribeType subscribeType = SubscribeType.P站画师;
            PixivUserScanReport scanReport = new PixivUserScanReport();
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return scanReport;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType];
            if (subscribeTaskList == null || subscribeTaskList.Count == 0) return scanReport;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    DateTime startTime = DateTime.Now;
                    scanReport.ScanUser++;
                    List<PixivSubscribe> pixivSubscribeList = await pixivBusiness.getPixivUserSubscribeAsync(subscribeTask, scanReport);
                    if (pixivSubscribeList == null || pixivSubscribeList.Count == 0) continue;
                    await sendGroupSubscribeAsync(pixivBusiness, pixivSubscribeList, subscribeTask.GroupIdList, startTime);
                }
                catch (Exception ex)
                {
                    scanReport.ErrorWork++;
                    string message = $"扫描pixiv用户[{subscribeTask.SubscribeCode}]订阅失败";
                    LogHelper.Error(ex, message);
                    ReportHelper.SendError(ex, message);
                }
                finally
                {
                    await Task.Delay(2000);
                }
            }
            return scanReport;
        }


        private static async Task<PixivUserScanReport> HandleWithFollow(PixivBusiness pixivBusiness)
        {
            DateTime startTime = DateTime.Now;
            PixivUserScanReport scanReport = new PixivUserScanReport();

            try
            {
                List<PixivSubscribe> pixivFollowLatestList = await pixivBusiness.getPixivFollowLatestAsync(scanReport);
                if (pixivFollowLatestList == null || pixivFollowLatestList.Count == 0) return scanReport;
                await sendGroupSubscribeAsync(pixivBusiness, pixivFollowLatestList, BotConfig.PermissionsConfig.SubscribeGroups, startTime);
            }
            catch (Exception ex)
            {
                string message = $"扫描pixiv关注用户最新作品失败";
                LogHelper.Error(ex, message);
                ReportHelper.SendError(ex, message);
            }
            return scanReport;
        }


        private static async Task sendGroupSubscribeAsync(PixivBusiness pixivBusiness, List<PixivSubscribe> pixivSubscribeList, List<long> groupIds, DateTime startTime)
        {
            foreach (PixivSubscribe pixivSubscribe in pixivSubscribeList)
            {
                PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfo;
                if (pixivWorkInfo == null || pixivWorkInfo.IsImproper || pixivWorkInfo.hasBanTag() != null) continue;
                if (groupIds == null || groupIds.Count == 0) continue;
                if (pixivWorkInfo.IsAI && groupIds.IsShowAISetu() == false) continue;

                bool isAISetu = pixivWorkInfo.IsAI;
                bool isR18Img = pixivWorkInfo.IsR18;
                bool isDownImg = groupIds.IsDownSetuImg(isR18Img);
                string remindTemplate = BotConfig.SubscribeConfig.PixivUser.Template;
                string pixivTemplate = BotConfig.PixivConfig.Template;
                List<FileInfo> setuFiles = isDownImg ? await pixivBusiness.downPixivImgsAsync(pixivWorkInfo) : null;

                List<IChatMessage> workMsgs = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(remindTemplate))
                {
                    workMsgs.Add(new PlainMessage($"pixiv画师[{pixivWorkInfo.userName}]发布了新作品："));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getUserPushRemindMsg(remindTemplate, pixivWorkInfo.userName)));
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
