using TheresaBot.Main.Common;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Timers
{
    internal static class MysUserScanTimer
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
            SystemTimer.Interval = BotConfig.SubscribeConfig.Miyoushe.ScanInterval * 1000;
            SystemTimer.AutoReset = true;
            SystemTimer.Elapsed += new System.Timers.ElapsedEventHandler(HandlerMethod);
            SystemTimer.Enabled = true;
        }

        public static void Destroy()
        {
            if (SystemTimer is null) return;
            SystemTimer.Enabled = false;
            SystemTimer.Stop();
            SystemTimer.Close();
            SystemTimer.Dispose();
            SystemTimer = null;
        }

        private static void HandlerMethod(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                SystemTimer.Enabled = false;
                new MiyousheHandler(Session, Reporter).HandleSubscribeAsync().Wait();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "MysUserTimer异常");
                Reporter.SendError(ex, "MysUserTimer异常").Wait();
            }
            finally
            {
                SystemTimer.Enabled = true;
            }
        }





    }
}
