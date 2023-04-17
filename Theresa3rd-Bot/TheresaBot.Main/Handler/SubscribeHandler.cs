using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Step;
using TheresaBot.Main.Relay;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    internal class SubscribeHandler: BaseHandler
    {
        private SubscribeBusiness subscribeBusiness;

        public SubscribeHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            subscribeBusiness = new SubscribeBusiness();
        }


        /// <summary>
        /// 取消订阅pixiv标签
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
                    StepInfo stepInfo = await StepCache.CreateStepAsync(command);
                    if (stepInfo is null) return;
                    StepDetail tagStep = new StepDetail(60, "请在60秒内发送要退订的Id", CheckSubscribeIdAsync);
                    stepInfo.AddStep(tagStep);
                    if (await stepInfo.HandleStep() == false) return;
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
                    await command.ReplyGroupMessageWithAtAsync($"退订失败，订阅Id{subscribeId}不存在");
                    return;
                }

                subscribeBusiness.delSubscribeGroup(dbSubscribe.Id);
                await command.ReplyGroupMessageWithAtAsync($"已为所有群退订了{dbSubscribe.SubscribeType}[{dbSubscribe.SubscribeName}]~");
                ConfigHelper.LoadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"取消订阅失败";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, errMsg);
            }
        }

        private async Task<bool> CheckSubscribeIdAsync(GroupCommand command, GroupRelay relay)
        {
            return await CheckSubscribeIdAsync(command, relay.Message);
        }

        private async Task<bool> CheckSubscribeIdAsync(GroupCommand command, string value)
        {
            int id = 0;
            if (int.TryParse(value, out id) == false)
            {
                await command.ReplyGroupMessageWithAtAsync("id必须为数字");
                return false;
            }
            return true;
        }

    }
}
