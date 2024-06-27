using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Infos;

namespace TheresaBot.Core.Common
{
    public static class FilePath
    {
        public const string ImgHttpPath = "/img";
        public const string DownDirName = "BotDownload";
        public const string TempDirName = "Temp";
        public const string UploadDirName = "Upload";
        public const string MiyousheDirName = "Miyoushe";
        public const string PixivWorkDirName = "PixivWork";
        public const string PixivPreviewDirName = "PixivPreview";
        public const string WordCloudDirName = "WordCloud";
        public const string BotImgDirName = "BotImg";
        public const string FontDirName = "Font";

        /// <summary>
        /// 获取下载图片的保存目录
        /// </summary>
        /// <returns></returns>
        public static string GetDownloadDirectory()
        {
            string configPath = BotConfig.GeneralConfig.TempPath;
            if (string.IsNullOrWhiteSpace(configPath)) configPath = AppContext.BaseDirectory;
            string dirPath = Path.Combine(configPath, DownDirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取表情文件存放目录
        /// </summary>
        /// <returns></returns>
        public static string GetFontDirectory()
        {
            string dirPath = Path.Combine(AppContext.BaseDirectory, FontDirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取表情文件存放目录
        /// </summary>
        /// <returns></returns>
        public static string GetBotImgDirectory()
        {
            string dirPath = Path.Combine(AppContext.BaseDirectory, BotImgDirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取表情文件存放目录
        /// </summary>
        /// <returns></returns>
        public static string GetFaceDirectory()
        {
            string dirPath = Path.Combine(GetBotImgDirectory(), "face");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取词云蒙版图片存放目录
        /// </summary>
        /// <returns></returns>
        public static string GetMaskDirectory()
        {
            string dirPath = Path.Combine(GetBotImgDirectory(), "mask");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取临时文件存放目录
        /// </summary>
        /// <returns></returns>
        public static string GetTempDirectory()
        {
            string downFilePath = GetDownloadDirectory();
            string dirPath = Path.Combine(downFilePath, TempDirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取临时上传目录
        /// </summary>
        /// <returns></returns>
        public static string GetTempUploadDirectory()
        {
            string downFilePath = GetDownloadDirectory();
            string dirPath = Path.Combine(downFilePath, TempDirName, UploadDirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取临时解压目录
        /// </summary>
        /// <param name="pixivId"></param>
        /// <returns></returns>
        public static string GetTempUnzipDirectory(string dirName = "")
        {
            if (string.IsNullOrWhiteSpace(dirName)) dirName = StringHelper.RandomUUID16();
            string dirPath = Path.Combine(GetTempDirectory(), dirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取词云图片存放目录
        /// </summary>
        /// <returns></returns>
        public static string GetWordCloudDirectory()
        {
            string downFilePath = GetDownloadDirectory();
            string dirPath = Path.Combine(downFilePath, WordCloudDirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取米游社存放路径
        /// </summary>
        /// <returns></returns>
        public static string GetMiyousheDirectory()
        {
            string downFilePath = GetDownloadDirectory();
            string dirPath = Path.Combine(downFilePath, MiyousheDirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取pixiv图片存放路径
        /// </summary>
        /// <param name="pixivId"></param>
        /// <returns></returns>
        public static string GetPixivTempDirectory(int pixivId)
        {
            string downFilePath = GetDownloadDirectory();
            string pixivDir = GetPixivDirectory(pixivId);
            string dirPath = Path.Combine(downFilePath, PixivWorkDirName, pixivDir);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取pixiv日榜大图存放路径
        /// </summary>
        /// <returns></returns>
        public static string GetPixivPreviewDirectory()
        {
            string downFilePath = GetDownloadDirectory();
            string dirPath = Path.Combine(downFilePath, PixivPreviewDirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取pixiv图片存放路径
        /// </summary>
        /// <param name="pixivId"></param>
        /// <returns></returns>
        public static string GetPixivUploadDirectory(int pixivId)
        {
            string tempUpload = GetTempUploadDirectory();
            string pixivDir = GetPixivDirectory(pixivId);
            string dirPath = Path.Combine(tempUpload, pixivDir);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取pixiv图片收藏路径
        /// </summary>
        /// <returns></returns>
        public static string GetPixivCollectionDirectory(int pixivId)
        {
            string localSavePath = BotConfig.PixivCollectionConfig.LocalSavePath;
            if (string.IsNullOrWhiteSpace(localSavePath))
            {
                localSavePath = Path.Combine(DownDirName, "PixivCollection");
            }
            string pixivDir = GetPixivDirectory(pixivId);
            string dirPath = Path.Combine(localSavePath, pixivDir);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// 获取图片下载错误后的替代图片
        /// </summary>
        /// <returns></returns>
        public static FileInfo GetErrorImgPath()
        {
            string fullImgPath = BotConfig.GeneralConfig.ErrorImgPath;
            if (File.Exists(fullImgPath) == false) return null;
            return new FileInfo(fullImgPath);
        }

        /// <summary>
        /// 获取米游社图片存放路径
        /// </summary>
        /// <returns></returns>
        public static string GetMiyousheImgSavePath(string imgUrl)
        {
            var fileInfo = new HttpFileInfo(imgUrl);
            string extension = fileInfo.FileExtension;
            if (string.IsNullOrEmpty(extension)) extension = "jpg";
            string fullFileName = StringHelper.RandomUUID16() + "." + extension;
            return Path.Combine(GetMiyousheDirectory(), fullFileName);
        }

        /// <summary>
        /// 获取词云图片存放路径
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static string GetWordCloudImgSavePath(string fullFileName = "")
        {
            if (string.IsNullOrEmpty(fullFileName))
            {
                fullFileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff") + ".jpg";
            }
            return Path.Combine(GetWordCloudDirectory(), fullFileName);
        }

        /// <summary>
        /// 获取临时图片存放路径
        /// </summary>
        /// <returns></returns>
        public static string GetTempImgSavePath(string fullFileName = "")
        {
            if (string.IsNullOrEmpty(fullFileName))
            {
                fullFileName = StringHelper.RandomUUID16() + ".jpg";
            }
            return Path.Combine(GetTempDirectory(), fullFileName);
        }

        /// <summary>
        /// 获取临时文件存放路径
        /// </summary>
        /// <returns></returns>
        public static string GetTempFileSavePath(string fullFileName)
        {
            return Path.Combine(GetTempDirectory(), fullFileName);
        }

        /// <summary>
        /// 获取Pixiv图片文件夹
        /// </summary>
        /// <param name="pixivId"></param>
        /// <returns></returns>
        public static string GetPixivDirectory(int pixivId)
        {
            //105866144-->105/10586/105866144
            //73532572 -->073/07353/73532572
            //3532572  -->003/00353/3532572
            var length = pixivId.ToString().Length;
            var lengthL = length < 6 ? 0 : length - 6;
            var lengthM = length < 4 ? 0 : length - 4;
            var dirName1 = pixivId.ToString().Substring(0, lengthL).PadLeft(3, '0');
            var dirName2 = pixivId.ToString().Substring(0, lengthM).PadLeft(5, '0');
            var dirName3 = pixivId.ToString();
            return Path.Combine(dirName1, dirName2, dirName3);
        }






    }
}
