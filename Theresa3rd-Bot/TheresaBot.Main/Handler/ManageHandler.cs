using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Process;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class ManageHandler : BaseHandler
    {
        private BanTagService banTagService;
        private BanMemberService banMemberService;

        public ManageHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            banTagService = new BanTagService();
            banMemberService = new BanMemberService();
        }

        public async Task DisableTagAsync(GroupCommand command)
        {
            try
            {
                var tagStr = string.Empty;
                var matchType = TagMatchType.Contain;
                if (command.Params.Length >= 2)
                {
                    tagStr = command.Params[0];
                    matchType = await CheckMatchTypeAsync(command.Params[1]);
                }
                else
                {
                    ProcessInfo processInfo = ProcessCache.CreateProcess(command);
                    StepInfo tagStep = processInfo.CreateStep("请在60秒内发送需要屏蔽的标签，多个标签之间用逗号或者换行隔开", CheckTextAsync);
                    StepInfo matchStep = processInfo.CreateStep($"请在60秒内发送数字选择标签匹配方式：\r\n{EnumHelper.TagMatchOptions.JoinToString()}", CheckMatchTypeAsync);
                    await processInfo.StartProcessing();
                    tagStr = tagStep.AnswerForString();
                    matchType = matchStep.AnswerForEnum<TagMatchType>();
                }
                var result = new ModifyResult();
                var banTags = tagStr.SplitParams();
                banTagService.InsertOrUpdate(result, banTags, matchType);
                BanTagDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync($"记录成功，新增记录{result.CreateCount}条，更新记录{result.UpdateCount}条");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "屏蔽标签异常");
            }
        }

        public async Task EnableTagAsync(GroupCommand command)
        {
            try
            {
                var tagStr = command.KeyWord;
                if (string.IsNullOrWhiteSpace(tagStr))
                {
                    await command.ReplyGroupMessageWithAtAsync("没有检测到要解除屏蔽的标签，请确保指令格式正确");
                    return;
                }
                var banTags = tagStr.SplitParams();
                banTagService.DelBanTags(banTags);
                BanTagDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync("记录成功");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "解除标签屏蔽异常");
            }
        }

        public async Task DisableMemberAsync(GroupCommand command)
        {
            try
            {
                string memberCode = command.KeyWord;
                if (string.IsNullOrWhiteSpace(memberCode))
                {
                    await command.ReplyGroupMessageWithAtAsync("没有检测到要屏蔽的QQ号，请确保指令格式正确");
                    return;
                }
                long memberId = 0;
                if (long.TryParse(memberCode, out memberId) == false || memberId <= 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("QQ号格式不正确");
                    return;
                }
                if (BotConfig.PermissionsConfig.SuperManagers.Contains(memberId))
                {
                    await command.ReplyGroupMessageWithAtAsync("无法拉黑超级管理员");
                    return;
                }
                banMemberService.insertBanMembers(memberId);
                BanMemberDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync("记录成功");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "屏蔽成员异常");
            }
        }

        public async Task EnableMemberAsync(GroupCommand command)
        {
            try
            {
                string memberCode = command.KeyWord;
                if (string.IsNullOrWhiteSpace(memberCode))
                {
                    await command.ReplyGroupMessageWithAtAsync("没有检测到要解除屏蔽的QQ号，请确保指令格式正确");
                    return;
                }
                long memberId = 0;
                if (long.TryParse(memberCode, out memberId) == false || memberId <= 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("QQ号格式不正确");
                    return;
                }
                banMemberService.DelBanMember(memberId);
                BanMemberDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync("解除成功");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "解除成员屏蔽异常");
            }
        }

        private async Task<TagMatchType> CheckMatchTypeAsync(string value)
        {
            int typeId = 0;
            if (int.TryParse(value, out typeId) == false)
            {
                throw new ProcessException("匹配方式不在范围内");
            }
            if (Enum.IsDefined(typeof(TagMatchType), typeId) == false)
            {
                throw new ProcessException("匹配方式不在范围内");
            }
            return await Task.FromResult((TagMatchType)typeId);
        }

    }
}
