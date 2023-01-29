using System;
using System.Collections.Generic;
using System.IO;
using TheresaBot.Main.Model.LocalSetu;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Business
{
    public class LocalSetuBusiness : SetuBusiness
    {
        public List<LocalSetuInfo> loadRandom(string localPath, int count, bool fromOneDir = false)
        {
            List<FileInfo> FileList = new List<FileInfo>();
            List<LocalSetuInfo> setuList = new List<LocalSetuInfo>();
            if (!Directory.Exists(localPath)) throw new Exception("LocalPath不存在");
            DirectoryInfo localDir = new DirectoryInfo(localPath);
            DirectoryInfo[] directoryInfos = localDir.GetDirectories();
            if (directoryInfos is null || directoryInfos.Length == 0) throw new Exception("LocalPath中不存在子文件夹");
            int randomDirIndex = new Random().Next(0, directoryInfos.Length);
            for (int i = 0; i < count; i++)
            {
                randomDirIndex = fromOneDir ? randomDirIndex : new Random().Next(0, directoryInfos.Length);
                DirectoryInfo randomDir = directoryInfos[randomDirIndex];
                FileInfo[] fileInfos = randomDir.GetFiles();
                if (fileInfos.Length == 0) continue;
                int randomFileIndex = new Random().Next(0, fileInfos.Length);
                FileInfo randomFile = fileInfos[randomFileIndex];
                if (FileList.Contains(randomFile)) continue;
                setuList.Add(new LocalSetuInfo(randomFile, randomDir));
            }
            return setuList;
        }

    }
}
