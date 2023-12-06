using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.LocalSetu;

namespace TheresaBot.Main.Services
{
    internal class LocalSetuService : SetuService
    {
        public List<LocalSetuInfo> loadRandomDir(string localPath, int count, bool fromOneDir = false)
        {
            List<LocalSetuInfo> setuList = new List<LocalSetuInfo>();
            DirectoryInfo localDir = new DirectoryInfo(localPath);
            DirectoryInfo[] directoryInfos = localDir.GetDirectories();
            if (directoryInfos.Length == 0) throw new Exception($"localPath路径下不存在子文件夹，请在子文件夹下存放图片");
            int singleDirIndex = new Random().Next(0, directoryInfos.Length);
            for (int i = 0; i < count; i++)
            {
                int dirIndex = fromOneDir ? singleDirIndex : new Random().Next(0, directoryInfos.Length);
                DirectoryInfo randomDir = directoryInfos[dirIndex];
                FileInfo[] fileInfos = randomDir.GetFiles();
                if (fileInfos.Length == 0) continue;
                int randomFileIndex = new Random().Next(0, fileInfos.Length);
                FileInfo randomFile = fileInfos[randomFileIndex];
                setuList.Add(new LocalSetuInfo(randomFile, randomDir));
            }
            return setuList;
        }

        public List<LocalSetuInfo> loadTargetDir(string localPath, string dirName, int count)
        {
            List<LocalSetuInfo> setuList = new List<LocalSetuInfo>();
            DirectoryInfo localDir = new DirectoryInfo(localPath);
            DirectoryInfo[] directoryInfos = localDir.GetDirectories();
            if (directoryInfos is null || directoryInfos.Length == 0) return setuList;
            DirectoryInfo directoryInfo = directoryInfos.Where(o => o.Name.ToLower() == dirName.ToLower()).FirstOrDefault();
            if (directoryInfo is null) return setuList;
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            if (fileInfos.Length == 0) return setuList;
            for (int i = 0; i < count; i++)
            {
                int randomFileIndex = new Random().Next(0, fileInfos.Length);
                FileInfo randomFile = fileInfos[randomFileIndex];
                setuList.Add(new LocalSetuInfo(randomFile, directoryInfo));
            }
            return setuList;
        }


        public string getSetuInfo(LocalSetuInfo setuInfo, long todayLeft, string template = "")
        {
            template = template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template)) return getDefaultSetuInfo(setuInfo);
            template = template.Replace("{FileName}", setuInfo.FileInfo.Name);
            template = template.Replace("{SizeMB}", MathHelper.ByteToMB(setuInfo.FileInfo.Length).ToString());
            template = template.Replace("{TodayLeft}", todayLeft.ToString());
            template = template.Replace("{MemberCD}", BotConfig.SetuConfig.MemberCD.ToString());
            template = template.Replace("{RevokeInterval}", BotConfig.SetuConfig.RevokeInterval.ToString());
            return template;
        }

        public string getDefaultSetuInfo(LocalSetuInfo setuInfo)
        {
            return $"文件名：{setuInfo.FileInfo.Name}，大小：{MathHelper.ByteToMB(setuInfo.FileInfo.Length)}MB";
        }

    }
}
