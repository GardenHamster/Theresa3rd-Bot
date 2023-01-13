using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Model.LocalSetu;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class LocalSetuHandler : BaseHandler
    {
        private LocalSetuBusiness localSetuBusiness;

        public LocalSetuHandler()
        {
            localSetuBusiness = new LocalSetuBusiness();
        }

        public async Task sendTimingSetu(IMiraiHttpSession session, TimingSetuTimer timingSetuTimer, long groupId)
        {
            string localPath = timingSetuTimer.LocalPath;
            if (string.IsNullOrWhiteSpace(localPath)) throw new Exception("未配置LocalPath");
            List<LocalSetuInfo> setuInfos = localSetuBusiness.loadRandom(localPath, timingSetuTimer.Quantity, timingSetuTimer.FromOneDir);
            if (setuInfos == null || setuInfos.Count == 0) throw new Exception("未能在LocalPath中读取任何涩图");
            string tags = timingSetuTimer.FromOneDir ? setuInfos[0].DirInfo.Name : "";
            await sendTimingSetuMessage(session, timingSetuTimer, tags, groupId);
            await Task.Delay(2000);
            foreach (LocalSetuInfo setuInfo in setuInfos)
            {
                await sendSetuInfoAsync(session, timingSetuTimer, setuInfo, groupId);
                await Task.Delay(1000);
            }
        }

        private async Task sendSetuInfoAsync(IMiraiHttpSession session, TimingSetuTimer timingSetuTimer, LocalSetuInfo setuInfo, long groupId)
        {
            try
            {
                List<IChatMessage> chainList = new List<IChatMessage>();
                string template = getSetuInfo(setuInfo, timingSetuTimer.LocalTemplate);
                if (string.IsNullOrWhiteSpace(template) == false) chainList.Add(new PlainMessage(template));
                chainList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, setuInfo.FileInfo.FullName));
                await session.SendGroupMessageAsync(groupId, chainList.ToArray());
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        private string getSetuInfo(LocalSetuInfo setuInfo, string template)
        {
            if (string.IsNullOrWhiteSpace(template)) return string.Empty;
            template = template.Replace("{FileName}", setuInfo.FileInfo.Name);
            template = template.Replace("{FilePath}", $"{setuInfo.DirInfo.Name}/${setuInfo.FileInfo.Name}");
            template = template.Replace("{SizeMB}", MathHelper.getMbWithByte(setuInfo.FileInfo.Length).ToString());
            return template;
        }





    }
}
