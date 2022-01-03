using System;
using System.IO;

namespace Theresa3rd_Bot.Util
{
    public static class FileHelper
    {
        public static void clearHistoryImg(CQApi CQApi)
        {
            clearDownloadImg(CQApi);
        }

        public static void clearDownloadImg(CQApi CQApi)
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
