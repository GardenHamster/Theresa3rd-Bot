using TheresaBot.Main.Business;
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
            string localPath = timingSetuTimer.LocalPath;
            if (string.IsNullOrWhiteSpace(localPath)) throw new Exception("未配置LocalPath");
            List<LocalSetuInfo> dataList = localSetuBusiness.loadRandom(localPath, timingSetuTimer.Quantity, timingSetuTimer.FromOneDir);
            if (dataList is null || dataList.Count == 0) throw new Exception("未能在LocalPath中读取任何涩图");
            string tags = timingSetuTimer.FromOneDir ? dataList[0].DirInfo.Name : "";
            List<SetuContent> setuContents = getSetuContent(timingSetuTimer, dataList);
            await sendTimingSetuMessageAsync(timingSetuTimer, tags, groupId);
            await Task.Delay(2000);
            await Session.SendGroupSetuAsync(setuContents, groupId, sendMerge);
        }

        private List<SetuContent> getSetuContent(TimingSetuTimer timingSetuTimer,List<LocalSetuInfo> datas)
        {
            List<SetuContent> setuContents = new List<SetuContent>();
            foreach (var data in datas) setuContents.Add(getSetuContent(timingSetuTimer, data));
            return setuContents;
        }

        private SetuContent getSetuContent(TimingSetuTimer timingSetuTimer, LocalSetuInfo data)
        {
            string setuInfo = getSetuInfo(data, timingSetuTimer.LocalTemplate);
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
