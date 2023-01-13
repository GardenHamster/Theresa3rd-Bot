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
                    loliconResult = await loliconBusiness.getLoliconResultAsync(r18Mode, tagArr);
                }

                if (loliconResult == null || loliconResult.data.Count == 0)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LoliconDataV2 loliconData = loliconResult.data.First();
                if (loliconData.IsImproper())
                {
                    await session.SendMessageWithAtAsync(args,new PlainMessage(" 该作品含有R18G等内容，不显示相关内容"));
                    return;
                }

                string banTagStr = loliconData.hasBanTag();
                if (banTagStr != null)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 该作品含有被屏蔽的标签【{banTagStr}】，不显示相关内容"));
                    return;
                }

                bool isShowImg = groupId.IsShowSetuImg(loliconData.isR18());
                long todayLeftCount = GetSetuLeftToday(groupId, memberId);
                FileInfo fileInfo = isShowImg ? await loliconBusiness.downImgAsync(loliconData.pid.ToString(), loliconData.urls.original, false) : null;

                int groupMsgId = 0;
                string template = BotConfig.SetuConfig.Lolicon.Template;
                List<IChatMessage> chatList = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(template))
                {
                    chatList.Add(new PlainMessage(loliconBusiness.getDefaultWorkInfo(loliconData, fileInfo, startDateTime)));
                }
                else
                {
                    chatList.Add(new PlainMessage(loliconBusiness.getWorkInfo(loliconData, fileInfo, startDateTime, todayLeftCount, template)));
                }

                try
                {
                    //发送群消息
                    List<IChatMessage> groupMsgList = new List<IChatMessage>(chatList);
                    if (isShowImg && fileInfo != null)
                    {
                        groupMsgList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                    }
                    else if (isShowImg && fileInfo == null)
                    {
                        groupMsgList.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg));
                    }
                    groupMsgId = await session.SendMessageWithAtAsync(args, groupMsgList);
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
                        if (isShowImg && fileInfo != null)
                        {
                            memberList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Temp, fileInfo.FullName));
                        }
                        else if (isShowImg && fileInfo == null)
                        {
                            memberList.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, UploadTarget.Temp));
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
                    await session.RevokeMessageAsync(groupMsgId, groupId);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "sendGeneralLoliconImageAsync消息撤回失败");
                }

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

        }



    }
}
