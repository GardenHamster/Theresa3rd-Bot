namespace TheresaBot.Core.Helper
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
                if (!File.Exists(fullFilePath)) return;
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
                if (fileInfo is null) return;
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
                if (!Directory.Exists(directoryPath)) return;
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
        public static FileInfo[] SearchFiles(string dirPath, string searchPattern = "*.*")
        {
            if (Directory.Exists(dirPath) == false) return new FileInfo[0];
            DirectoryInfo searchFolder = new DirectoryInfo(dirPath);
            return searchFolder.GetFiles(searchPattern, SearchOption.AllDirectories);
        }

        /// <summary>
        /// 返回一个file在path中的相对路径
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetRelativePath(this FileInfo file, string path)
        {
            Uri uri1 = new Uri(path);
            Uri uri2 = new Uri(file.FullName);
            return uri1.MakeRelativeUri(uri2).ToString().Replace(@"\", "/");
        }

        /// <summary>
        /// 返回多个file在path中的相对路径
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetRelativePath(this FileInfo[] files, string path)
        {
            return files.Select(o => o.GetRelativePath(path)).ToArray();
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
                DirectoryInfo randomDirectory = directoryInfos.Length == 0 ? sourceDirectory : directoryInfos[RandomHelper.RandomBetween(0, directoryInfos.Length - 1)];
                FileInfo[] fileInfos = randomDirectory.GetFiles();
                int randomFileIndex = RandomHelper.RandomBetween(0, fileInfos.Length - 1);
                FileInfo randomFile = fileInfos[randomFileIndex];
                if (stList.Where(m => m.FullName == randomFile.FullName).ToList().Count > 0) continue;
                stList.Add(randomFile);
            }
            return stList;
        }

        public static void CopyToDirectory(this DirectoryInfo copyDir, string targetDirPath)
        {
            var files = copyDir.GetFiles();
            if (Directory.Exists(targetDirPath) == false)
            {
                Directory.CreateDirectory(targetDirPath);
            }
            foreach (FileInfo file in files)
            {
                var destFileName = Path.Combine(targetDirPath, file.Name);
                file.CopyTo(destFileName, true);
            }
        }

    }
}
