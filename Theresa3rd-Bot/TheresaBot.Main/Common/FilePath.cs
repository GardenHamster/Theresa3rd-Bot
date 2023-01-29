using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TheresaBot.Main.Common
{
    public static class FilePath
    {
        /// <summary>
        /// 获取下载图的保存的绝对路径
        /// </summary>
        /// <param name="CQApi"></param>
        /// <returns></returns>
        public static string getDownImgSavePath()
        {
            string path = BotConfig.GeneralConfig.DownloadPath;
            if (string.IsNullOrWhiteSpace(path)) path = Path.Combine(AppContext.BaseDirectory, "download");
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            return path;
        }




    }
}
