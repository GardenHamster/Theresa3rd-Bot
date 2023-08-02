using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Relay;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal abstract class BaseHandler
    {
        protected BaseSession Session;
        protected BaseReporter Reporter;
        protected RequestRecordBusiness requestRecordBusiness;

        public BaseHandler(BaseSession session, BaseReporter reporter)
        {
            this.Session = session;
            this.Reporter = reporter;
            this.requestRecordBusiness = new RequestRecordBusiness();
        }

        public async Task LogAndReplyError(GroupCommand command, Exception ex, string message = "")
        {
            LogHelper.Error(ex, message);
            await command.ReplyError(ex, message);
            await Task.Delay(1000);
            await Reporter.SendError(ex, message);
        }

        public async Task LogAndReportError(Exception ex, string message = "")
        {
            LogHelper.Error(ex, message);
            await Reporter.SendError(ex, message);
        }

        public async Task<int> GetUsedCountToday(long groupId, long memberId, params CommandType[] commandTypeArr)
        {
            return await Task.FromResult(requestRecordBusiness.getUsedCountToday(groupId, memberId, commandTypeArr));
        }

        public async Task<RequestRecordPO> InsertRecord(GroupCommand command)
        {
            return await Task.FromResult(requestRecordBusiness.addRecord(command.GroupId, command.MemberId, command.CommandType, command.Instruction));
        }

        public async Task<RequestRecordPO> InsertRecord(FriendCommand command)
        {
            return await Task.FromResult(requestRecordBusiness.addRecord(0, command.MemberId, command.CommandType, command.Instruction));
        }

        public async Task<bool> CheckPixivCookieAvailableAsync(GroupCommand command)
        {
            if (string.IsNullOrWhiteSpace(WebsiteDatas.Pixiv.Cookie))
            {
                await command.ReplyGroupMessageWithQuoteAsync("缺少pixiv cookie，请设置cookie");
                return false;
            }
            if (DateTime.Now > WebsiteDatas.Pixiv.CookieExpireDate)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.PixivConfig.CookieExpireMsg, "cookie过期了，让管理员更新cookie吧");
                return false;
            }
            if (WebsiteDatas.Pixiv.UserId <= 0)
            {
                await command.ReplyGroupMessageWithQuoteAsync("缺少userId，请更新cookie");
                return false;
            }
            return true;
        }

        public async Task<bool> CheckSubscribeEnableAsync(GroupCommand command, BaseSubscribeConfig subscribeConfig)
        {
            if (BotConfig.PermissionsConfig.SubscribeGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (subscribeConfig is null || subscribeConfig.Enable == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.GeneralConfig.DisableMsg, "该功能已关闭");
                return false;
            }
            return true;
        }

        public async Task<bool> CheckSetuEnableAsync(GroupCommand command, BasePluginConfig pluginConfig)
        {
            if (BotConfig.PermissionsConfig.SetuGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (pluginConfig is null || pluginConfig.Enable == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.GeneralConfig.DisableMsg, "该功能已关闭");
                return false;
            }
            return true;
        }

        public async Task<bool> CheckR18ImgEnableAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuShowR18Groups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync("当前群未配置R18权限");
                return false;
            }
            if (BotConfig.PermissionsConfig.SetuShowR18ImgGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync("当前群未配置R18图片权限");
                return false;
            }
            return true;
        }

        public async Task<bool> CheckSaucenaoEnableAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SaucenaoGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (BotConfig.SaucenaoConfig is null || BotConfig.SaucenaoConfig.Enable == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.GeneralConfig.DisableMsg, "该功能已关闭");
                return false;
            }
            return true;
        }

        public async Task<bool> CheckPixivRankingEnableAsync(GroupCommand command, PixivRankingItem rankingItem)
        {
            if (BotConfig.PermissionsConfig.PixivRankingGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (rankingItem is null || rankingItem.Enable == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.GeneralConfig.DisableMsg, "该功能已关闭");
                return false;
            }
            return true;
        }

        public async Task<bool> CheckWordCloudEnableAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.WordCloudGroups.Contains(command.GroupId) == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (BotConfig.WordCloudConfig is null || BotConfig.WordCloudConfig.Enable == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.GeneralConfig.DisableMsg, "该功能已关闭");
                return false;
            }
            return true;
        }

        public async Task<bool> CheckSuperManagersAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SuperManagers.Contains(command.MemberId) == false)
            {
                await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.GeneralConfig.ManagersRequiredMsg, "该功能需要管理员执行");
                return false;
            }
            return true;
        }

        public async Task<bool> CheckSuperManagersAsync(FriendCommand command)
        {
            if (BotConfig.PermissionsConfig.SuperManagers.Contains(command.MemberId) == false)
            {
                await command.ReplyFriendTemplateAsync(BotConfig.GeneralConfig.ManagersRequiredMsg, "该功能需要管理员执行");
                return false;
            }
            return true;
        }

        public async Task<bool> CheckMemberSetuCoolingAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int cdSecond = CoolingCache.GetMemberSetuCD(command.GroupId, command.MemberId);
            if (cdSecond <= 0) return false;
            await command.ReplyGroupMessageWithQuoteAsync($"功能冷却中，{cdSecond}秒后再来哦~");
            return true;
        }

        public async Task<bool> CheckGroupSetuCoolingAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int cdSecond = CoolingCache.GetGroupSetuCD(command.GroupId);
            if (cdSecond <= 0) return false;
            await command.ReplyGroupMessageWithQuoteAsync($"群功能冷却中，{cdSecond}秒后再来哦~");
            return true;
        }

        public async Task<bool> CheckGroupRankingCoolingAsync(GroupCommand command, PixivRankingType rankingType)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int cdSecond = CoolingCache.GetGroupPixivRankingCD(rankingType, command.GroupId);
            if (cdSecond <= 0) return false;
            await command.ReplyGroupMessageWithQuoteAsync($"群功能冷却中，{cdSecond}秒后再来哦~");
            return true;
        }

        public async Task<bool> CheckGroupWordCloudCoolingAsync(GroupCommand command)
        {
            int cdSecond = CoolingCache.GetGroupWordCloudCD(command.GroupId);
            if (cdSecond <= 0) return false;
            await command.ReplyGroupMessageWithQuoteAsync($"群功能冷却中，{cdSecond}秒后再来哦~");
            return true;
        }

        public async Task<bool> CheckSetuUseUpAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            if (BotConfig.SetuConfig.MaxDaily == 0) return false;
            int useCount = new RequestRecordBusiness().getUsedCountToday(command.GroupId, command.MemberId, CommandType.Setu);
            if (useCount < BotConfig.SetuConfig.MaxDaily) return false;
            await command.ReplyGroupMessageWithQuoteAsync("你今天的使用次数已经达到上限了，明天再来吧");
            return true;
        }

        public async Task<bool> CheckSaucenaoUseUpAsync(GroupCommand command)
        {
            if (BotConfig.SaucenaoConfig.MaxDaily == 0) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int useCount = new RequestRecordBusiness().getUsedCountToday(command.GroupId, command.MemberId, CommandType.Saucenao);
            if (useCount < BotConfig.SaucenaoConfig.MaxDaily) return false;
            await command.ReplyGroupMessageWithQuoteAsync("你今天的使用次数已经达到上限了，明天再来吧");
            return true;
        }

        public async Task<bool> CheckMemberSaucenaoCoolingAsync(GroupCommand command)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(command.GroupId)) return false;
            if (BotConfig.PermissionsConfig.LimitlessMembers.Contains(command.MemberId)) return false;
            int cdSecond = CoolingCache.GetMemberSaucenaoCD(command.GroupId, command.MemberId);
            if (cdSecond <= 0) return false;
            await command.ReplyGroupMessageWithQuoteAsync($"功能冷却中，{cdSecond}秒后再来哦~");
            return true;
        }

        public async Task<bool> CheckHandingAsync(GroupCommand command)
        {
            if (CoolingCache.IsHanding(command.GroupId, command.MemberId) == false) return false;
            await command.ReplyGroupMessageWithQuoteAsync("你的一个请求正在处理中，稍后再来吧");
            return true;
        }

        public async Task<bool> CheckPixivRankingHandingAsync(GroupCommand command)
        {
            if (CoolingCache.IsPixivRankingHanding() == false) return false;
            await command.ReplyGroupMessageWithQuoteAsync("一个日榜功能正在处理中，稍后再来吧");
            return true;
        }

        public async Task<bool> CheckWordCloudHandingAsync(GroupCommand command)
        {
            if (CoolingCache.IsWordCloudHanding(command.GroupId) == false) return false;
            await command.ReplyGroupMessageWithQuoteAsync("一个词云功能正在处理中，稍后再来吧");
            return true;
        }

        protected async Task CheckTextAsync(GroupRelay relay)
        {
            if (string.IsNullOrWhiteSpace(relay.Answer)) throw new NoAnswerException();
            await Task.CompletedTask;
        }

        protected async Task CheckImageAsync(GroupRelay relay)
        {
            List<string> imgList = relay.GetImageUrls();
            if (imgList.Count == 0) throw new NoAnswerException();
            await Task.CompletedTask;
        }

    }
}
