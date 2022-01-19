using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Common
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
            string path = Path.Combine(BotConfig.SetuConfig.ImgSavePath, "download");
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            return path;
        }




    }
}
