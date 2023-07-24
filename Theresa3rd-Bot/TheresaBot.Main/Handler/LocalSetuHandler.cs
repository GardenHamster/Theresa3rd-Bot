using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.LocalSetu;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    internal class LocalSetuHandler : SetuHandler
    {
        private LocalSetuBusiness localSetuBusiness;

        public LocalSetuHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            localSetuBusiness = new LocalSetuBusiness();
        }

        public async Task localSearchAsync(GroupCommand command)
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
                    dataList = localSetuBusiness.loadRandomDir(localPath, 1, true);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;
                    dataList = localSetuBusiness.loadTargetDir(localPath, tagName, 1);
                }

                if (dataList.Count == 0)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.SetuConfig.NotFoundMsg, "没有获取到任何本地涩图~");
                    return;
                }

                string template = BotConfig.SetuConfig.Local.Template;
                long todayLeftCount = GetSetuLeftToday(command.GroupId, command.MemberId);

                LocalSetuInfo setuInfo = dataList.First();
                List<BaseContent> workMsgs = new List<BaseContent>();
                workMsgs.Add(new PlainContent(localSetuBusiness.getSetuInfo(setuInfo, todayLeftCount, template)));
                List<FileInfo> setuFiles = new() { setuInfo.FileInfo };

                SetuContent setuContent = new SetuContent(workMsgs, setuFiles);
                var results = await command.ReplyGroupSetuAsync(setuContent, BotConfig.SetuConfig.RevokeInterval, BotConfig.PixivConfig.SendImgBehind, true);
                var msgIds = results.Select(o => o.MsgId).ToArray();
                var recordTask = recordBusiness.AddPixivRecord(setuContent, Session.PlatformType, msgIds, command.GroupId);
                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = command.SendTempSetuAsync(setuContent, BotConfig.PixivConfig.SendImgBehind);
                }

                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "localSearchAsync异常");
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, "localSearchAsync异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        public async Task sendTimingSetuAsync(TimingSetuTimer timingSetuTimer, long groupId)
        {
            int margeEachPage = 5;
            bool sendMerge = timingSetuTimer.SendMerge;
            bool fromOneDir = BotConfig.TimingSetuConfig.FromOneDir;
            string localPath = BotConfig.TimingSetuConfig.LocalPath;
            if (string.IsNullOrWhiteSpace(localPath)) throw new Exception($"未配置LocalPath");
            if (Directory.Exists(localPath) == false) throw new Exception($"本地涩图路径：{localPath}不存在");
            List<LocalSetuInfo> dataList = localSetuBusiness.loadRandomDir(localPath, timingSetuTimer.Quantity, fromOneDir);
            if (dataList is null || dataList.Count == 0) throw new Exception("未能在LocalPath中读取任何涩图");
            string tags = fromOneDir ? dataList[0].DirInfo.Name : "";
            List<SetuContent> setuContents = getSetuContent(dataList);
            await sendTimingSetuMessageAsync(timingSetuTimer, tags, groupId);
            await Task.Delay(2000);
            await SendGroupSetuAsync(setuContents, groupId, sendMerge, margeEachPage);
        }

        private List<SetuContent> getSetuContent(List<LocalSetuInfo> datas)
        {
            List<SetuContent> setuContents = new List<SetuContent>();
            foreach (var data in datas) setuContents.Add(getSetuContent(data));
            return setuContents;
        }

        private SetuContent getSetuContent(LocalSetuInfo data)
        {
            string setuInfo = localSetuBusiness.getDefaultSetuInfo(data);
            List<FileInfo> setuFiles = new List<FileInfo>() { data.FileInfo };
            return new SetuContent(setuInfo, setuFiles);
        }







    }
}
