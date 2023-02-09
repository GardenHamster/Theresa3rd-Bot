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
    public class LocalSetuHandler : SetuHandler
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

                string localPath = BotConfig.TimingSetuConfig.LocalPath;
                if (string.IsNullOrWhiteSpace(localPath)) throw new Exception($"未配置LocalPath");
                if (Directory.Exists(localPath) == false) throw new Exception($"本地涩图路径：{localPath}不存在");
                
                if (await CheckSetuTagEnableAsync(command, tagName) == false) return;

                if (string.IsNullOrEmpty(tagName))
                {
                    dataList = localSetuBusiness.loadRandom(localPath, 1, true);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;
                    dataList = localSetuBusiness.loadInDir(localPath, tagName, 1);
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

                Task sendGroupTask = command.ReplyGroupSetuAndRevokeAsync(workMsgs, setuFiles, BotConfig.SetuConfig.RevokeInterval, true);
                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = command.SendTempSetuAsync(workMsgs, setuFiles);
                }

                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);
            }
            catch (Exception ex)
            {
                string errMsg = "localSetuSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.ErrorMsg, "获取涩图出错了，再试一次吧~");
                Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
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
            string setuInfo = localSetuBusiness.getDefaultSetuInfo(data);
            List<FileInfo> setuFiles = new List<FileInfo>() { data.FileInfo };
            return new SetuContent(setuInfo, setuFiles);
        }







    }
}
