using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.File;

namespace TheresaBot.Main.Common
{
    public static class FilePath
    {
        private const string DownDir = "BotDownload";
        private const string TempDir = "Temp";
        private const string MiyousheDir = "Miyoushe";
        private const string PixivWorkDir = "PixivWork";
        private const string PixivPreviewDir = "PixivPreview";

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

        /// <summary>
        /// 获取下载图片的保存路径
        /// </summary>
        /// <returns></returns>
        public static string GetDownFileSavePath()
        {
            string configPath = BotConfig.GeneralConfig.DownloadPath;
            if (string.IsNullOrWhiteSpace(configPath)) configPath = AppContext.BaseDirectory;
            string savePath = Path.Combine(configPath, DownDir);
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            return savePath;
        }

        /// <summary>
        /// 获取米游社图片存放路径
        /// </summary>
        /// <returns></returns>
        public static string GetFullMysImgSavePath(string imgUrl)
        {
            var fileInfo = new HttpFileInfo(imgUrl);
            string extension = fileInfo.FileExtension;
            if (string.IsNullOrEmpty(extension)) extension = "jpg";
            string fullFileName = StringHelper.get16UUID() + "." + extension;
            string savePath = GetMysImgSavePath();
            return Path.Combine(savePath, fullFileName);
        }

        /// <summary>
        /// 获取米游社图片存放路径
        /// </summary>
        /// <returns></returns>
        public static string GetFullTempImgSavePath(string extension = "jpg")
        {
            if (string.IsNullOrEmpty(extension)) extension = "jpg";
            string fullFileName = StringHelper.get16UUID() + "." + extension;
            string savePath = GetTempSavePath();
            return Path.Combine(savePath, fullFileName);
        }

        /// <summary>
        /// 获取pixiv图片存放路径
        /// </summary>
        /// <param name="pixivId"></param>
        /// <returns></returns>
        public static string GetPixivImgSavePath(int pixivId)
        {
            string downFilePath = GetDownFileSavePath();
            string pixivImgDir = GetPixivImgDir(pixivId);
            string savePath = Path.Combine(downFilePath, PixivWorkDir, pixivImgDir);
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            return savePath;
        }

        /// <summary>
        /// 获取pixiv日榜大图存放路径
        /// </summary>
        /// <param name="pixivId"></param>
        /// <returns></returns>
        public static string GetPixivPreviewSavePath()
        {
            string downFilePath = GetDownFileSavePath();
            string savePath = Path.Combine(downFilePath, PixivPreviewDir);
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            return savePath;
        }

        /// <summary>
        /// 获取米游社存放路径
        /// </summary>
        /// <param name="pixivId"></param>
        /// <returns></returns>
        public static string GetMysImgSavePath()
        {
            string downFilePath = GetDownFileSavePath();
            string savePath = Path.Combine(downFilePath, MiyousheDir);
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            return savePath;
        }

        /// <summary>
        /// 获取临时文件存放路径
        /// </summary>
        /// <param name="pixivId"></param>
        /// <returns></returns>
        public static string GetTempSavePath()
        {
            string downFilePath = GetDownFileSavePath();
            string savePath = Path.Combine(downFilePath, TempDir);
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            return savePath;
        }

        /// <summary>
        /// 获取pixiv图片存放文件夹名称
        /// </summary>
        /// <param name="pixivId"></param>
        /// <returns></returns>
        private static string GetPixivImgDir(int pixivId)
        {
            //105866144
            if (pixivId > 100000000) return $"{(pixivId / 100000) * 100000}";
            if (pixivId > 80000000) return $"{(pixivId / 5000000) * 5000000}";
            if (pixivId > 50000000) return $"{(pixivId / 10000000) * 10000000}";
            return "50000000";
        }


    }
}
