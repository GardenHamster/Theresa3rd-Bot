using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.LocalSetu;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    internal class LocalSetuHandler : SetuHandler
    {
        private LocalSetuService localSetuService;

        public LocalSetuHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            localSetuService = new LocalSetuService();
        }

        public async Task LocalSearchAsync(GroupCommand command)
        {
            try
            {
                List<LocalSetuInfo> dataList;
                string tagName = command.KeyWord;

                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中

                string localPath = BotConfig.SetuConfig.Local.LocalPath;
                if (string.IsNullOrWhiteSpace(localPath)) throw new Exception($"未配置LocalPath");
                if (Directory.Exists(localPath) == false) throw new Exception($"本地涩图路径：{localPath}不存在");
                if (string.IsNullOrEmpty(tagName))
                {
                    dataList = localSetuService.LoadRandomDir(localPath, 1, true);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;
                    dataList = localSetuService.LoadTargetDir(localPath, tagName, 1);
                }

                if (dataList.Count == 0)
                {
                    await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.SetuConfig.NotFoundMsg, "没有获取到任何本地涩图~");
                    return;
                }

                string template = BotConfig.SetuConfig.Local.Template;
                long todayLeftCount = GetSetuLeftToday(command.GroupId, command.MemberId);

                LocalSetuInfo setuInfo = dataList.First();
                List<BaseContent> workMsgs = new List<BaseContent>();
                workMsgs.Add(new PlainContent(localSetuService.GetSetuInfo(setuInfo, todayLeftCount, template)));
                List<FileInfo> setuFiles = new() { setuInfo.FileInfo };

                SetuContent setuContent = new SetuContent(workMsgs, setuFiles);
                var results = await command.ReplyGroupSetuAsync(setuContent, BotConfig.SetuConfig.RevokeInterval, BotConfig.PixivConfig.SendImgBehind);
                var msgIds = results.Select(o => o.MessageId).ToArray();
                var recordTask = recordService.InsertPixivRecord(setuContent, Session.PlatformType, msgIds, command.GroupId);
                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = command.SendPrivateSetuAsync(setuContent, BotConfig.PixivConfig.SendImgBehind);
                }
                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "本地涩图功能异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        public async Task SendTimingSetuAsync(TimingSetuTimer timingSetuTimer, long groupId)
        {
            int margeEachPage = 5;
            bool sendMerge = timingSetuTimer.SendMerge;
            bool fromOneDir = BotConfig.TimingSetuConfig.FromOneDir;
            string localPath = BotConfig.TimingSetuConfig.LocalPath;
            if (string.IsNullOrWhiteSpace(localPath)) throw new Exception($"未配置LocalPath");
            if (Directory.Exists(localPath) == false) throw new Exception($"本地涩图路径：{localPath}不存在");
            List<LocalSetuInfo> dataList = localSetuService.LoadRandomDir(localPath, timingSetuTimer.Quantity, fromOneDir);
            if (dataList is null || dataList.Count == 0) throw new Exception("未能在LocalPath中读取任何涩图");
            string tags = fromOneDir ? dataList[0].DirInfo.Name : "";
            List<SetuContent> setuContents = GetSetuContent(dataList);
            await sendTimingSetuMessageAsync(timingSetuTimer, tags, groupId);
            await Task.Delay(2000);
            await SendGroupSetuAsync(setuContents, groupId, sendMerge, margeEachPage);
        }

        private List<SetuContent> GetSetuContent(List<LocalSetuInfo> datas)
        {
            List<SetuContent> setuContents = new List<SetuContent>();
            foreach (var data in datas) setuContents.Add(GetSetuContent(data));
            return setuContents;
        }

        private SetuContent GetSetuContent(LocalSetuInfo data)
        {
            string setuInfo = localSetuService.GetDefaultSetuInfo(data);
            List<FileInfo> setuFiles = new List<FileInfo>() { data.FileInfo };
            return new SetuContent(setuInfo, setuFiles);
        }







    }
}
