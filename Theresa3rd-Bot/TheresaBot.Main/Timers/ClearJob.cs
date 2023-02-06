using Quartz;
using System.Runtime.InteropServices;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Timers
{
    [DisallowConcurrentExecution]
    public class ClearJob : IJob
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
                ClearUploadTemp();
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ClearJob异常");
                reporter.SendError(ex, "ClearJob异常");
            }
        }

        /// <summary>
        /// 清理下载目录
        /// </summary>
        private void ClearDownload()
        {
            try
            {
                string path = FilePath.GetDownFileSavePath();
                if (Directory.Exists(path) == false) return;
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                directoryInfo.Delete(true);
                LogHelper.Info("图片下载目录清理完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "图片下载目录清理失败");
                reporter.SendError(ex, "图片下载目录清理失败");
            }
        }

        /// <summary>
        /// 清理上传临时文件
        /// </summary>
        private void ClearUploadTemp()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    ClearWindowsUploadTemp();
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    ClearLinuxUploadTemp();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "上传临时文件清理失败");
                reporter.SendError(ex, "上传临时文件清理失败");
            }
        }

        private void ClearWindowsUploadTemp()
        {
            string dirPath = System.IO.Path.GetTempPath();
            FileInfo[] fileList = FileHelper.searchFiles(dirPath, "file-upload*.tmp");
            if (fileList is null || fileList.Length == 0) return;
            foreach (var item in fileList) FileHelper.deleteFile(item);
            LogHelper.Info($"上传临时文件清理完毕，共计清理 {fileList.Length} 个临时文件...");
        }

        private void ClearLinuxUploadTemp()
        {

        }




    }
}
