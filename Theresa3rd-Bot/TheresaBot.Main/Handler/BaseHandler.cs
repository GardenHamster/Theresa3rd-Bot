using AnimatedGif;
using System.Drawing;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Base;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    public abstract class BaseHandler
    {
        protected BaseSession Session;
        protected BaseReporter Reporter;
        protected RequestRecordBusiness requestRecordBusiness;

        public BaseHandler(BaseSession session, BaseReporter reporter)
        {
            this.Session = session;
            this.Reporter = reporter;
            this.requestRecordBusiness = new RequestRecordBusiness();
        }

        public async Task<int> getUsedCountToday(long groupId, long memberId, params CommandType[] commandTypeArr)
        {
            return await Task.FromResult(requestRecordBusiness.getUsedCountToday(groupId, memberId, commandTypeArr));
        }

        public async Task<RequestRecordPO> addRecord(GroupCommand command)
        {
            return await Task.FromResult(requestRecordBusiness.addRecord(command.GroupId, command.MemberId, command.CommandType, command.Instruction));
        }

        public async Task<RequestRecordPO> addRecord(FriendCommand command)
        {
            return await Task.FromResult(requestRecordBusiness.addRecord(0, command.MemberId, command.CommandType, command.Instruction));
        }

        /// <summary>
        /// 检查pixiv cookie是否已经过期
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<bool> CheckPixivCookieAvailableAsync(GroupCommand command)
        {
            if (string.IsNullOrWhiteSpace(BotConfig.WebsiteConfig.Pixiv.Cookie))
            {
                await command.ReplyGroupMessageWithAtAsync("缺少pixiv cookie，请设置cookie");
                return false;
            }
            if (DateTime.Now > BotConfig.WebsiteConfig.Pixiv.CookieExpireDate)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.PixivConfig.CookieExpireMsg, "cookie过期了，让管理员更新cookie吧");
                return false;
            }
            if (BotConfig.WebsiteConfig.Pixiv.UserId <= 0)
            {
                await command.ReplyGroupMessageWithAtAsync("缺少userId，请更新cookie");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查订阅功能是否开启
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<bool> CheckSubscribeEnableAsync(GroupCommand command, BaseSubscribeConfig subscribeConfig)
        {
            if (BotConfig.PermissionsConfig.SubscribeGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (subscribeConfig is null || subscribeConfig.Enable == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.DisableMsg, "该功能已关闭");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查涩图功能是否可用
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSetuEnableAsync(GroupCommand command, BasePluginConfig pluginConfig)
        {
            if (BotConfig.PermissionsConfig.SetuGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (pluginConfig is null || pluginConfig.Enable == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.DisableMsg, "该功能已关闭");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查原图功能是否可用
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSaucenaoEnableAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SaucenaoGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (BotConfig.SaucenaoConfig is null || BotConfig.SaucenaoConfig.Enable == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.DisableMsg, "该功能已关闭");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查是否超级管理员
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSuperManagersAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SuperManagers.Contains(command.MemberId) == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.ManagersRequiredMsg, "该功能需要管理员执行");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查是否超级管理员
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSuperManagersAsync(FriendCommand command)
        {
            if (BotConfig.PermissionsConfig.SuperManagers.Contains(command.MemberId) == false)
            {
                await command.ReplyFriendTemplateAsync(BotConfig.GeneralConfig.ManagersRequiredMsg, "该功能需要管理员执行");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查涩图功能是否在冷却中
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckMemberSetuCoolingAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int cdSecond = CoolingCache.GetMemberSetuCooling(command.GroupId, command.MemberId);
            if (cdSecond <= 0) return false;
            await command.ReplyGroupMessageWithAtAsync($"功能冷却中，{cdSecond}秒后再来哦~");
            return true;
        }

        /// <summary>
        /// 检查涩图功能是否开启
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckGroupSetuCoolingAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int cdSecond = CoolingCache.GetGroupSetuCooling(command.GroupId, command.MemberId);
            if (cdSecond <= 0) return false;
            await command.ReplyGroupMessageWithAtAsync($"群功能冷却中，{cdSecond}秒后再来哦~");
            return true;
        }

        /// <summary>
        /// 检查涩图功能可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSetuUseUpAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            if (BotConfig.SetuConfig.MaxDaily == 0) return false;
            int useCount = new RequestRecordBusiness().getUsedCountToday(command.GroupId, command.MemberId, CommandType.Setu);
            if (useCount < BotConfig.SetuConfig.MaxDaily) return false;
            await command.ReplyGroupMessageWithAtAsync("你今天的使用次数已经达到上限了，明天再来吧");
            return true;
        }

        /// <summary>
        /// 检查原图功能可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSaucenaoUseUpAsync(GroupCommand command)
        {
            if (BotConfig.SaucenaoConfig.MaxDaily == 0) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int useCount = new RequestRecordBusiness().getUsedCountToday(command.GroupId, command.MemberId, CommandType.Saucenao);
            if (useCount < BotConfig.SaucenaoConfig.MaxDaily) return false;
            await command.ReplyGroupMessageWithAtAsync("你今天的使用次数已经达到上限了，明天再来吧");
            return true;
        }

        /// <summary>
        /// 检查原图功能是否在冷却中
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckMemberSaucenaoCoolingAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int cdSecond = CoolingCache.GetMemberSaucenaoCooling(command.GroupId, command.MemberId);
            if (cdSecond <= 0) return false;
            await command.ReplyGroupMessageWithAtAsync($"功能冷却中，{cdSecond}秒后再来哦~");
            return true;
        }

        /// <summary>
        /// 检查是否有涩图请求在处理中
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckHandingAsync(GroupCommand command)
        {
            if (CoolingCache.IsHanding(command.GroupId, command.MemberId) == false) return false;
            await command.ReplyGroupMessageWithAtAsync("你的一个请求正在处理中，稍后再来吧");
            return true;
        }



        /// <summary>
        /// 根据配置使用代理或者直连下载图片
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        public async Task<List<FileInfo>> downPixivImgsAsync(BaseWorkInfo pixivWorkInfo)
        {
            try
            {
                if (pixivWorkInfo.IsGif)
                {
                    return new List<FileInfo>() { await downAndComposeGifAsync(pixivWorkInfo.PixivId) };
                }
                List<FileInfo> imgList = new List<FileInfo>();
                List<string> originUrls = pixivWorkInfo.getOriginalUrls();
                int maxCount = BotConfig.PixivConfig.ImgShowMaximum <= 0 ? originUrls.Count : BotConfig.PixivConfig.ImgShowMaximum;
                for (int i = 0; i < maxCount && i < originUrls.Count; i++)
                {
                    imgList.Add(await downPixivImgAsync(pixivWorkInfo.PixivId, originUrls[i], BotConfig.PixivConfig.ImgRetryTimes));
                }
                return imgList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "PixivBusiness.downImg下载图片失败");
                return null;
            }
        }

        private async Task<FileInfo> downPixivImgAsync(string pixivId, string originUrl, int retryTimes, string fullFileName = null)
        {
            if (retryTimes < 0) retryTimes = 0;
            while (retryTimes >= 0)
            {
                try
                {
                    string downloadUrl = getDownImgUrl(originUrl);
                    if (string.IsNullOrWhiteSpace(fullFileName)) fullFileName = originUrl.getHttpFileName();
                    string fullImgSavePath = Path.Combine(FilePath.getDownImgSavePath(), fullFileName);
                    Dictionary<string, string> headerDic = new Dictionary<string, string>();
                    headerDic.Add("Referer", HttpUrl.getPixivArtworksReferer(pixivId));
                    headerDic.Add("Cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);
                    if (BotConfig.PixivConfig.FreeProxy || string.IsNullOrWhiteSpace(BotConfig.PixivConfig.ImgProxy) == false)
                    {
                        return await HttpHelper.DownFileAsync(downloadUrl.ToProxyUrl(), fullImgSavePath);
                    }
                    else if (string.IsNullOrWhiteSpace(BotConfig.PixivConfig.HttpProxy) == false)
                    {
                        return await HttpHelper.DownFileWithProxyAsync(downloadUrl.ToPximgUrl(), fullImgSavePath, headerDic);
                    }
                    else
                    {
                        return await HttpHelper.DownFileAsync(downloadUrl.ToPximgUrl(), fullImgSavePath, headerDic);
                    }
                }
                catch (Exception)
                {
                    if (--retryTimes < 0) throw;
                    await Task.Delay(3000);
                }
            }
            return null;
        }

        /// <summary>
        /// 下载动图zip包并合成gif图片
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        protected async Task<FileInfo> downAndComposeGifAsync(string pixivId)
        {
            try
            {
                string fullGifSavePath = Path.Combine(FilePath.getDownImgSavePath(), $"{pixivId}.gif");
                if (File.Exists(fullGifSavePath)) return new FileInfo(fullGifSavePath);

                PixivResult<PixivUgoiraMeta> pixivUgoiraMetaDto = await PixivHelper.GetPixivUgoiraMetaAsync(pixivId);
                string fullZipSavePath = Path.Combine(FilePath.getDownImgSavePath(), $"{StringHelper.get16UUID()}.zip");
                string zipHttpUrl = pixivUgoiraMetaDto.body.src;

                Dictionary<string, string> headerDic = new Dictionary<string, string>();
                headerDic.Add("Referer", HttpUrl.getPixivArtworksReferer(pixivId));
                headerDic.Add("Cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);

                if (BotConfig.PixivConfig.FreeProxy || string.IsNullOrWhiteSpace(BotConfig.PixivConfig.ImgProxy) == false)
                {
                    await HttpHelper.DownFileAsync(zipHttpUrl.ToProxyUrl(), fullZipSavePath);
                }
                else if (string.IsNullOrWhiteSpace(BotConfig.PixivConfig.HttpProxy) == false)
                {
                    await HttpHelper.DownFileWithProxyAsync(zipHttpUrl.ToPximgUrl(), fullZipSavePath, headerDic);
                }
                else
                {
                    await HttpHelper.DownFileAsync(zipHttpUrl.ToPximgUrl(), fullZipSavePath, headerDic);
                }

                string unZipDirPath = Path.Combine(FilePath.getDownImgSavePath(), pixivId);
                ZipHelper.ZipToFile(fullZipSavePath, unZipDirPath);
                DirectoryInfo directoryInfo = new DirectoryInfo(unZipDirPath);
                FileInfo[] files = directoryInfo.GetFiles();
                List<PixivUgoiraMetaFrames> frames = pixivUgoiraMetaDto.body.frames;
                using AnimatedGifCreator gif = AnimatedGif.AnimatedGif.Create(fullGifSavePath, 0);
                foreach (FileInfo file in files)
                {
                    PixivUgoiraMetaFrames frame = frames.Where(o => o.file.Trim() == file.Name).FirstOrDefault();
                    int delay = frame is null ? 60 : frame.delay;
                    using Image img = Image.FromFile(file.FullName);
                    gif.AddFrame(img, delay, GifQuality.Bit8);
                    await Task.Delay(1000);
                }
                FileHelper.deleteFile(fullZipSavePath);
                FileHelper.deleteDirectory(unZipDirPath);
                return new FileInfo(fullGifSavePath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "gif合成失败");
                Reporter.SendError(ex, "gif合成失败");
                return null;
            }
        }

        /// <summary>
        /// 根据配置文件设置的图片大小获取图片下载地址
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        private string getDownImgUrl(string originalUrl)
        {
            string imgSize = BotConfig.PixivConfig.ImgSize?.ToLower();
            if (imgSize == "original") return originalUrl;
            if (imgSize == "regular") return originalUrl.ToRegularUrl();
            if (imgSize == "small") return originalUrl.ToSmallUrl();
            if (imgSize == "thumb") return originalUrl.ToThumbUrl();
            return originalUrl.ToThumbUrl();
        }


    }
}
