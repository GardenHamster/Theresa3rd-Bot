using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
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
        private BanPixiverService banPixiverService;

        public ManageHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            banTagService = new BanTagService();
            banMemberService = new BanMemberService();
            banPixiverService = new BanPixiverService();
        }

        public async Task DisableTagAsync(GroupCommand command)
        {
            try
            {
                var banTags = new string[0];
                var matchType = TagMatchType.Contain;
                if (command.Params.Length == 2)
                {
                    banTags = await CheckBanTagsAsync(command.Params[0]);
                    matchType = await CheckMatchTypeAsync(command.Params[1]);
                }
                else
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var tagStep = processInfo.CreateStep("请在60秒内发送需要屏蔽的标签，多个标签之间用逗号或者换行隔开", WaitParamsAsync);
                    var matchStep = processInfo.CreateStep($"请在60秒内发送数字选择标签匹配方式：\r\n{EnumHelper.TagMatchOptions.JoinToString()}", CheckMatchTypeAsync);
                    await processInfo.StartProcessing();
                    banTags = tagStep.Answer;
                    matchType = matchStep.Answer;
                }
                banTagService.InsertOrUpdate(banTags, matchType);
                BanTagDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync($"记录成功~");
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
                long memberId = await CheckMemberIdAsync(command.KeyWord);
                if (memberId.IsSuperManager())
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
                long memberId = await CheckMemberIdAsync(command.KeyWord);
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

        public async Task DisablePixiverAsync(GroupCommand command)
        {
            try
            {
                long pixiverId = await CheckPixiverIdAsync(command.KeyWord);
                banPixiverService.insertBanPixivers(pixiverId);
                BanPixiverDatas.LoadDatas();
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

        public async Task EnablePixiverAsync(GroupCommand command)
        {
            try
            {
                long pixiverId = await CheckPixiverIdAsync(command.KeyWord);
                banPixiverService.DelBanPixiver(pixiverId);
                BanPixiverDatas.LoadDatas();
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


        private async Task<string[]> CheckBanTagsAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ProcessException("没有检测到需要屏蔽的标签");
            }
            var banTags = value.SplitParams();
            if (banTags.Length == 0)
            {
                throw new ProcessException("没有检测到需要屏蔽的标签");
            }
            return await Task.FromResult(banTags);
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

        private async Task<long> CheckMemberIdAsync(string value)
        {
            long memberId = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ProcessException("没有检测到要解除屏蔽的QQ号");
            }
            if (long.TryParse(value, out memberId) == false || memberId <= 1000)
            {
                throw new ProcessException("QQ号格式错误");
            }
            return await Task.FromResult(memberId);
        }

        private async Task<long> CheckPixiverIdAsync(string value)
        {
            long pixiverId = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ProcessException("没有检测到要解除屏蔽的画师ID");
            }
            if (long.TryParse(value, out pixiverId) == false || pixiverId <= 10)
            {
                throw new ProcessException("画师ID格式错误");
            }
            return await Task.FromResult(pixiverId);
        }

    }
}
