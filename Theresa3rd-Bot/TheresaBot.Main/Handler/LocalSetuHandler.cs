using TheresaBot.Main.Business;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.LocalSetu;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    public class LocalSetuHandler : SetuHandler
    {
        private LocalSetuBusiness localSetuBusiness;

        public LocalSetuHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            localSetuBusiness = new LocalSetuBusiness();
        }

        public async Task sendTimingSetuAsync(TimingSetuTimer timingSetuTimer, long groupId)
        {
            bool sendMerge = timingSetuTimer.SendMerge;
            bool fromOneDir = BotConfig.TimingSetuConfig.FromOneDir;
            string localPath = BotConfig.TimingSetuConfig.LocalPath;
            if (string.IsNullOrWhiteSpace(localPath)) throw new Exception($"未配置LocalPath");
            if (Directory.Exists(localPath) == false) throw new Exception($"本地涩图路径：{localPath}不存在");
            List<LocalSetuInfo> dataList = localSetuBusiness.loadRandom(localPath, timingSetuTimer.Quantity, fromOneDir);
            if (dataList is null || dataList.Count == 0) throw new Exception("未能在LocalPath中读取任何涩图");
            string tags = fromOneDir ? dataList[0].DirInfo.Name : "";
            List<SetuContent> setuContents = getSetuContent(dataList);
            await sendTimingSetuMessageAsync(timingSetuTimer, tags, groupId);
            await Task.Delay(2000);
            await Session.SendGroupSetuAsync(setuContents, groupId, sendMerge);
        }

        private List<SetuContent> getSetuContent(List<LocalSetuInfo> datas)
        {
            List<SetuContent> setuContents = new List<SetuContent>();
            foreach (var data in datas) setuContents.Add(getSetuContent(data));
            return setuContents;
        }

        private SetuContent getSetuContent(LocalSetuInfo data)
        {
            string setuInfo = getSetuInfo(data, BotConfig.TimingSetuConfig.LocalTemplate);
            List<FileInfo> setuFiles = new List<FileInfo>() { data.FileInfo };
            return new SetuContent(setuInfo, setuFiles);
        }

        private string getSetuInfo(LocalSetuInfo setuInfo, string template)
        {
            if (string.IsNullOrWhiteSpace(template)) return string.Empty;
            template = template.Replace("{FileName}", setuInfo.FileInfo.Name);
            template = template.Replace("{FilePath}", $"{setuInfo.DirInfo.Name}/{setuInfo.FileInfo.Name}");
            template = template.Replace("{SizeMB}", MathHelper.getMbWithByte(setuInfo.FileInfo.Length).ToString());
            return template;
        }





    }
}
