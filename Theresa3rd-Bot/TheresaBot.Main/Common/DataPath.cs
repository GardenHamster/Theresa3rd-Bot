namespace TheresaBot.Main.Common
{
    public static class DataPath
    {
        public const string DataDirName = "Data";

        /// <summary>
        /// 获取Data文件夹目录
        /// </summary>
        /// <returns></returns>
        public static string GetDataDirectory()
        {
            string dirPath = Path.Combine(AppContext.BaseDirectory, DataDirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// Count.xml目录
        /// </summary>
        /// <returns></returns>
        public static string GetCountPath()
        {
            string dirPath = GetDataDirectory();
            return Path.Combine(dirPath, "Count.xml");
        }


    }
}
