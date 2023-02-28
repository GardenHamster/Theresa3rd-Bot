namespace TheresaBot.Main.Common
{
    public static class FilePath
    {
        private const string DownDir = "BotDownload";

        /// <summary>
        /// 获取下载图的保存的绝对路径
        /// </summary>
        /// <returns></returns>
        public static string GetDownFileSavePath()
        {
            string configPath = BotConfig.GeneralConfig.DownloadPath;
            if (string.IsNullOrWhiteSpace(configPath)) configPath = AppContext.BaseDirectory;
            string savePath = Path.Combine(configPath, DownDir);
            if (Directory.Exists(savePath) == false) Directory.CreateDirectory(savePath);
            return savePath;
        }

        /// <summary>
        /// 获取图片下载错误后的替代图片
        /// </summary>
        /// <returns></returns>
        public static FileInfo GetDownErrorImg()
        {
            string fullImgPath = BotConfig.GeneralConfig.DownErrorImgPath;
            if (File.Exists(fullImgPath) == false) return null;
            return new FileInfo(fullImgPath);
        }

    }
}
