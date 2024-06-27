﻿using TheresaBot.Core.Cache;
using TheresaBot.Core.Command;
using TheresaBot.Core.Common;
using TheresaBot.Core.Datas;
using TheresaBot.Core.Drawer;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Cache;
using TheresaBot.Core.Model.Content;
using TheresaBot.Core.Model.Pixiv;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Services;
using TheresaBot.Core.Session;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Handler
{
    internal class PixivHandler : SetuHandler
    {
        private PixivService pixivService;

        public PixivHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivService = new PixivService();
        }

        public async Task PixivSearchAsync(GroupCommand command)
        {
            try
            {
                PixivWorkInfo pixivWorkInfo;
                string keyword = command.KeyWord;
                bool isShowAI = command.GroupId.IsShowAISetu();
                bool isShowR18 = command.GroupId.IsShowR18();

                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中
                if (await CheckSetuTagEnableAsync(command, keyword) == false) return;
                await command.ReplyProcessingMessageAsync(BotConfig.SetuConfig.ProcessingMsg);

                if (BusinessHelper.IsPixivId(keyword))
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;
                    pixivWorkInfo = await pixivService.FetchWorkInfoAsync(keyword);//根据作品id获取作品
                }
                else if (string.IsNullOrEmpty(keyword) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomType.RandomSubscribe)
                {
                    pixivWorkInfo = await pixivService.FetchRandomWorkInSubscribeAsync(command.GroupId, isShowR18, isShowAI);//获取随机一个订阅中的画师的作品
                }
                else if (string.IsNullOrEmpty(keyword) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomType.RandomFollow)
                {
                    pixivWorkInfo = await pixivService.FetchRandomWorkInFollowAsync(isShowR18, isShowAI);//获取随机一个关注中的画师的作品
                }
                else if (string.IsNullOrEmpty(keyword) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomType.RandomBookmark)
                {
                    pixivWorkInfo = await pixivService.FetchRandomWorkInBookmarkAsync(isShowR18, isShowAI);//获取随机一个收藏中的作品
                }
                else if (string.IsNullOrEmpty(keyword))
                {
                    pixivWorkInfo = await pixivService.FetchRandomWorkInTagsAsync(isShowR18, isShowAI);//获取随机一个标签中的作品
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;
                    pixivWorkInfo = await pixivService.FetchRandomWorkAsync(keyword, isShowR18, isShowAI);//获取随机一个作品
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
                    workMsgs.Add(new PlainContent(pixivService.GetSetuRemind(remindTemplate, todayLeft)));
                }

                workMsgs.Add(new PlainContent(pixivService.GetWorkInfo(pixivWorkInfo)));

                PixivSetuContent setuContent = new PixivSetuContent(workMsgs, setuFiles, pixivWorkInfo);
                var results = await command.ReplyGroupSetuAsync(setuContent, BotConfig.SetuConfig.RevokeInterval, BotConfig.PixivConfig.SendImgBehind);
                var msgIds = results.Select(o => o.MessageId).ToArray();
                var recordTask = recordService.InsertPixivRecord(setuContent, Session.PlatformType, msgIds, command.GroupId);
                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = command.SendPrivateSetuAsync(setuContent, BotConfig.PixivConfig.SendImgBehind);
                }

                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);//进入CD状态
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "Pixiv搜索异常");
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
                if (userId.IsBanPixiver())
                {
                    await command.ReplyGroupMessageWithQuoteAsync("这个画师ID已被屏蔽~");
                    return;
                }

                CoolingCache.SetHanding(command.GroupId, command.MemberId);
                await command.ReplyProcessingMessageAsync(BotConfig.SetuConfig.PixivUser.ProcessingMsg);

                PixivUserProfileInfo profileInfo = PixivUserProfileCache.GetCache(userId);
                if (profileInfo == null)
                {
                    profileInfo = await pixivService.FetchUserProfileAsync(userId, command.GroupId);
                    PixivUserProfileCache.AddCache(userId, profileInfo);
                }

                string template = BotConfig.SetuConfig.PixivUser.Template;
                string templateMsg = pixivService.GetUserProfileMessage(profileInfo.UserName, template);

                List<string> PreviewFilePaths = profileInfo.PreviewFilePaths;
                if (PreviewFilePaths is null || PreviewFilePaths.IsFilesExists() == false)
                {
                    PreviewFilePaths = await CreatePreviewImgAsync(profileInfo);
                    profileInfo.PreviewFilePaths = PreviewFilePaths;
                }

                BaseContent[] titleContents = new BaseContent[] { new PlainContent(templateMsg) };

                List<SetuContent> setuContents = new List<SetuContent>();
                setuContents.AddRange(PreviewFilePaths.Select(o => new SetuContent(new FileInfo(o))));
                setuContents.AddRange(pixivService.GetNoAndPids(profileInfo, 10));

                await command.ReplyGroupMessageWithQuoteAsync(templateMsg);
                await Task.Delay(1000);
                await SendGroupMergeSetuAsync(setuContents, new() { titleContents }, command.GroupId);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "画师作品一览功能异常");
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
                using var drawer = new PixivUserWorkDrawer();
                return await drawer.DrawPreview(profileInfo, details, fullSavePath);
            }
            catch (Exception ex)
            {
                await LogAndReportError(ex, "画师作品一览合成失败");
                return null;
            }
        }


    }
}