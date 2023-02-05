using AnimatedGif;
using System.Drawing;
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
    public abstract class SetuHandler : BaseHandler
    {
        public SetuHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
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
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="setuInfo"></param>
        /// <param name="isShowR18"></param>
        /// <returns></returns>
        public async Task<bool> CheckSetuSendable(GroupCommand command, BaseWorkInfo setuInfo, bool isShowR18, bool isShowAI)
        {
            if (setuInfo.IsImproper)
            {
                await command.ReplyGroupMessageWithAtAsync("该作品含有R18G等内容，不显示相关内容");
                return false;
            }

            string banTagStr = setuInfo.hasBanTag();
            if (banTagStr != null)
            {
                await command.ReplyGroupMessageWithAtAsync($"该作品含有被屏蔽的标签【{banTagStr}】，不显示相关内容");
                return false;
            }

            if (setuInfo.IsR18 && isShowR18 == false)
            {
                await command.ReplyGroupMessageWithAtAsync("该作品为R-18作品，不显示相关内容，如需显示请在配置文件中修改权限");
                return false;
            }

            if (setuInfo.IsAI && isShowAI == false)
            {
                await command.ReplyGroupMessageWithAtAsync("该作品为AI生成作品，不显示相关内容，如需显示请在配置文件中修改权限");
                return false;
            }
            return true;
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
                List<BaseContent> chainList = new List<BaseContent>();
                string template = timingSetuTimer.TimingMsg;
                if (string.IsNullOrWhiteSpace(template))
                {
                    if (chainList.Count == 0) return;
                    await Session.SendGroupMessageAsync(groupId, chainList, timingSetuTimer.AtAll);
                    return;
                }
                template = template.Replace("{Tags}", tags);
                template = template.Replace("{SourceName}", timingSetuTimer.Source.GetTypeName());
                chainList.AddRange(template.SplitToChainAsync());
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
        public static long GetSetuLeftToday(long groupId, long memberId)
        {
            if (BotConfig.SetuConfig.MaxDaily == 0) return 0;
            if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(groupId)) return BotConfig.SetuConfig.MaxDaily;
            RequestRecordBusiness requestRecordBusiness = new RequestRecordBusiness();
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
        public static int GetSaucenaoLeftToday(long groupId, long memberId)
        {
            if (BotConfig.SaucenaoConfig.MaxDaily == 0) return 0;
            RequestRecordBusiness requestRecordBusiness = new RequestRecordBusiness();
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
                    imgList.Add(await PixivHelper.DownPixivImgAsync(pixivWorkInfo.PixivId, originUrls[i]));
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
        protected async Task<FileInfo> downAndComposeGifAsync(string pixivId)
        {
            try
            {
                string fullGifSavePath = Path.Combine(FilePath.getDownFileSavePath(), $"{pixivId}.gif");
                if (File.Exists(fullGifSavePath)) return new FileInfo(fullGifSavePath);

                PixivUgoiraMeta pixivUgoiraMetaDto = await PixivHelper.GetPixivUgoiraMetaAsync(pixivId);
                string zipHttpUrl = pixivUgoiraMetaDto.src;

                string fullZipSavePath = Path.Combine(FilePath.getDownFileSavePath(), $"{StringHelper.get16UUID()}.zip");
                FileInfo zipFile = await PixivHelper.DownPixivFileAsync(pixivId, zipHttpUrl, fullZipSavePath);
                if (zipFile == null) return null;

                string unZipDirPath = Path.Combine(FilePath.getDownFileSavePath(), pixivId);
                ZipHelper.ZipToFile(zipFile.FullName, unZipDirPath);

                DirectoryInfo directoryInfo = new DirectoryInfo(unZipDirPath);
                FileInfo[] files = directoryInfo.GetFiles();
                List<PixivUgoiraMetaFrames> frames = pixivUgoiraMetaDto.frames;
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


    }
}
