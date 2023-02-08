using System.Timers;
using TheresaBot.Main.Common;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Timers
{
    public static class PixivUserTimer
    {
        private static BaseSession Session;
        private static BaseReporter Reporter;
        private static System.Timers.Timer SystemTimer;

        public static void init(BaseSession session, BaseReporter reporter)
        {
            Session = session;
            Reporter = reporter;
            SystemTimer = new System.Timers.Timer();
            SystemTimer.Interval = BotConfig.SubscribeConfig.PixivUser.ScanInterval * 1000;
            SystemTimer.AutoReset = true;
            SystemTimer.Elapsed += new ElapsedEventHandler(HandlerMethod);
            SystemTimer.Enabled = true;
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
                PixivUserScanReport report;
                LogHelper.Info($"开始扫描pixiv画师最新作品...");
                if (BotConfig.SubscribeConfig.PixivUser.ScanMode == PixivScanType.ScanSubscribe)
                {
                    report = new PixivHandler(Session, Reporter).HandleUserSubscribeAsync().Result;
                }
                else
                {
                    report = new PixivHandler(Session, Reporter).HandleFollowSubscribeAsync().Result;
                }
                LogHelper.Info($"pixiv画师扫描完毕，扫描画师/扫描作品/失败画师/失败作品={report.ScanUser}/{report.ScanWork}/{report.ErrorUser}/{report.ErrorWork}");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "PixivUserTimer异常");
                Reporter.SendError(ex, "PixivUserTimer异常");
            }
            finally
            {
                SystemTimer.Enabled = true;
            }
        }




    }
}
