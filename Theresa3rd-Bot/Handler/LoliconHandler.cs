using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Model.Lolicon;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class LoliconHandler : BaseHandler
    {
        private LoliconBusiness loliconBusiness;

        public LoliconHandler()
        {
            loliconBusiness = new LoliconBusiness();
        }

        public async Task sendGeneralLoliconImageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(groupId, memberId);//请求处理中

                bool isShowR18 = groupId.IsShowR18Setu();
                string tagStr = message.splitKeyWord(BotConfig.SetuConfig.Lolicon.Command) ?? "";
                if (await CheckSetuTagEnableAsync(session, args, tagStr) == false) return;
                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.ProcessingMsg) == false)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.ProcessingMsg, null);
                    await Task.Delay(1000);
                }

                LoliconResultV2 loliconResult = null;
                int r18Mode = groupId.IsShowR18Setu() ? 2 : 0;

                if (string.IsNullOrEmpty(tagStr))
                {
                    loliconResult = await loliconBusiness.getLoliconResultAsync(r18Mode);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(session, args) == false) return;
                    string[] tagArr = toLoliconTagArr(tagStr);
                    loliconResult = await loliconBusiness.getLoliconResultAsync(r18Mode, 1, tagArr);
                }

                if (loliconResult == null || loliconResult.data.Count == 0)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LoliconDataV2 loliconData = loliconResult.data.First();
                if (loliconData.IsImproper())
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 该作品含有R18G等内容，不显示相关内容"));
                    return;
                }

                string banTagStr = loliconData.hasBanTag();
                if (banTagStr != null)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 该作品含有被屏蔽的标签【{banTagStr}】，不显示相关内容"));
                    return;
                }

                if (loliconData.isR18() && isShowR18 == false)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 该作品为R-18作品，不显示相关内容，如需显示请在配置文件中修改权限"));
                    return;
                }

                long todayLeftCount = GetSetuLeftToday(groupId, memberId);
                bool isShowImg = groupId.IsShowSetuImg(loliconData.isR18());
                FileInfo fileInfo = isShowImg ? await loliconBusiness.downImgAsync(loliconData.pid.ToString(), loliconData.urls.original, false) : null;

                string template = BotConfig.SetuConfig.Lolicon.Template;
                List<IChatMessage> workMsgs = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(template))
                {
                    workMsgs.Add(new PlainMessage(loliconBusiness.getDefaultWorkInfo(loliconData, fileInfo, startDateTime)));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(loliconBusiness.getWorkInfo(loliconData, fileInfo, startDateTime, todayLeftCount, template)));
                }

                Task sendGroupTask = session.SendGroupSetuAndRevokeWithAtAsync(args, workMsgs, fileInfo, isShowImg);
                
                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = session.SendTempSetuAsync(args, workMsgs, fileInfo, isShowImg);
                }

                CoolingCache.SetMemberSetuCooling(groupId, memberId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "sendGeneralLoliconImageAsync异常");
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.ErrorMsg, " 获取图片出错了，再试一次吧~");
            }
            finally
            {
                CoolingCache.SetHandFinish(args.Sender.Group.Id, args.Sender.Id);//请求处理完成
            }
        }

        public async Task sendTimingSetu(IMiraiHttpSession session, TimingSetuTimer timingSetuTimer, long groupId)
        {
            int eachPage = 5;
            int r18Mode = groupId.IsShowR18Setu() ? 2 : 0;
            int count = timingSetuTimer.Quantity > 20 ? 20 : timingSetuTimer.Quantity;
            string tagStr = RandomHelper.getRandomItem(timingSetuTimer.Tags);
            string[] tagArr = string.IsNullOrWhiteSpace(tagStr) ? null : toLoliconTagArr(tagStr);
            await sendTimingSetuMessage(session, timingSetuTimer, tagStr, groupId);
            await Task.Delay(2000);
            while (count > 0)
            {
                int num = count >= eachPage ? eachPage : count;
                LoliconResultV2 loliconResult = await loliconBusiness.getLoliconResultAsync(r18Mode, num, tagArr);
                count -= num;
                if (loliconResult.data.Count == 0) continue;
                foreach (var setuInfo in loliconResult.data)
                {
                    await sendSetuInfoAsync(session, setuInfo, groupId);
                    await Task.Delay(1000);
                }
            }
        }

        private async Task sendSetuInfoAsync(IMiraiHttpSession session, LoliconDataV2 setuInfo, long groupId)
        {
            try
            {
                bool isR18Img = setuInfo.isR18();
                bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                DateTime startTime = DateTime.Now;
                List<IChatMessage> workMsgs = new List<IChatMessage>();
                FileInfo fileInfo = isShowImg ? await loliconBusiness.downImgAsync(setuInfo.pid.ToString(), setuInfo.urls.original, setuInfo.isGif()) : null;
                workMsgs.Add(new PlainMessage(loliconBusiness.getDefaultWorkInfo(setuInfo, fileInfo, startTime)));
                await session.SendGroupSetuAsync(workMsgs, fileInfo, groupId, isShowImg);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }


    }
}
