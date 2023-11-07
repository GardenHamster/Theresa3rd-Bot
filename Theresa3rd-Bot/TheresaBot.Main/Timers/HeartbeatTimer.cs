using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Timers
{
    internal static class HeartbeatTimer
    {
        private static readonly int HeartbeatSeconds = 5;
        private static System.Timers.Timer SystemTimer;

        public static void Init()
        {
            if (SystemTimer is not null) return;
            SystemTimer = new System.Timers.Timer();
            SystemTimer.Interval = HeartbeatSeconds * 1000;
            SystemTimer.AutoReset = true;
            SystemTimer.Elapsed += new System.Timers.ElapsedEventHandler(HandlerMethod);
            SystemTimer.Enabled = true;
        }

        private static void HandlerMethod(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                RunningDatas.AddRunningSeconds(HeartbeatSeconds);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "CountingTimer异常");
            }
        }


    }
}
