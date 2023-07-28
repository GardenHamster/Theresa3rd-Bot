using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Drawer;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Step;
using TheresaBot.Main.Relay;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class SubscribeHandler : BaseHandler
    {
        private SubscribeBusiness subscribeBusiness;

        public SubscribeHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            subscribeBusiness = new SubscribeBusiness();
        }

        /// <summary>
        /// 发送订阅列表
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task listSubscribeAsync(GroupCommand command)
        {
            try
            {
                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中
                var miyousheSubList = subscribeBusiness.getSubscribes(command.GroupId, SubscribeType.米游社用户);
                var pixivUserSubList = subscribeBusiness.getSubscribes(command.GroupId, SubscribeType.P站画师);
                var pixivTagSubList = subscribeBusiness.getSubscribes(command.GroupId, SubscribeType.P站标签).Select(o => o with { SubscribeCode = String.Empty }).ToList();
                var drawTagList = pixivTagSubList.Select(o => o with { SubscribeCode = String.Empty });
                string fullSavePath = FilePath.GetTempImgSavePath();
                FileInfo fileInfo = new SubscribeDrawer().DrawSubscribe(miyousheSubList, pixivUserSubList, pixivTagSubList, fullSavePath);
                List<BaseContent> sendContents = new List<BaseContent>();
                sendContents.Add(new PlainContent("当前群已订阅内容如下"));
                sendContents.Add(new LocalImageContent(fileInfo));
                await command.ReplyGroupMessageWithQuoteAsync(sendContents);
            }
            catch (Exception ex)
            {
                string errMsg = $"查询订阅失败";
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


        /// <summary>
        /// 取消一个订阅
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task cancleSubscribeAsync(GroupCommand command)
        {
            try
            {
                string subscribeIdStr = command.KeyWord;
                if (string.IsNullOrWhiteSpace(subscribeIdStr))
                {
                    ProcessInfo stepInfo = await ProcessCache.CreateProcessAsync(command);
                    if (stepInfo is null) return;
                    StepInfo tagStep = new StepDetail(60, "请在60秒内发送要退订的Id", CheckSubscribeIdAsync);
                    stepInfo.AddStep(tagStep);
                    if (await stepInfo.StartProcessing() == false) return;
                    subscribeIdStr = tagStep.Answer;
                }
                else
                {
                    if (await CheckSubscribeIdAsync(command, subscribeIdStr) == false) return;
                }

                int subscribeId = Convert.ToInt32(subscribeIdStr);
                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(subscribeId);
                if (dbSubscribe is null)
                {
                    await command.ReplyGroupMessageWithQuoteAsync($"退订失败，订阅Id{subscribeId}不存在");
                    return;
                }

                subscribeBusiness.cancleSubscribe(dbSubscribe.Id);
                await command.ReplyGroupMessageWithQuoteAsync($"已为所有群退订了{dbSubscribe.SubscribeType}[{dbSubscribe.SubscribeName}]~");
                SubscribeDatas.LoadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"取消订阅失败";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
            }
        }

        private async Task<bool> CheckSubscribeIdAsync(GroupCommand command, GroupRelay relay)
        {
            return await CheckSubscribeIdAsync(command, relay.Answer);
        }

        private async Task<bool> CheckSubscribeIdAsync(GroupCommand command, string value)
        {
            int id = 0;
            if (int.TryParse(value, out id) == false)
            {
                await command.ReplyGroupMessageWithQuoteAsync("id必须为数字");
                return false;
            }
            return true;
        }

    }
}
