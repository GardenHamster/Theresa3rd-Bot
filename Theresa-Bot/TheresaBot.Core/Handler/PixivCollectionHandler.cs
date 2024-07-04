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
        private RecordService recordService;
        private PixivCollectionService pixivCollectionService;

        public PixivCollectionHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            recordService = new RecordService();
            pixivCollectionService = new PixivCollectionService();
        }

        public async Task AddCollection(GroupCommand command)
        {
            var param = await CheckParamsAsync(command.Params);
            await AddCollection(command, param);
        }

        public async Task AddCollection(GroupQuoteCommand command)
        {
            var param = await CheckParamsAsync(command, command.Params);
            await AddCollection(command, param);
        }

        private async Task AddCollection(GroupCommand command, PixivCollectionParam param)
        {
            try
            {
                var tempDir = string.Empty;
                var config = BotConfig.PixivCollectionConfig;
                var pixivId = param.PixivId;
                var ossPath = string.Empty;
                var localPath = string.Empty;
                var workInfo = await PixivHelper.GetPixivWorkInfoAsync(pixivId.ToString());
                if (config.PixivCollect)
                {
                    await AddBookmark(command, param);
                }
                if (config.LocalCollect || config.OSSCollect)
                {
                    tempDir = await DownOriginal(command, workInfo);
                }
                if (config.LocalCollect)
                {
                    localPath = await CopyToCollection(command, pixivId, tempDir);
                }
                if (config.OSSCollect)
                {
                    ossPath = await UploadOSS(command, pixivId, tempDir);
                }
                await pixivCollectionService.AddPixivCollection(workInfo, param, localPath, ossPath);
                FileHelper.DeleteDirectory(tempDir);
                string remindMsg = $"Pid={param.PixivId}，Level={param.Level}，Title={workInfo.Title}，自定义标签={param.Tags.JoinToString()}";
                await command.ReplyGroupMessageWithQuoteAsync("收藏完毕，" + remindMsg);
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

        private async Task<string> DownOriginal(GroupCommand command, PixivWorkInfo workInfo)
        {
            try
            {
                var taskList = new List<Task>();
                var pixivId = workInfo.illustId;
                var dirSavePath = FilePath.GetPixivUploadDirectory(pixivId);
                var orginUrl = workInfo.urls.original;
                for (int i = 0; i < workInfo.pageCount; i++)
                {
                    var imgUrl = orginUrl.Replace("_p0.", $"_p{i}.");
                    var fullFileName = new HttpFileInfo(imgUrl).FullFileName;
                    Task task = PixivHelper.DownPixivImgAsync(imgUrl, pixivId.ToString(), fullFileName, dirSavePath);
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

        private async Task<string> CopyToCollection(GroupCommand command, int pixivId, string copyDirPath)
        {
            try
            {
                var targetDirPath = FilePath.GetPixivCollectionDirectory(pixivId);
                var directoryInfo = new DirectoryInfo(copyDirPath);
                directoryInfo.CopyToDirectory(targetDirPath);
                return targetDirPath;
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "文件复制失败");
                return string.Empty;
            }
        }

        private async Task<string> UploadOSS(GroupCommand command, int pixivId, string uploadDirPath)
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
                return ossDir;
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "上传文件到OSS失败");
                return string.Empty;
            }
        }

        private async Task<PixivCollectionParam> CheckParamsAsync(string[] paramArr)
        {
            int pixivId = 0, level = 0;
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

        private async Task<PixivCollectionParam> CheckParamsAsync(GroupQuoteCommand command, string[] paramArr)
        {
            var level = 0;
            var groupId = command.GroupId;
            var tags = new List<string>();
            var quoteMsgId = command.GetQuoteMessageId();
            var pixivRecord = recordService.GetPixivRecord(Session.PlatformType, quoteMsgId, groupId).FirstOrDefault();
            if (pixivRecord is null)
            {
                throw new ProcessException("未能读取引用消息中的Pid，请使用收藏指令进行收藏");
            }
            var pixivId = pixivRecord.PixivId;
            if (paramArr.Length == 0)
            {
                return await Task.FromResult(new PixivCollectionParam(pixivId, level, tags));
            }
            if (int.TryParse(paramArr[0], out level))
            {
                tags = paramArr.Skip(1).ToList();
            }
            else
            {
                tags = paramArr.ToList();
            }
            return await Task.FromResult(new PixivCollectionParam(pixivId, level, tags));
        }





    }
}
