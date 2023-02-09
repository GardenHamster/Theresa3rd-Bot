using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.LocalSetu;

namespace TheresaBot.Main.Business
{
    public class LocalSetuBusiness : SetuBusiness
    {
        public List<LocalSetuInfo> loadRandom(string localPath, int count, bool fromOneDir = false)
        {
            List<LocalSetuInfo> setuList = new List<LocalSetuInfo>();
            DirectoryInfo localDir = new DirectoryInfo(localPath);
            DirectoryInfo[] directoryInfos = localDir.GetDirectories();
            int randomDirIndex = new Random().Next(0, directoryInfos.Length);
            for (int i = 0; i < count; i++)
            {
                randomDirIndex = fromOneDir ? randomDirIndex : new Random().Next(0, directoryInfos.Length);
                DirectoryInfo randomDir = directoryInfos[randomDirIndex];
                FileInfo[] fileInfos = randomDir.GetFiles();
                if (fileInfos.Length == 0) continue;
                int randomFileIndex = new Random().Next(0, fileInfos.Length);
                FileInfo randomFile = fileInfos[randomFileIndex];
                setuList.Add(new LocalSetuInfo(randomFile, randomDir));
            }
            return setuList;
        }

        public List<LocalSetuInfo> loadInDir(string localPath, string dirName, int count)
        {
            List<LocalSetuInfo> setuList = new List<LocalSetuInfo>();
            DirectoryInfo localDir = new DirectoryInfo(localPath);
            DirectoryInfo[] directoryInfos = localDir.GetDirectories();
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
            if (string.IsNullOrWhiteSpace(template)) return getDefaultSetuInfo(setuInfo);
            template = template.Replace("{FileName}", setuInfo.FileInfo.Name);
            template = template.Replace("{SizeMB}", MathHelper.getMbWithByte(setuInfo.FileInfo.Length).ToString());
            template = template.Replace("{TodayLeft}", todayLeft.ToString());
            template = template.Replace("{MemberCD}", BotConfig.SetuConfig.MemberCD.ToString());
            template = template.Replace("{RevokeInterval}", BotConfig.SetuConfig.RevokeInterval.ToString());
            return template;
        }

        public string getDefaultSetuInfo(LocalSetuInfo setuInfo)
        {
            return $"文件名：{setuInfo.FileInfo.Name}，大小：{MathHelper.getMbWithByte(setuInfo.FileInfo.Length)}MB";
        }

    }
}
