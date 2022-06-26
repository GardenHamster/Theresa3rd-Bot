using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Lolicon;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class LoliconHandler
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

                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.Lolicon.ProcessingMsg) == false)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Lolicon.ProcessingMsg, null);
                    await Task.Delay(1000);
                }

                LoliconResultV2 loliconResult = null;
                int r18Mode = groupId.IsShowR18() ? 2 : 0;
                string tagNames = message.splitKeyWord(BotConfig.SetuConfig.Lolicon.Command) ?? "";
                if (string.IsNullOrEmpty(tagNames))
                {
                    loliconResult = await loliconBusiness.getLoliconResultAsync(r18Mode);
                }
                else
                {
                    string[] tagArr = tagNames.Split(new char[] { ' ', ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                    if (await BusinessHelper.CheckSetuCustomEnableAsync(session, args) == false) return;
                    loliconResult = await loliconBusiness.getLoliconResultAsync(r18Mode, tagArr);
                }

                if (loliconResult == null || loliconResult.data.Count == 0)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Lolicon.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LoliconDataV2 loliconData = loliconResult.data.First();
                if (loliconData.IsImproper())
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Lolicon.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                long todayLeftCount = BusinessHelper.GetSetuLeftToday(session, args);
                FileInfo fileInfo = await loliconBusiness.downImgAsync(loliconData);

                int groupMsgId = 0;
                string template = BotConfig.SetuConfig.Lolicon.Template;
                List<IChatMessage> chatList = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(template))
                {
                    StringBuilder warnBuilder = new StringBuilder();
                    if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(groupId) == false)
                    {
                        if (warnBuilder.Length > 0) warnBuilder.Append("，");
                        warnBuilder.Append($"{BotConfig.SetuConfig.MemberCD}秒后再来哦");
                    }
                    if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(groupId) == false && BotConfig.SetuConfig.MaxDaily > 0)
                    {
                        if (warnBuilder.Length > 0) warnBuilder.Append("，");
                        warnBuilder.Append($"今天剩余使用次数{todayLeftCount}次");
                    }
                    if (BotConfig.SetuConfig.RevokeInterval > 0)
                    {
                        if (warnBuilder.Length > 0) warnBuilder.Append("，");
                        warnBuilder.Append($"本消息将在{BotConfig.SetuConfig.RevokeInterval}秒后撤回，尽快保存哦");
                    }
                    if (warnBuilder.Length > 0)
                    {
                        warnBuilder.Append("\r\n");
                    }

                    chatList.Add(new PlainMessage(warnBuilder.ToString()));
                    chatList.Add(new PlainMessage(loliconBusiness.getDefaultWorkInfo(loliconData, fileInfo, startDateTime)));
                }
                else
                {
                    chatList.Add(new PlainMessage(loliconBusiness.getWorkInfo(loliconData, fileInfo, startDateTime, todayLeftCount, template)));
                }

                try
                {
                    //发送群消息
                    List<IChatMessage> groupList = new List<IChatMessage>(chatList);
                    if (fileInfo == null)
                    {
                        groupList.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg));
                    }
                    else if (loliconData.isR18() == false)
                    {
                        groupList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                    }
                    else if (loliconData.isR18() && groupId.IsShowR18Img())
                    {
                        groupList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                    }
                    groupMsgId = await session.SendMessageWithAtAsync(args, groupList);
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "sendGeneralLoliconImageAsync群消息发送失败");
                    throw;
                }

                if (BotConfig.SetuConfig.SendPrivate)
                {
                    try
                    {
                        //发送临时会话
                        List<IChatMessage> memberList = new List<IChatMessage>(chatList);
                        if (fileInfo == null)
                        {
                            memberList.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, UploadTarget.Temp));
                        }
                        else if (loliconData.isR18() == false)
                        {
                            memberList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Temp, fileInfo.FullName));
                        }
                        else if (loliconData.isR18() && groupId.IsShowR18Img())
                        {
                            memberList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Temp, fileInfo.FullName));
                        }
                        await session.SendTempMessageAsync(memberId, groupId, memberList.ToArray());
                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, "临时消息发送失败");
                    }
                }

                //进入CD状态
                CoolingCache.SetMemberSetuCooling(groupId, memberId);
                if (groupMsgId == 0 || BotConfig.SetuConfig.RevokeInterval == 0) return;

                try
                {
                    //等待撤回
                    await Task.Delay(BotConfig.SetuConfig.RevokeInterval * 1000);
                    await session.RevokeMessageAsync(groupMsgId);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "sendGeneralLoliconImageAsync消息撤回失败");
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "sendGeneralLoliconImageAsync异常");
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Pixiv.ErrorMsg, " 获取图片出错了，再试一次吧~");
            }
            finally
            {
                CoolingCache.SetHandFinish(args.Sender.Group.Id, args.Sender.Id);//请求处理完成
            }
        }





    }
}
