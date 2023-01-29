using System;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Handler;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
{
    public static class MysUserTimer
    {
        private static System.Timers.Timer timer;

        public static void init()
        {
            timer = new System.Timers.Timer();
            timer.Interval = BotConfig.SubscribeConfig.Mihoyo.ScanInterval * 1000;
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandlerMethod);
            timer.Enabled = true;
        }

        private static void HandlerMethod(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                timer.Enabled = false;
                new MYSHandler().HandleUserSubscribeAsync().Wait();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "MysUserTimer异常");
            }
            finally
            {
                timer.Enabled = true;
            }
        }

        



    }
}
