using Mirai.CSharp.Models.ChatMessages;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
{
    public class ClearJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            ClearDownload();
            await Task.Delay(1000);
        }

        /// <summary>
        /// 清理下载目录
        /// </summary>
        public void ClearDownload()
        {
            try
            {
                string path = FilePath.getDownImgSavePath();
                if (Directory.Exists(path) == false) return;
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                directoryInfo.Delete(true);
                LogHelper.Info("图片下载目录清理完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "图片下载目录清理失败");
            }
        }


    }
}
