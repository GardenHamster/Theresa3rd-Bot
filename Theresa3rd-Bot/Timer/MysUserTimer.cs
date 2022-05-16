using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
{
    public static class MysUserTimer
    {
        private static System.Timers.Timer timer;

        public static void init()
        {
            timer = new System.Timers.Timer();
            timer.Interval = BotConfig.SubscribeConfig.Mihoyo.ScanInterval * 1000;
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandlerMethod);
            timer.Enabled = true;
        }

        private static void HandlerMethod(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                timer.Enabled = false;
                SubscribeMethodAsync(source, e).Wait();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "MysUserTimer.HandlerMethod方法异常");
            }
            finally
            {
                timer.Enabled = true;
            }
        }

        private static async Task SubscribeMethodAsync(object source, System.Timers.ElapsedEventArgs e)
        {
            MYSBusiness mysBusiness = new MYSBusiness();
            SubscribeType subscribeType = SubscribeType.米游社用户;
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType];
            if (subscribeTaskList == null || subscribeTaskList.Count == 0) return;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    SubscribeInfo subInfo = subscribeTask.SubscribeInfo;
                    List<MysSubscribe> mysSubscribeList = mysBusiness.getMysUserSubscribeAsync(subInfo.SubscribeSubType, subInfo.SubscribeId, subInfo.SubscribeCode).Result;
                    if (mysSubscribeList == null || mysSubscribeList.Count == 0) continue;
                    await sendGroupSubscribeAsync(mysBusiness, subscribeTask, mysSubscribeList);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"获取米游社[{subscribeTask.SubscribeInfo.SubscribeCode}]订阅失败");
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
        }

        private static async Task sendGroupSubscribeAsync(MYSBusiness mysBusiness, SubscribeTask subscribeTask, List<MysSubscribe> mysSubscribeList)
        {
            foreach (MysSubscribe mysSubscribe in mysSubscribeList)
            {
                List<IChatMessage> chailList = mysBusiness.getSubscribeInfoAsync(mysSubscribe, BotConfig.SubscribeConfig.Mihoyo.Template).Result;
                foreach (long groupId in subscribeTask.GroupIdList)
                {
                    await MiraiHelper.Session.SendGroupMessageAsync(groupId, chailList.ToArray());
                }
            }
        }



    }
}
