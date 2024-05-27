using TheresaBot.Core.Cache;
using TheresaBot.Core.Command;
using TheresaBot.Core.Common;
using TheresaBot.Core.Datas;
using TheresaBot.Core.Drawer;
using TheresaBot.Core.Exceptions;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Content;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Services;
using TheresaBot.Core.Session;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Handler
{
    internal class SubscribeHandler : BaseHandler
    {
        private SubscribeService subscribeService;
        private SubscribeGroupService subscribeGroupService;

        public SubscribeHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            subscribeService = new SubscribeService();
            subscribeGroupService = new SubscribeGroupService();
        }

        public async Task ListSubscribeAsync(GroupCommand command)
        {
            try
            {
                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中
                var miyousheSubList = subscribeGroupService.GetSubscribeInfos(command.GroupId, SubscribeType.米游社用户);
                var pixivUserSubList = subscribeGroupService.GetSubscribeInfos(command.GroupId, SubscribeType.P站画师);
                var pixivTagSubList = subscribeGroupService.GetSubscribeInfos(command.GroupId, SubscribeType.P站标签).Select(o => o with { SubscribeCode = String.Empty }).ToList();
                var fullSavePath = FilePath.GetTempImgSavePath();
                using var drawer = new SubscribeDrawer();
                FileInfo fileInfo = drawer.DrawSubscribe(miyousheSubList, pixivUserSubList, pixivTagSubList, fullSavePath);
                var sendContents = new List<BaseContent>
                {
                    new PlainContent("本群的订阅内容如下："),
                    new LocalImageContent(fileInfo)
                };
                await command.ReplyGroupMessageWithQuoteAsync(sendContents);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "订阅列表查询异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        public async Task DeleteSubscribeAsync(GroupCommand command)
        {
            try
            {
                var subscribeIds = new int[0];
                if (command.KeyWord.Length > 0)
                {
                    subscribeIds = await CheckSubscribeIdsAsync(command.KeyWord);
                }
                else
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var idStep = processInfo.CreateStep("请在60秒内发送要退订的ID，多个ID之间可以用逗号或者换行隔开，订阅ID可以通过订阅列表指令获取", CheckSubscribeIdsAsync);
                    await processInfo.StartProcessing();
                    subscribeIds = idStep.Answer;
                }

                foreach (var subscribeId in subscribeIds)
                {
                    await DeleteSubscribeAsync(command, subscribeId);
                }

                SubscribeDatas.LoadSubscribeTask();
                await command.ReplyGroupMessageWithQuoteAsync($"退订完毕~");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "订阅取消失败");
            }
        }

        private async Task DeleteSubscribeAsync(GroupCommand command, int subscribeId)
        {
            try
            {
                var subscribe = subscribeService.GetSubscribe(subscribeId);
                if (subscribe is null)
                {
                    await command.ReplyGroupMessageWithQuoteAsync($"订阅ID【{subscribeId}】退订失败，ID不存在");
                    await Task.Delay(1000);
                }
                else
                {
                    subscribeGroupService.DeleteBySubscribeId(subscribeId);
                }
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, $"订阅ID【{subscribeId}】退订失败");
                await Task.Delay(1000);
            }
        }

        private async Task<int[]> CheckSubscribeIdsAsync(string value)
        {
            var ids = new List<int>();
            var splitArr = value.SplitParams();
            if (splitArr.Length == 0)
            {
                throw new ProcessException("没有检测到用户id");
            }
            foreach (var idStr in splitArr)
            {
                ids.Add(await CheckSubscribeIdAsync(idStr));
            }
            return await Task.FromResult(ids.ToArray());
        }

        private async Task<int> CheckSubscribeIdAsync(string value)
        {
            int id = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ProcessException("没有检测到订阅ID");
            }
            if (int.TryParse(value, out id) == false)
            {
                throw new ProcessException($"订阅ID{value}必须为数字");
            }
            if (id <= 0)
            {
                throw new ProcessException($"订阅ID{value}无效");
            }
            return await Task.FromResult(id);
        }



    }
}
