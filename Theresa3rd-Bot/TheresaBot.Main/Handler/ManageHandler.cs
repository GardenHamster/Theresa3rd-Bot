using AngleSharp.Text;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Model.Step;
using TheresaBot.Main.Relay;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;
using YamlDotNet.Core.Tokens;

namespace TheresaBot.Main.Handler
{
    internal class ManageHandler : BaseHandler
    {
        private BanTagBusiness banTagBusiness;
        private BanMemberBusiness banMemberBusiness;

        public ManageHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            banTagBusiness = new BanTagBusiness();
            banMemberBusiness = new BanMemberBusiness();
        }

        public async Task disableTagAsync(GroupCommand command)
        {
            try
            {
                string tagStr = string.Empty;
                TagMatchType matchType = TagMatchType.Contain;
                if (command.Params.Length >= 2)
                {
                    tagStr = command.Params[0];
                    await CheckMatchTypeAsync(command.Params[1]);
                    matchType = (TagMatchType)Convert.ToInt32(command.Params[1]);
                }
                else
                {
                    ProcessInfo process = ProcessCache.CreateProcessAsync(command);
                    StepInfo tagStep = process.CreateStep("请在60秒内发送需要屏蔽的标签，多个标签之间用逗号或者换行隔开");
                    StepInfo matchStep = process.CreateStep($"请在60秒内发送数字选择标签匹配方式：\r\n{EnumHelper.TagMatchTypeOption()}", CheckMatchTypeAsync);
                    await process.StartProcessing();
                    tagStr = tagStep.AnswerForString();
                    matchType = matchStep.AnswerForEnum<TagMatchType>();
                }
                var result = new ModifyResult();
                var banTags = tagStr.splitParams();
                banTagBusiness.InsertOrUpdate(result, tagStr, matchType);
                BanTagDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync($"记录成功，新增记录{result.CreateCount}条，更新记录{result.UpdateCount}条");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
                return;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "disableTagAsync异常");
                await Reporter.SendError(ex, "disableTagAsync异常");
            }
        }

        public async Task enableTagAsync(GroupCommand command)
        {
            try
            {
                string tagStr = command.KeyWord;
                if (string.IsNullOrEmpty(tagStr))
                {
                    await command.ReplyGroupMessageWithQuoteAsync("没有检测到要解除屏蔽的标签，请确保指令格式正确");
                    return;
                }
                var banTags = tagStr.splitParams();
                int count = banTagBusiness.DelBanTags(banTags);
                BanTagDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync("记录成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "enableTagAsync异常");
                await Reporter.SendError(ex, "enableTagAsync异常");
            }
        }

        public async Task disableMemberAsync(GroupCommand command)
        {
            try
            {
                string memberCode = command.KeyWord;
                if (string.IsNullOrEmpty(memberCode))
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
                banMemberBusiness.insertBanMembers(memberId);
                BanMemberDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync("记录成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "disableMemberAsync异常");
                await Reporter.SendError(ex, "disableMemberAsync异常");
            }
        }

        public async Task enableMemberAsync(GroupCommand command)
        {
            try
            {
                string memberCode = command.KeyWord;
                if (string.IsNullOrEmpty(memberCode))
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
                banMemberBusiness.DelBanMember(memberId);
                BanMemberDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync("解除成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "enableMemberAsync异常");
                await Reporter.SendError(ex, "enableMemberAsync异常");
            }
        }

        private async Task CheckMatchTypeAsync(string value)
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
            await Task.CompletedTask;
        }

    }
}
