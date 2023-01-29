using System;
using System.Timers;
using TheresaBot.Main.Common;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Type;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Timer
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
                if (BotConfig.SubscribeConfig.PixivUser.ScanMode == PixivScanMode.ScanSubscribe)
                {
                    report = new PixivHandler().HandleUserSubscribeAsync().Result;
                }
                else
                {
                    report = new PixivHandler().HandleFollowSubscribeAsync().Result;
                }
                LogHelper.Info($"pixiv画师作品扫描完毕，扫描画师{report.ScanUser}个，失败{report.ErrorUser}个; 扫描作品{report.ScanWork}个，失败{report.ErrorWork}个;");
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




    }
}
