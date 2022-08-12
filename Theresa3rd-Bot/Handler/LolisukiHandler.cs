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
using Theresa3rd_Bot.Model.Lolisuki;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class LolisukiHandler : BaseHandler
    {
        private LolisukiBusiness lolisukiBusiness;

        public LolisukiHandler()
        {
            lolisukiBusiness = new LolisukiBusiness();
        }

        public async Task sendGeneralLolisukiImageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(groupId, memberId);//请求处理中

                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.ProcessingMsg) == false)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.ProcessingMsg, null);
                    await Task.Delay(1000);
                }

                LolisukiResult lolisukiResult = null;
                bool isShowR18 = groupId.IsShowR18Setu();
                int r18Mode = isShowR18 ? 2 : 0;
                string levelStr = getLevelStr(isShowR18);
                string tagStr = message.splitKeyWord(BotConfig.SetuConfig.Lolisuki.Command) ?? "";
                if (string.IsNullOrEmpty(tagStr))
                {
                    lolisukiResult = await lolisukiBusiness.getLolisukiResultAsync(r18Mode, levelStr);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(session, args) == false) return;
                    if (await CheckSetuTagEnableAsync(session, args, tagStr) == false) return;
                    string[] tagArr = toLoliconTagArr(tagStr);
                    lolisukiResult = await lolisukiBusiness.getLolisukiResultAsync(r18Mode, levelStr, tagArr);
                }

                if (lolisukiResult == null || lolisukiResult.data.Count == 0)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LolisukiData lolisukiData = lolisukiResult.data.First();
                if (lolisukiData.IsImproper() || lolisukiData.hasBanTag())
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                bool isShowImg = groupId.IsShowSetuImg(lolisukiData.isR18());
                long todayLeftCount = GetSetuLeftToday(groupId, memberId);
                FileInfo fileInfo = isShowImg ? await lolisukiBusiness.downImgAsync(lolisukiData) : null;

                int groupMsgId = 0;
                string template = BotConfig.SetuConfig.Lolisuki.Template;
                List<IChatMessage> chatList = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(template))
                {
                    chatList.Add(new PlainMessage(lolisukiBusiness.getDefaultRemindMsg(groupId, todayLeftCount)));
                    chatList.Add(new PlainMessage(lolisukiBusiness.getDefaultWorkInfo(lolisukiData, fileInfo, startDateTime)));
                }
                else
                {
                    chatList.Add(new PlainMessage(lolisukiBusiness.getWorkInfo(lolisukiData, fileInfo, startDateTime, todayLeftCount, template)));
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
                    LogHelper.Error(ex, "sendGeneralLolisukiImageAsync群消息发送失败");
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
                    await session.RevokeMessageAsync(groupMsgId);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "sendGeneralLolisukiImageAsync消息撤回失败");
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "sendGeneralLolisukiImageAsync异常");
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.ErrorMsg, " 获取图片出错了，再试一次吧~");
            }
            finally
            {
                CoolingCache.SetHandFinish(args.Sender.Group.Id, args.Sender.Id);//请求处理完成
            }
        }


        private string getLevelStr(bool isShowR18)
        {
            try
            {
                string levelStr = BotConfig.SetuConfig.Lolisuki.Level;
                if (string.IsNullOrWhiteSpace(levelStr)) return $"{(int)LolisukiLevel.Level0}-{(int)LolisukiLevel.Level3}";

                string[] levelArr = levelStr.Split('-', StringSplitOptions.RemoveEmptyEntries);
                string minLevelStr = levelArr[0].Trim();
                string maxLevelStr = levelArr.Length > 1 ? levelArr[1].Trim() : levelArr[0].Trim();
                int minLevel = int.Parse(minLevelStr);
                int maxLevel = int.Parse(maxLevelStr);
                if (minLevel < (int)LolisukiLevel.Level0) minLevel = (int)LolisukiLevel.Level0;
                if (maxLevel > (int)LolisukiLevel.Level6) maxLevel = (int)LolisukiLevel.Level6;
                if (maxLevel > (int)LolisukiLevel.Level4 && isShowR18 == false) maxLevel = (int)LolisukiLevel.Level4;
                return minLevel == maxLevel ? $"{minLevel}" : $"{minLevel}-{maxLevel}";
            }
            catch (Exception)
            {
                return $"{(int)LolisukiLevel.Level0}-{(int)(isShowR18 ? LolisukiLevel.Level6 : LolisukiLevel.Level3)}";
            }
        }





    }
}
