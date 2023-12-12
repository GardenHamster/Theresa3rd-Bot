using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Drawer;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class SubscribeHandler : BaseHandler
    {
        private SubscribeService subscribeService;

        public SubscribeHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            subscribeService = new SubscribeService();
        }

        public async Task ListSubscribeAsync(GroupCommand command)
        {
            try
            {
                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中
                var miyousheSubList = subscribeService.getSubscribes(command.GroupId, SubscribeType.米游社用户);
                var pixivUserSubList = subscribeService.getSubscribes(command.GroupId, SubscribeType.P站画师);
                var pixivTagSubList = subscribeService.getSubscribes(command.GroupId, SubscribeType.P站标签).Select(o => o with { SubscribeCode = String.Empty }).ToList();
                var fullSavePath = FilePath.GetTempImgSavePath();
                using var drawer = new SubscribeDrawer();
                FileInfo fileInfo = drawer.DrawSubscribe(miyousheSubList, pixivUserSubList, pixivTagSubList, fullSavePath);
                var sendContents = new List<BaseContent>
                {
                    new PlainContent("当前群已订阅内容如下"),
                    new LocalImageContent(fileInfo)
                };
                await command.ReplyGroupMessageWithQuoteAsync(sendContents);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "查询订阅列表异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        public async Task CancleSubscribeAsync(GroupCommand command)
        {
            try
            {
                int subscribeId = 0;
                if (command.KeyWord.Length > 0)
                {
                    subscribeId = await CheckSubscribeIdAsync(command.KeyWord);
                }
                else
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var tagStep = processInfo.CreateStep("请在60秒内发送要退订的Id", CheckSubscribeIdAsync);
                    await processInfo.StartProcessing();
                    subscribeId = tagStep.Answer;
                }

                var dbSubscribe = subscribeService.getSubscribe(subscribeId);
                if (dbSubscribe is null)
                {
                    await command.ReplyGroupMessageWithQuoteAsync($"退订失败，订阅Id{subscribeId}不存在");
                    return;
                }

                subscribeService.deleteSubscribe(dbSubscribe.Id);
                await command.ReplyGroupMessageWithQuoteAsync($"已为所有群退订了{dbSubscribe.SubscribeType}[{dbSubscribe.SubscribeName}]~");
                SubscribeDatas.LoadSubscribeTask();
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "取消订阅失败");
            }
        }

        private async Task<int> CheckSubscribeIdAsync(string value)
        {
            int subscribeId = 0;
            if (int.TryParse(value, out subscribeId) == false)
            {
                throw new ProcessException("id必须为数字");
            }
            return await Task.FromResult(subscribeId);
        }

    }
}
