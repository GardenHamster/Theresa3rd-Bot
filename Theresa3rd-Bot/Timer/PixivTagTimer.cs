using System;
using System.Timers;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Handler;
using Theresa3rd_Bot.Model.Pixiv;
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
                PixivTagScanReport report = new PixivHandler().HandleTagSubscribeAsync().Result;
                LogHelper.Info($"pixiv标签作品扫描完毕，扫描标签{report.ScanTag}个，失败{report.ErrorTag}个; 扫描作品{report.ScanWork}个，失败{report.ErrorWork}个;");
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

        



    }
}
