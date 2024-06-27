using TheresaBot.Core.Command;
using TheresaBot.Core.Common;
using TheresaBot.Core.Datas;
using TheresaBot.Core.Exceptions;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Infos;
using TheresaBot.Core.Model.Pixiv;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Services;
using TheresaBot.Core.Session;

namespace TheresaBot.Core.Handler
{
    internal class PixivCollectionHandler : BaseHandler
    {
        private PixivCollectionService pixivCollectionService;

        public PixivCollectionHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivCollectionService = new PixivCollectionService();
        }

        public async Task AddCollection(GroupCommand command)
        {
            try
            {
                var tempDir = string.Empty;
                var config = BotConfig.PixivCollectionConfig;
                var param = await CheckParamsAsync(command.Params);
                var taskList = new List<Task>();
                if (config.PixivCollect)
                {
                    await AddBookmark(command, param);
                }
                if (config.LocalCollect || config.OSSCollect)
                {
                    tempDir = await DownOriginal(command, param);
                }
                if (config.LocalCollect)
                {
                    await CopyToCollection(command, param.PixivId, tempDir);
                }
                if (config.OSSCollect)
                {
                    await UploadOSS(command, param.PixivId, tempDir);
                }
                //Directory.Delete(tempDir, true);
                await command.ReplyGroupMessageWithQuoteAsync("收藏完毕!");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "添加收藏异常");
            }
        }


        public async Task AddCollection(GroupQuoteCommand command)
        {
            await Task.CompletedTask;
        }

        private async Task AddBookmark(GroupCommand command, PixivCollectionParam param)
        {
            try
            {
                string token = WebsiteDatas.Pixiv?.CsrfToken ?? string.Empty;
                if (string.IsNullOrWhiteSpace(token))
                {
                    await command.ReplyGroupMessageWithQuoteAsync("未检测到Csrf-Token，请先配置token");
                    return;
                }
                await PixivHelper.PostPixivBookmarkAsync(param.PixivId.ToString());
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "添加Pixiv收藏失败");
            }
        }

        private async Task<string> DownOriginal(GroupCommand command, PixivCollectionParam param)
        {
            try
            {
                var taskList = new List<Task>();
                var pixivId = param.PixivId.ToString();
                var dirSavePath = FilePath.GetPixivUploadDirectory(param.PixivId);
                var workInfo = await PixivHelper.GetPixivWorkInfoAsync(param.PixivId.ToString());
                var orginUrl = workInfo.urls.original;
                for (int i = 0; i < workInfo.pageCount; i++)
                {
                    var imgUrl = orginUrl.Replace("_p0.", $"_p{i}.");
                    var fullFileName = new HttpFileInfo(imgUrl).FullFileName;
                    Task task = PixivHelper.DownPixivImgAsync(imgUrl, pixivId, fullFileName, dirSavePath);
                    taskList.Add(task);
                }
                Task.WaitAll(taskList.ToArray());
                return await Task.FromResult(dirSavePath);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "Pixiv收藏原图下载失败");
                throw;
            }
        }

        private async Task CopyToCollection(GroupCommand command, int pixivId, string copyDirPath)
        {
            try
            {
                var targetDirPath = FilePath.GetPixivCollectionDirectory(pixivId);
                var directoryInfo = new DirectoryInfo(copyDirPath);
                directoryInfo.CopyToDirectory(targetDirPath);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "文件复制失败");
            }
        }

        private async Task UploadOSS(GroupCommand command, int pixivId, string uploadDirPath)
        {
            try
            {
                var ossDir = FilePath.GetPixivDirectory(pixivId);
                var directoryInfo = new DirectoryInfo(uploadDirPath);
                var files = directoryInfo.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    var ossPath = Path.Combine(ossDir, files[i].Name);
                    await OSSHelper.UploadFile(files[i], ossPath);
                }
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "上传文件到OSS失败");
            }
        }

        private async Task<PixivCollectionParam> CheckParamsAsync(string[] paramArr)
        {
            var pixivId = 0;
            var level = 0;
            var tags = new List<string>();
            if (paramArr.Length == 0)
            {
                throw new ProcessException("至少包含一个PixivId");
            }
            if (int.TryParse(paramArr[0], out pixivId) == false)
            {
                throw new ProcessException("PixivId必须为数字");
            }
            if (pixivId < 1000000)
            {
                throw new ProcessException("PixivId必须在1000000及以上");
            }
            if (paramArr.Length == 1)
            {
                return await Task.FromResult(new PixivCollectionParam(pixivId, level, tags));
            }
            if (int.TryParse(paramArr[1], out level))
            {
                tags = paramArr.Skip(2).ToList();
            }
            else
            {
                tags = paramArr.Skip(1).ToList();
            }
            return await Task.FromResult(new PixivCollectionParam(pixivId, level, tags));
        }



    }
}
