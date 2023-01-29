using System;
using System.Threading.Tasks;
using TheresaBot.Main.BotPlatform.Base.Command;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    public abstract class BaseHandler
    {
        private RequestRecordBusiness requestRecordBusiness;

        public BaseHandler()
        {
            this.requestRecordBusiness = new RequestRecordBusiness();
        }

        public async Task<int> getUsedCountToday(long groupId, long memberId, params CommandType[] commandTypeArr)
        {
            return await Task.FromResult(requestRecordBusiness.getUsedCountToday(groupId, memberId, commandTypeArr));
        }

        public async Task<RequestRecordPO> addRecord(GroupCommand botCommand)
        {
            return await Task.FromResult(requestRecordBusiness.addRecord(botCommand.GroupId, botCommand.MemberId, botCommand.CommandType, botCommand.Instruction));
        }

        public async Task<RequestRecordPO> addRecord(FriendCommand botCommand)
        {
            return await Task.FromResult(requestRecordBusiness.addRecord(0, botCommand.MemberId, botCommand.CommandType, botCommand.Instruction));
        }

        /// <summary>
        /// 检查pixiv cookie是否已经过期
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<bool> CheckPixivCookieAvailableAsync(GroupCommand command)
        {
            if (string.IsNullOrWhiteSpace(BotConfig.WebsiteConfig.Pixiv.Cookie))
            {
                await command.ReplyGroupMessageWithAtAsync("缺少pixiv cookie，请设置cookie");
                return false;
            }
            if (DateTime.Now > BotConfig.WebsiteConfig.Pixiv.CookieExpireDate)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.PixivConfig.CookieExpireMsg, "cookie过期了，让管理员更新cookie吧");
                return false;
            }
            if (BotConfig.WebsiteConfig.Pixiv.UserId <= 0)
            {
                await command.ReplyGroupMessageWithAtAsync("缺少userId，请更新cookie");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查订阅功能是否开启
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<bool> CheckSubscribeEnableAsync(GroupCommand command, BaseSubscribeConfig subscribeConfig)
        {
            if (BotConfig.PermissionsConfig.SubscribeGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (subscribeConfig is null || subscribeConfig.Enable == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.DisableMsg, "该功能已关闭");
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
        public async Task<bool> CheckSetuEnableAsync(GroupCommand command, BasePluginConfig pluginConfig)
        {
            if (BotConfig.PermissionsConfig.SetuGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (pluginConfig is null || pluginConfig.Enable == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.SetuConfig.DisableMsg, "该功能已关闭");
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
        public async Task<bool> CheckSaucenaoEnableAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SaucenaoGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (BotConfig.SaucenaoConfig is null || BotConfig.SaucenaoConfig.Enable == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.SaucenaoConfig.DisableMsg, "该功能已关闭");
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
        public async Task<bool> CheckSuperManagersAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SuperManagers.Contains(command.MemberId) == false)
            {
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.ManagersRequiredMsg, "该功能需要管理员执行");
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
        public async Task<bool> CheckSuperManagersAsync(FriendCommand command)
        {
            if (BotConfig.PermissionsConfig.SuperManagers.Contains(command.MemberId) == false)
            {
                await command.ReplyFriendTemplateAsync(BotConfig.GeneralConfig.ManagersRequiredMsg, "该功能需要管理员执行");
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
        public async Task<bool> CheckMemberSetuCoolingAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int cdSecond = CoolingCache.GetMemberSetuCooling(command.GroupId, command.MemberId);
            if (cdSecond <= 0) return false;
            await command.ReplyGroupMessageWithAtAsync($"功能冷却中，{cdSecond}秒后再来哦~");
            return true;
        }

        /// <summary>
        /// 检查涩图功能是否开启
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckGroupSetuCoolingAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int cdSecond = CoolingCache.GetGroupSetuCooling(command.GroupId, command.MemberId);
            if (cdSecond <= 0) return false;
            await command.ReplyGroupMessageWithAtAsync($"群功能冷却中，{cdSecond}秒后再来哦~");
            return true;
        }

        /// <summary>
        /// 检查涩图功能可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSetuUseUpAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            if (BotConfig.SetuConfig.MaxDaily == 0) return false;
            int useCount = new RequestRecordBusiness().getUsedCountToday(command.GroupId, command.MemberId, CommandType.Setu);
            if (useCount < BotConfig.SetuConfig.MaxDaily) return false;
            await command.ReplyGroupMessageWithAtAsync("你今天的使用次数已经达到上限了，明天再来吧");
            return true;
        }

        /// <summary>
        /// 检查原图功能可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSaucenaoUseUpAsync(GroupCommand command)
        {
            if (BotConfig.SaucenaoConfig.MaxDaily == 0) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int useCount = new RequestRecordBusiness().getUsedCountToday(command.GroupId, command.MemberId, CommandType.Saucenao);
            if (useCount < BotConfig.SaucenaoConfig.MaxDaily) return false;
            await command.ReplyGroupMessageWithAtAsync("你今天的使用次数已经达到上限了，明天再来吧");
            return true;
        }

        /// <summary>
        /// 检查原图功能是否在冷却中
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckMemberSaucenaoCoolingAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int cdSecond = CoolingCache.GetMemberSaucenaoCooling(command.GroupId, command.MemberId);
            if (cdSecond <= 0) return false;
            await command.ReplyGroupMessageWithAtAsync($"功能冷却中，{cdSecond}秒后再来哦~");
            return true;
        }

        /// <summary>
        /// 检查是否有涩图请求在处理中
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckHandingAsync(GroupCommand command)
        {
            if (CoolingCache.IsHanding(command.GroupId, command.MemberId) == false) return false;
            await command.ReplyGroupMessageWithAtAsync("你的一个请求正在处理中，稍后再来吧");
            return true;
        }
    }
}
