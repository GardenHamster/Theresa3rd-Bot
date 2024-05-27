using System.Timers;
using TheresaBot.Core.Common;
using TheresaBot.Core.Datas;
using TheresaBot.Core.Handler;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Timers
{
    internal static class PixivUserScanTimer
    {
        private static BaseSession Session;
        private static BaseReporter Reporter;
        private static System.Timers.Timer SystemTimer;

        public static void Init(BaseSession session, BaseReporter reporter)
        {
            Destroy();
            Session = session;
            Reporter = reporter;
            SystemTimer = new System.Timers.Timer();
            SystemTimer.Interval = BotConfig.SubscribeConfig.PixivUser.ScanInterval * 1000;
            SystemTimer.AutoReset = true;
            SystemTimer.Elapsed += new ElapsedEventHandler(HandlerMethod);
            SystemTimer.Enabled = true;
        }

        public static bool Destroy()
        {
            if (SystemTimer is null) return false;
            if (SystemTimer.Enabled == false) return false;
            SystemTimer.Enabled = false;
            SystemTimer.Stop();
            SystemTimer.Close();
            SystemTimer.Dispose();
            SystemTimer = null;
            return true;
        }

        private static void HandlerMethod(object source, ElapsedEventArgs e)
        {
            try
            {
                SystemTimer.Enabled = false;
                if (BusinessHelper.IsPixivCookieAvailable() == false)
                {
                    LogHelper.Info("Pixiv Cookie过期或不可用，已停止扫描pixiv画师最新作品，请更新Cookie...");
                    return;
                }
                if (BotConfig.SubscribeConfig.PixivUser.ScanMode == PixivUserScanType.ScanFollow)
                {
                    LogHelper.Info($"开始扫描关注画师最新作品...");
                    var report = new PixivPushHandler(Session, Reporter).HandleFollowPushAsync().Result;
                    LogHelper.Info($"pixiv画师扫描完毕，扫描作品/失败作品={report.ScanWork}/{report.ErrorWork}");
                    CountDatas.AddPixivScanTimes(report);
                    CountDatas.AddPixivPushTimes(report);
                }
                else
                {
                    LogHelper.Info($"开始扫描订阅画师最新作品...");
                    var report = new PixivPushHandler(Session, Reporter).HandleUserPushAsync().Result;
                    LogHelper.Info($"pixiv画师扫描完毕，扫描画师/扫描作品/失败画师/失败作品={report.ScanUser}/{report.ScanWork}/{report.ErrorUser}/{report.ErrorWork}");
                    CountDatas.AddPixivScanTimes(report);
                    CountDatas.AddPixivPushTimes(report);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "PixivUserTimer异常");
                Reporter.SendError(ex, "PixivUserTimer异常").Wait();
            }
            finally
            {
                SystemTimer.Enabled = true;
            }
        }




    }
}
