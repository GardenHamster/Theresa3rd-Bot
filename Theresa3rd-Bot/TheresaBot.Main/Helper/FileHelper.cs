using TheresaBot.Main.Common;

namespace TheresaBot.Main.Helper
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
                string path = FilePath.GetDownFileSavePath();
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                FileInfo[] fileInfoArr = directoryInfo.GetFiles();
                foreach (FileInfo fileInfo in fileInfoArr) deleteFile(fileInfo);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        public static void deleteFile(string fullFilePath)
        {
            try
            {
                File.Delete(fullFilePath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        public static void deleteFile(FileInfo fileInfo)
        {
            try
            {
                fileInfo.Delete();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        public static void deleteDirectory(string directoryPath)
        {
            try
            {
                Directory.Delete(directoryPath, true);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        /// <summary>
        /// 正则搜索一个目录下的文件
        /// </summary>
        /// <param name="downTask"></param>
        /// <returns></returns>
        public static FileInfo[] searchFiles(string dirPath, string searchPattern)
        {
            if (Directory.Exists(dirPath) == false) return null;
            DirectoryInfo searchFolder = new DirectoryInfo(dirPath);
            return searchFolder.GetFiles(searchPattern, SearchOption.AllDirectories);
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
