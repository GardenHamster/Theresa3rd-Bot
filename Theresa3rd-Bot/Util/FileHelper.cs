using System;
using System.IO;

namespace Theresa3rd_Bot.Util
{
    public static class FileHelper
    {
        public static void clearHistoryImg()
        {
            clearDownloadImg();
        }

        public static void clearDownloadImg()
        {
            try
            {
                string path = FilePath.getDownImgSavePath();
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                FileInfo[] fileInfoArr = directoryInfo.GetFiles();
                foreach (FileInfo fileInfo in fileInfoArr) deleteFile(fileInfo);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }

        public static void deleteFile(FileInfo fileInfo)
        {
            try { fileInfo.Delete(); } catch (Exception ex) { LogHelper.LogError(ex); }
        }

    }
}
