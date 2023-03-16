using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class PixivHandler : SetuHandler
    {
        private PixivBusiness pixivBusiness;

        public PixivHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivBusiness = new PixivBusiness();
        }

        public async Task pixivSearchAsync(GroupCommand command)
        {
            try
            {
                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中

                PixivWorkInfo pixivWorkInfo;
                string keyword = command.KeyWord;
                bool isShowAI = command.GroupId.IsShowAISetu();
                bool isShowR18 = command.GroupId.IsShowR18Setu();
                if (await CheckSetuTagEnableAsync(command, keyword) == false) return;

                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.ProcessingMsg) == false)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.SetuConfig.ProcessingMsg);
                    await Task.Delay(1000);
                }

                if (StringHelper.isPureNumber(keyword))
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;
                    pixivWorkInfo = await pixivBusiness.getPixivWorkInfoAsync(keyword);//根据作品id获取作品
                }
                else if (string.IsNullOrEmpty(keyword) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomType.RandomSubscribe)
                {
                    pixivWorkInfo = await pixivBusiness.getRandomWorkInSubscribeAsync(command.GroupId, isShowR18, isShowAI);//获取随机一个订阅中的画师的作品
                }
                else if (string.IsNullOrEmpty(keyword) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomType.RandomFollow)
                {
                    pixivWorkInfo = await pixivBusiness.getRandomWorkInFollowAsync(isShowR18, isShowAI);//获取随机一个关注中的画师的作品
                }
                else if (string.IsNullOrEmpty(keyword) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomType.RandomBookmark)
                {
                    pixivWorkInfo = await pixivBusiness.getRandomWorkInBookmarkAsync(isShowR18, isShowAI);//获取随机一个收藏中的作品
                }
                else if (string.IsNullOrEmpty(keyword))
                {
                    pixivWorkInfo = await pixivBusiness.getRandomWorkInTagsAsync(isShowR18, isShowAI);//获取随机一个标签中的作品
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;
                    pixivWorkInfo = await pixivBusiness.getRandomWorkAsync(keyword, isShowR18, isShowAI);//获取随机一个作品
                }

                if (pixivWorkInfo is null)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.SetuConfig.NotFoundMsg, "找不到这类型的图片或者收藏比过低，换个标签试试吧~");
                    return;
                }

                if (await CheckSetuSendable(command, pixivWorkInfo, isShowR18, isShowAI) == false) return;

                long todayLeft = GetSetuLeftToday(command.GroupId, command.MemberId);
                List<FileInfo> setuFiles = await GetSetuFilesAsync(pixivWorkInfo, command.GroupId);

                string remindTemplate = BotConfig.SetuConfig.Pixiv.Template;
                string pixivTemplate = BotConfig.PixivConfig.Template;
                List<BaseContent> workMsgs = new List<BaseContent>();
                if (string.IsNullOrWhiteSpace(remindTemplate) == false)
                {
                    workMsgs.Add(new PlainContent(pixivBusiness.getSetuRemindMsg(remindTemplate, todayLeft)));
                }

                workMsgs.Add(new PlainContent(pixivBusiness.getWorkInfo(pixivWorkInfo, pixivTemplate)));

                SetuContent setuContent = new SetuContent(workMsgs, setuFiles);
                Task sendGroupTask = command.ReplyGroupSetuAndRevokeAsync(setuContent, BotConfig.SetuConfig.RevokeInterval, BotConfig.PixivConfig.SendImgBehind, true);
                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = command.ReplyTempMessageAsync(setuContent, BotConfig.PixivConfig.SendImgBehind);
                }

                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);//进入CD状态
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"pixivSearchAsync异常");
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, $"pixivSearchAsync异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }


    }
}
