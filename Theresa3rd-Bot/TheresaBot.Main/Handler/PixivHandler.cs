using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Drawer;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Cache;
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

        public async Task PixivSearchAsync(GroupCommand command)
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
                    await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.SetuConfig.ProcessingMsg);
                    await Task.Delay(1000);
                }

                if (BusinessHelper.IsPixivId(keyword))
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
                    await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.SetuConfig.NotFoundMsg, "找不到这类型的图片或者收藏比过低，换个标签试试吧~");
                    return;
                }

                if (await CheckSetuSendable(command, pixivWorkInfo, isShowR18) == false) return;

                long todayLeft = GetSetuLeftToday(command.GroupId, command.MemberId);
                List<FileInfo> setuFiles = await GetSetuFilesAsync(pixivWorkInfo, command.GroupId);

                string remindTemplate = BotConfig.SetuConfig.Pixiv.Template;
                string pixivTemplate = BotConfig.PixivConfig.Template;
                List<BaseContent> workMsgs = new List<BaseContent>();
                if (string.IsNullOrWhiteSpace(remindTemplate) == false)
                {
                    workMsgs.Add(new PlainContent(pixivBusiness.getSetuRemindMsg(remindTemplate, todayLeft)));
                }

                workMsgs.Add(new PlainContent(pixivBusiness.getWorkInfo(pixivWorkInfo)));

                PixivSetuContent setuContent = new PixivSetuContent(workMsgs, setuFiles, pixivWorkInfo);
                var results = await command.ReplyGroupSetuAsync(setuContent, BotConfig.SetuConfig.RevokeInterval, BotConfig.PixivConfig.SendImgBehind);
                var msgIds = results.Select(o => o.MessageId).ToArray();
                var recordTask = recordBusiness.AddPixivRecord(setuContent, Session.PlatformType, msgIds, command.GroupId);
                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = command.SendTempSetuAsync(setuContent, BotConfig.PixivConfig.SendImgBehind);
                }

                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);//进入CD状态
            }
            catch (Exception ex)
            {
                string errMsg = $"pixivSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }


        public async Task PixivUserProfileAsync(GroupCommand command)
        {
            try
            {
                string userId = command.KeyWord;
                if (StringHelper.IsPureNumber(userId) == false)
                {
                    await command.ReplyGroupMessageWithQuoteAsync("请指定一个画师id~");
                    return;
                }

                CoolingCache.SetHanding(command.GroupId, command.MemberId);
                await command.ReplyProcessingMessageAsync(BotConfig.SetuConfig.PixivUser.ProcessingMsg);

                PixivUserProfileInfo profileInfo = PixivUserProfileCache.GetCache(userId);
                if (profileInfo == null)
                {
                    profileInfo = await pixivBusiness.getUserProfileInfoAsync(userId, command.GroupId);
                    PixivUserProfileCache.AddCache(userId, profileInfo);
                }

                string template = BotConfig.SetuConfig.PixivUser.Template;
                string templateMsg = pixivBusiness.getUserProfileMsg(profileInfo.UserName, template);

                List<string> PreviewFilePaths = profileInfo.PreviewFilePaths;
                if (PreviewFilePaths is null || PreviewFilePaths.IsFilesExists() == false)
                {
                    PreviewFilePaths = await CreatePreviewImgAsync(profileInfo);
                    profileInfo.PreviewFilePaths = PreviewFilePaths;
                }

                BaseContent[] titleContents = new BaseContent[] { new PlainContent(templateMsg) };

                List<SetuContent> setuContents = new List<SetuContent>();
                setuContents.AddRange(PreviewFilePaths.Select(o => new SetuContent(new FileInfo(o))));
                setuContents.AddRange(pixivBusiness.getNumAndPids(profileInfo, 10));

                await command.ReplyGroupMessageWithQuoteAsync(templateMsg);
                await Task.Delay(1000);
                await SendGroupMergeSetuAsync(setuContents, new() { titleContents }, command.GroupId);
            }
            catch (Exception ex)
            {
                string errMsg = $"pixivUserProfileAsync异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        private async Task<List<string>> CreatePreviewImgAsync(PixivUserProfileInfo profileInfo)
        {
            int startIndex = 0;
            int previewInPage = BotConfig.SetuConfig.PixivUser.PreviewInPage;
            if (previewInPage <= 0) previewInPage = 30;
            List<string> fileInfos = new List<string>();
            List<PixivProfileDetail> profileDetails = profileInfo.ProfileDetails;
            if (profileDetails.Count == 0) return fileInfos;
            while (startIndex < profileDetails.Count)
            {
                string fileName = $"pixiv_user_{profileInfo.UserId}_preview_{startIndex}_{startIndex + previewInPage}.jpg";
                string fullSavePath = Path.Combine(FilePath.GetPixivPreviewDirectory(), fileName);
                var partList = profileDetails.Skip(startIndex).Take(previewInPage).ToList();
                var previewFile = await CreatePreviewImgAsync(profileInfo, partList, fullSavePath);
                if (previewFile is not null) fileInfos.Add(previewFile.FullName);
                startIndex += previewInPage;
            }
            return fileInfos;
        }

        private async Task<FileInfo> CreatePreviewImgAsync(PixivUserProfileInfo profileInfo, List<PixivProfileDetail> details, string fullSavePath)
        {
            try
            {
                return await new PixivUserWorkDrawer().DrawPreview(profileInfo, details, fullSavePath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "画师作品一览图合成失败");
                await Reporter.SendError(ex, "画师作品一览图合成失败");
                return null;
            }
        }


    }
}
