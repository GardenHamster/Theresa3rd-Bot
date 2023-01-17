using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Event
{
    public abstract class BaseEvent
    {

        /// <summary>
        /// 检查是否黑名单成员
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool CheckBanMemberAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            return BotConfig.BanMemberList.Where(o => o.KeyWord == args.Sender.Id.ToString()).Any();
        }

        /// <summary>
        /// 检查pixiv cookie是否已经过期
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<bool> CheckPixivCookieAvailableAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(BotConfig.WebsiteConfig.Pixiv.Cookie))
            {
                await session.SendGroupMessageWithAtAsync(args, new PlainMessage("缺少pixiv cookie，请设置cookie"));
                return false;
            }
            if (DateTime.Now > BotConfig.WebsiteConfig.Pixiv.CookieExpireDate)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.PixivConfig.CookieExpireMsg, "cookie过期了，让管理员更新cookie吧");
                return false;
            }
            if (BotConfig.WebsiteConfig.Pixiv.UserId <= 0)
            {
                await session.SendGroupMessageWithAtAsync(args, new PlainMessage("缺少userId，请更新cookie"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查订阅功能是否开启
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<bool> CheckSubscribeEnableAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, BaseSubscribeConfig subscribeConfig)
        {
            if (BotConfig.PermissionsConfig.SubscribeGroups.Contains(args.Sender.Group.Id) == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (subscribeConfig == null || subscribeConfig.Enable == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.DisableMsg, "该功能已关闭");
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
        public async Task<bool> CheckSetuEnableAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SetuGroups.Contains(args.Sender.Group.Id) == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (BotConfig.SetuConfig?.Pixiv == null || BotConfig.SetuConfig.Pixiv.Enable == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.DisableMsg, "该功能已关闭");
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
        public async Task<bool> CheckSaucenaoEnableAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SaucenaoGroups.Contains(args.Sender.Group.Id) == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (BotConfig.SaucenaoConfig == null || BotConfig.SaucenaoConfig.Enable == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.DisableMsg, "该功能已关闭");
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
        public async Task<bool> CheckSuperManagersAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SuperManagers.Contains(args.Sender.Id) == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.ManagersRequiredMsg, "该功能需要管理员执行");
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
        public async Task<bool> CheckSuperManagersAsync(IMiraiHttpSession session, IFriendMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SuperManagers.Contains(args.Sender.Id) == false)
            {
                await session.SendTemplateAsync(args, BotConfig.GeneralConfig.ManagersRequiredMsg, "该功能需要管理员执行");
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
        public async Task<bool> CheckMemberSetuCoolingAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(args.Sender.Id)) return false;
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(args.Sender.Group.Id)) return false;
            int cdSecond = CoolingCache.GetMemberSetuCooling(args.Sender.Group.Id, args.Sender.Id);
            if (cdSecond <= 0) return false;
            await session.SendGroupMessageWithAtAsync(args, new PlainMessage($" 功能冷却中，{cdSecond}秒后再来哦~"));
            return true;
        }

        /// <summary>
        /// 检查涩图功能是否开启
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckGroupSetuCoolingAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(args.Sender.Id)) return false;
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(args.Sender.Group.Id)) return false;
            int cdSecond = CoolingCache.GetGroupSetuCooling(args.Sender.Group.Id, args.Sender.Id);
            if (cdSecond <= 0) return false;
            await session.SendGroupMessageWithAtAsync(args, new PlainMessage($" 群功能冷却中，{cdSecond}秒后再来哦~"));
            return true;
        }

        /// <summary>
        /// 检查涩图功能可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSetuUseUpAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(args.Sender.Id)) return false;
            if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(args.Sender.Group.Id)) return false;
            if (BotConfig.SetuConfig.MaxDaily == 0) return false;
            int useCount = new RequestRecordBusiness().getUsedCountToday(args.Sender.Group.Id, args.Sender.Id, CommandType.Setu);
            if (useCount < BotConfig.SetuConfig.MaxDaily) return false;
            await session.SendGroupMessageWithAtAsync(args, new PlainMessage(" 你今天的使用次数已经达到上限了，明天再来吧"));
            return true;
        }

        /// <summary>
        /// 检查原图功能可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSaucenaoUseUpAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.SaucenaoConfig.MaxDaily == 0) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(args.Sender.Id)) return false;
            int useCount = new RequestRecordBusiness().getUsedCountToday(args.Sender.Group.Id, args.Sender.Id, CommandType.Saucenao);
            if (useCount < BotConfig.SaucenaoConfig.MaxDaily) return false;
            await session.SendGroupMessageWithAtAsync(args, new PlainMessage(" 你今天的使用次数已经达到上限了，明天再来吧"));
            return true;
        }

        /// <summary>
        /// 检查是否有涩图请求在处理中
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckHandingAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (CoolingCache.IsHanding(args.Sender.Group.Id, args.Sender.Id) == false) return false;
            await session.SendGroupMessageWithAtAsync(args, new PlainMessage(" 你的一个请求正在处理中，稍后再来吧"));
            return true;
        }

        /// <summary>
        /// 检查原图功能是否在冷却中
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckMemberSaucenaoCoolingAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(args.Sender.Group.Id)) return false;
            int cdSecond = CoolingCache.GetMemberSaucenaoCooling(args.Sender.Group.Id, args.Sender.Id);
            if (cdSecond <= 0) return false;
            await session.SendGroupMessageWithAtAsync(args, new PlainMessage($" 功能冷却中，{cdSecond}秒后再来哦~"));
            return true;
        }


    }
}
