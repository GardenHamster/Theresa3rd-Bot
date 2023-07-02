using SixLabors.ImageSharp;
using TheresaBot.Main.Business;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Base;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal abstract class SetuHandler : BaseHandler
    {
        protected RecordBusiness recordBusiness;

        public SetuHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            recordBusiness = new RecordBusiness();
        }

        public async Task<long[]> SendGroupSetuAsync(List<SetuContent> setuContents, List<SetuContent> headerContents, long groupId, int eachPage = 5)
        {
            int startIndex = 0;
            if (eachPage <= 0) return new long[] { 0 };
            var msgIds = new List<long>();
            while (startIndex < setuContents.Count)
            {
                List<SetuContent> pageContents = new List<SetuContent>();
                pageContents.AddRange(headerContents);
                pageContents.AddRange(setuContents.Skip(startIndex).Take(eachPage).ToList());
                msgIds.AddRange(await SendGroupMergeSetuAsync(pageContents, groupId));
                startIndex += eachPage;
            }
            return msgIds.ToArray();
        }

        public async Task<long[]> SendGroupSetuAsync(List<SetuContent> setuContents, long groupId, bool sendMerge, int margeEachPage = 0)
        {
            if (sendMerge == false || margeEachPage <= 0)
            {
                return await SendGroupSetuAsync(setuContents, groupId, sendMerge);
            }

            int startIndex = 0;
            var msgIds = new List<long>();
            while (startIndex < setuContents.Count)
            {
                List<SetuContent> pageContents = setuContents.Skip(startIndex).Take(margeEachPage).ToList();
                msgIds.AddRange(await SendGroupSetuAsync(pageContents, groupId, sendMerge));
                startIndex += margeEachPage;
            }
            return msgIds.ToArray();
        }

        public async Task<long[]> SendGroupSetuAsync(List<SetuContent> setuContents, long groupId, bool sendMerge)
        {
            if (sendMerge)
            {
                return await SendGroupMergeSetuAsync(setuContents, groupId);
            }
            else
            {
                return await SendGroupSetuAsync(setuContents, groupId);
            }
        }

        private async Task<long[]> SendGroupSetuAsync(List<SetuContent> setuContents, long groupId)
        {
            var msgIdList = new List<long>();
            foreach (var content in setuContents)
            {
                msgIdList.AddRange(await SendGroupSetuAsync(content, groupId));
                await Task.Delay(1000);
            }
            return msgIdList.ToArray();
        }

        public async Task<long[]> SendGroupSetuAsync(SetuContent setuContent, long groupId)
        {
            long[] msgIds = await Session.SendGroupMessageAsync(groupId, setuContent, BotConfig.PixivConfig.SendImgBehind);
            if (msgIds.Where(o => o < 0).Any() && BotConfig.PixivConfig.ImgResend != ResendType.None)
            {
                await Task.Delay(1000);
                SetuContent resendContent = setuContent.ToResendContent(BotConfig.PixivConfig.ImgResend);
                msgIds = await Session.SendGroupMessageAsync(groupId, resendContent, BotConfig.PixivConfig.SendImgBehind);
            }
            Task recordTask = recordBusiness.AddPixivRecord(setuContent, msgIds, groupId);
            return msgIds;
        }

        private async Task<long[]> SendGroupMergeSetuAsync(List<SetuContent> setuContents, long groupId)
        {
            long msgId = await Session.SendGroupMergeAsync(groupId, setuContents);
            if (msgId < 0)
            {
                await Task.Delay(1000);
                List<SetuContent> resendContents = GetResendContent(setuContents);
                msgId = await Session.SendGroupMergeAsync(groupId, resendContents);
            }
            Task recordTask = recordBusiness.AddPixivRecord(setuContents, msgId, groupId);
            return new[] { msgId };
        }

        private List<SetuContent> GetResendContent(List<SetuContent> setuContents)
        {
            if (setuContents is null) return new();
            if (setuContents.Count == 0) return new();
            ResendType resendType = BotConfig.PixivConfig.ImgResend;
            if (resendType == ResendType.None) return new();
            return setuContents.Select(o => o.ToResendContent(resendType)).ToList();
        }

        public async Task<List<FileInfo>> GetSetuFilesAsync(BaseWorkInfo workInfo, long groupId)
        {
            bool isShowImg = groupId.IsShowSetuImg(workInfo.IsR18);
            if (isShowImg == false) return new List<FileInfo>();
            return await GetSetuFilesAsync(workInfo);
        }

        public async Task<List<FileInfo>> GetSetuFilesAsync(BaseWorkInfo workInfo, List<long> groupIds)
        {
            bool isShowImg = groupIds.IsShowSetuImg(workInfo.IsR18);
            if (isShowImg == false) return new List<FileInfo>();
            return await GetSetuFilesAsync(workInfo);
        }

        protected async Task<List<FileInfo>> GetSetuFilesAsync(BaseWorkInfo workInfo)
        {
            if (workInfo.IsGif) return new() { await downAndComposeGifAsync(workInfo) };
            List<FileInfo> setuFiles = await downPixivImgsAsync(workInfo);
            if (workInfo.IsR18 == false) return setuFiles;
            float sigma = BotConfig.PixivConfig.R18ImgBlur;
            return setuFiles.ReduceAndBlur(sigma, 300);
        }

        /// <summary>
        /// 检查是否拥有自定义涩图权限
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSetuCustomEnableAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuCustomGroups.Contains(command.GroupId)) return true;
            await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.SetuCustomDisableMsg, "自定义功能已关闭");
            return false;
        }

        /// <summary>
        /// 检查涩图标签是否被禁止
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSetuTagEnableAsync(GroupCommand command, string tagName)
        {
            tagName = tagName.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(tagName)) return true;
            if (tagName.IsR18() && command.GroupId.IsShowR18Setu() == false)
            {
                await command.ReplyGroupMessageWithAtAsync("本群未设置R18权限，禁止搜索R18相关标签");
                return false;
            }

            List<BanWordPO> banSetuTagList = BotConfig.BanSetuTagList;
            if (banSetuTagList.Where(o => tagName.Contains(o.KeyWord.ToLower().Trim())).Any())
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.SetuConfig.DisableTagsMsg, "禁止查找这个类型的涩图");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查一张涩图是否可以发送，并且发送提示消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="setuInfo"></param>
        /// <param name="isShowR18"></param>
        /// <param name="isShowAI"></param>
        /// <returns></returns>
        public async Task<bool> CheckSetuSendable(GroupCommand command, BaseWorkInfo setuInfo, bool isShowR18)
        {
            string message = IsSetuSendable(command, setuInfo, isShowR18);
            if (string.IsNullOrWhiteSpace(message)) return true;
            await command.ReplyGroupMessageWithAtAsync(message);
            return false;
        }

        /// <summary>
        /// 判断一张涩图是否可以发送，并且返回不可发送原因
        /// </summary>
        /// <param name="command"></param>
        /// <param name="setuInfo"></param>
        /// <param name="isShowR18"></param>
        /// <returns></returns>
        public string IsSetuSendable(GroupCommand command, BaseWorkInfo setuInfo, bool isShowR18)
        {
            if (setuInfo.IsImproper)
            {
                return "该作品含有R18G等内容，不显示相关内容";
            }

            string banTagStr = setuInfo.hasBanTag();
            if (string.IsNullOrWhiteSpace(banTagStr) == false)
            {
                return $"该作品含有被屏蔽的标签【{banTagStr}】，不显示相关内容";
            }

            if (setuInfo.IsR18 && isShowR18 == false)
            {
                return "该作品为R-18作品，不显示相关内容，如需显示请在配置文件中修改权限";
            }
            return null;
        }

        /// <summary>
        /// 发送定时涩图Message
        /// </summary>
        /// <param name="session"></param>
        /// <param name="timingSetuTimer"></param>
        /// <param name="tags"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        protected async Task sendTimingSetuMessageAsync(TimingSetuTimer timingSetuTimer, string tags, long groupId)
        {
            try
            {
                string template = timingSetuTimer.TimingMsg?.Trim()?.TrimLine();
                if (string.IsNullOrWhiteSpace(template)) return;
                template = template.Replace("{Tags}", tags);
                template = template.Replace("{SourceName}", timingSetuTimer.Source.GetTypeName());
                List<BaseContent> chainList = new List<BaseContent>(template.SplitToChainAsync());
                await Session.SendGroupMessageAsync(groupId, chainList, timingSetuTimer.AtAll);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        /// <summary>
        /// 获取今日涩图可用次数
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public long GetSetuLeftToday(long groupId, long memberId)
        {
            if (BotConfig.SetuConfig.MaxDaily == 0) return 0;
            if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(groupId)) return BotConfig.SetuConfig.MaxDaily;
            int todayUseCount = requestRecordBusiness.getUsedCountToday(groupId, memberId, CommandType.Setu);
            long leftToday = BotConfig.SetuConfig.MaxDaily - todayUseCount - 1;
            return leftToday < 0 ? 0 : leftToday;
        }

        /// <summary>
        /// 获取今日原图可用次数
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public int GetSaucenaoLeftToday(long groupId, long memberId)
        {
            if (BotConfig.SaucenaoConfig.MaxDaily == 0) return 0;
            int todayUseCount = requestRecordBusiness.getUsedCountToday(groupId, memberId, CommandType.Saucenao);
            int leftToday = BotConfig.SaucenaoConfig.MaxDaily - todayUseCount - 1;
            return leftToday < 0 ? 0 : leftToday;
        }

        /// <summary>
        /// 将一个tag字符串拆分为LoliconApi的tag参数
        /// </summary>
        /// <param name="tagStr"></param>
        /// <returns></returns>
        public string[] toLoliconTagArr(string tagStr)
        {
            if (string.IsNullOrWhiteSpace(tagStr)) return new string[0];
            tagStr = tagStr.Trim().Replace(",", "|").Replace("，", "|");
            return tagStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }


        /// <summary>
        /// 根据配置使用代理或者直连下载图片
        /// </summary>
        /// <param name="workInfo"></param>
        /// <returns></returns>
        protected async Task<List<FileInfo>> downPixivImgsAsync(BaseWorkInfo workInfo)
        {
            try
            {
                List<FileInfo> imgList = new List<FileInfo>();
                List<string> originUrls = workInfo.getOriginalUrls();
                int maxCount = BotConfig.PixivConfig.ImgShowMaximum <= 0 ? originUrls.Count : BotConfig.PixivConfig.ImgShowMaximum;
                for (int i = 0; i < maxCount && i < originUrls.Count; i++)
                {
                    imgList.Add(await PixivHelper.DownPixivImgBySizeAsync(workInfo.PixivId.ToString(), originUrls[i]));
                }
                return imgList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "downPixivImgsAsync异常");
                return null;
            }
        }


        /// <summary>
        /// 下载动图zip包并合成gif图片
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        protected async Task<FileInfo> downAndComposeGifAsync(BaseWorkInfo workInfo)
        {
            try
            {
                int gifSigma = 15;
                bool isR18 = workInfo.IsR18;
                int pixivId = workInfo.PixivId;
                string fullGifSavePath = Path.Combine(FilePath.GetPixivImgSavePath(pixivId), $"{pixivId}.gif");
                if (File.Exists(fullGifSavePath)) return new FileInfo(fullGifSavePath);

                PixivUgoiraMeta pixivUgoiraMetaDto = await PixivHelper.GetPixivUgoiraMetaAsync(pixivId.ToString());
                string zipHttpUrl = pixivUgoiraMetaDto.src;

                FileInfo zipFile = await PixivHelper.DownPixivFileAsync(pixivId.ToString(), zipHttpUrl);
                if (zipFile == null) return null;

                string unZipDirPath = Path.Combine(FilePath.GetTempSavePath(), pixivId.ToString());
                ZipHelper.ZipToFile(zipFile.FullName, unZipDirPath);

                DirectoryInfo directoryInfo = new DirectoryInfo(unZipDirPath);
                List<FileInfo> files = directoryInfo.GetFiles().ToList();
                if (files.Count == 0) return null;
                if (isR18) files = files.Blur(gifSigma, unZipDirPath);

                FileInfo firstFile = files.First();
                using Image gif = await Image.LoadAsync(firstFile.FullName);
                PixivUgoiraMetaFrames firstFrame = pixivUgoiraMetaDto.frames.Where(o => o.file.Trim() == firstFile.Name).FirstOrDefault();
                gif.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay = firstFrame is null ? 6 : firstFrame.delay / 10;

                for (int i = 1; i < files.Count; i++)
                {
                    using var image = await Image.LoadAsync(files[i].FullName);
                    PixivUgoiraMetaFrames frame = pixivUgoiraMetaDto.frames.Where(o => o.file.Trim() == files[i].Name).FirstOrDefault();
                    image.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay = frame is null ? 6 : frame.delay / 10;
                    gif.Frames.AddFrame(image.Frames.RootFrame);
                }
                gif.SaveAsGif(fullGifSavePath);

                FileHelper.DeleteFile(zipFile);
                FileHelper.DeleteDirectory(unZipDirPath);
                return new FileInfo(fullGifSavePath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "gif合成失败");
                await Reporter.SendError(ex, "gif合成失败");
                return null;
            }
        }


    }
}
