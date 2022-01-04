using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Theresa3rd_Bot.Common;

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
                LogHelper.Error(ex);
            }
        }

        public static void deleteFile(FileInfo fileInfo)
        {
            try { fileInfo.Delete(); } catch (Exception ex) { LogHelper.Error(ex); }
        }

        /// <summary>
        /// 从文件夹中获取指定数量的涩图
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<FileInfo> getRandomFileInList(string dirPath, int count)
        {
            List<FileInfo> stList = new List<FileInfo>();
            DirectoryInfo sourceDirectory = new DirectoryInfo(dirPath);
            DirectoryInfo[] directoryInfos = sourceDirectory.GetDirectories();
            while (stList.Count < count)
            {
                DirectoryInfo randomDirectory = directoryInfos.Length == 0 ? sourceDirectory : directoryInfos[RandomHelper.getRandomBetween(0, directoryInfos.Length - 1)];
                FileInfo[] fileInfos = randomDirectory.GetFiles();
                int randomFileIndex = RandomHelper.getRandomBetween(0, fileInfos.Length - 1);
                FileInfo randomFile = fileInfos[randomFileIndex];
                if (stList.Where(m => m.FullName == randomFile.FullName).ToList().Count > 0) continue;
                stList.Add(randomFile);
            }
            return stList;
        }

    }
}
