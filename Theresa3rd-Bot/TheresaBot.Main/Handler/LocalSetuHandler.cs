using TheresaBot.Main.Business;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.LocalSetu;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Handler
{
    public class LocalSetuHandler : SetuHandler
    {
        private LocalSetuBusiness localSetuBusiness;

        public LocalSetuHandler()
        {
            localSetuBusiness = new LocalSetuBusiness();
        }

        public async Task sendTimingSetuAsync(TimingSetuTimer timingSetuTimer, long groupId)
        {
            string localPath = timingSetuTimer.LocalPath;
            if (string.IsNullOrWhiteSpace(localPath)) throw new Exception("未配置LocalPath");
            List<LocalSetuInfo> setuInfos = localSetuBusiness.loadRandom(localPath, timingSetuTimer.Quantity, timingSetuTimer.FromOneDir);
            if (setuInfos is null || setuInfos.Count == 0) throw new Exception("未能在LocalPath中读取任何涩图");
            string tags = timingSetuTimer.FromOneDir ? setuInfos[0].DirInfo.Name : "";
            await sendTimingSetuMessage(session, timingSetuTimer, tags, groupId);
            await Task.Delay(2000);
            foreach (LocalSetuInfo setuInfo in setuInfos)
            {
                await sendSetuInfoAsync(session, timingSetuTimer, setuInfo, groupId);
                await Task.Delay(1000);
            }
        }

        private async Task sendSetuInfoAsync(TimingSetuTimer timingSetuTimer, LocalSetuInfo setuInfo, long groupId)
        {
            try
            {
                List<IChatMessage> workMsgs = new List<IChatMessage>();
                string template = getSetuInfo(setuInfo, timingSetuTimer.LocalTemplate);
                if (string.IsNullOrWhiteSpace(template) == false) workMsgs.Add(new PlainMessage(template));
                List<FileInfo> setuFiles = new List<FileInfo>() { setuInfo.FileInfo };
                await session.SendGroupSetuAsync(workMsgs, setuFiles, groupId, true);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "定时涩图发送失败");
                ReportHelper.SendError(ex, "定时涩图发送失败");
            }
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
