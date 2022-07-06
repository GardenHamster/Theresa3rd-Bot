using Mirai.CSharp.HttpApi.Models.ChatMessages;
using System;
using System.Collections.Generic;
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
                MYSBusiness mysBusiness = new MYSBusiness();
                SubscribeMethodAsync(mysBusiness).Wait();
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

        private static async Task SubscribeMethodAsync(MYSBusiness mysBusiness)
        {
            SubscribeType subscribeType = SubscribeType.米游社用户;
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType];
            if (subscribeTaskList == null || subscribeTaskList.Count == 0) return;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    List<MysSubscribe> mysSubscribeList = await mysBusiness.getMysUserSubscribeAsync(subscribeTask);
                    if (mysSubscribeList == null || mysSubscribeList.Count == 0) continue;
                    await sendGroupSubscribeAsync(mysBusiness, subscribeTask, mysSubscribeList);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"获取米游社[{subscribeTask.SubscribeCode}]订阅失败");
                }
                finally
                {
                    await Task.Delay(3000);
                }
            }
        }

        private static async Task sendGroupSubscribeAsync(MYSBusiness mysBusiness, SubscribeTask subscribeTask, List<MysSubscribe> mysSubscribeList)
        {
            foreach (MysSubscribe mysSubscribe in mysSubscribeList)
            {
                if (subscribeTask.GroupIdList == null || subscribeTask.GroupIdList.Count == 0) continue;
                List<IChatMessage> chailList = await mysBusiness.getSubscribeInfoAsync(mysSubscribe, BotConfig.SubscribeConfig.Mihoyo.Template);
                foreach (long groupId in subscribeTask.GroupIdList)
                {
                    try
                    {
                        await MiraiHelper.Session.SendGroupMessageAsync(groupId, chailList.ToArray());
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, "米游社订阅消息发送失败");
                    }
                    finally
                    {
                        await Task.Delay(1000);
                    }
                }
            }
        }



    }
}
