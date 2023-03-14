using TheresaBot.Main.Common;

namespace TheresaBot.Main.Helper
{
    public static class FileHelper
    {
        public static string GetFileName(this FileInfo fileInfo)
        {
            string fullFileName = fileInfo.Name;
            var splitArr = fullFileName.Split('.');
            return splitArr.Length > 0 ? splitArr[0] : string.Empty;
        }

        public static bool IsFilesExists(this List<string> fullFilePaths)
        {
            if (fullFilePaths is null || fullFilePaths.Count == 0) return false;
            foreach (string fullFilePath in fullFilePaths)
            {
                if (File.Exists(fullFilePath) == false) return false;
            }
            return true;
        }

        public static void DeleteFile(string fullFilePath)
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

        public static void DeleteFile(FileInfo fileInfo)
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

        public static void DeleteDirectory(string directoryPath)
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
        public static FileInfo[] SearchFiles(string dirPath, string searchPattern)
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
        public static List<FileInfo> GetRandomFileInList(string dirPath, int count)
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
