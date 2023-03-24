using Quartz;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Timers
{
    [DisallowConcurrentExecution]
    internal class DownloadClearJob : IJob
    {
        private BaseReporter reporter;

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.MergedJobDataMap;
                reporter = (BaseReporter)dataMap["BaseReporter"];
                BaseSession session = (BaseSession)dataMap["BaseSession"];
                ClearDownload();
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "DownloadClearJob异常");
                reporter.SendError(ex, "DownloadClearJob异常");
            }
        }

        /// <summary>
        /// 清理下载目录
        /// </summary>
        private void ClearDownload()
        {
            try
            {
                lock (TimerManager.ClearLock)
                {
                    string path = FilePath.GetDownFileSavePath();
                    if (Directory.Exists(path) == false) return;
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    directoryInfo.Delete(true);
                    LogHelper.Info("图片下载目录清理完毕...");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "图片下载目录清理失败");
                reporter.SendError(ex, "图片下载目录清理失败");
            }
        }

    }
}
