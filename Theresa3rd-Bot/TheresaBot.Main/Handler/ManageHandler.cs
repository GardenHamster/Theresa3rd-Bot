using TheresaBot.Main.Business;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

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
                object banTypeObj = TagBanType.Contain;

                if (command.Params.Length > 0)
                {
                    tagStr = command.Params[0];
                }
                if (command.Params.Length > 1)
                {
                    banTypeObj = command.Params[1].ToEnum<TagBanType>();
                }
                if (string.IsNullOrEmpty(tagStr))
                {
                    await command.ReplyGroupMessageWithAtAsync("没有检测到需要屏蔽的标签，请确保指令格式正确");
                    return;
                }
                if (banTypeObj is null)
                {
                    await command.ReplyGroupMessageWithAtAsync("不存在的屏蔽类型，请确保指令格式正确");
                    return;
                }
                var banType = (TagBanType)banTypeObj;
                var banTag = banTagBusiness.getBanTag(tagStr);
                if (banTag is not null)
                {
                    await command.ReplyGroupMessageWithAtAsync("该标签已有记录了");
                    return;
                }
                banTagBusiness.AddBanTag(tagStr, banType);
                BanTagDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync("记录成功");
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
                    await command.ReplyGroupMessageWithAtAsync("没有检测到要解除屏蔽的标签，请确保指令格式正确");
                    return;
                }
                BanTagPO banTag = banTagBusiness.getBanTag(tagStr);
                if (banTag is null)
                {
                    await command.ReplyGroupMessageWithAtAsync("该标签未有记录了");
                    return;
                }
                banTagBusiness.DelBanTag(banTag);
                BanTagDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync("解除成功");
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
                BanMemberPO banMember = banMemberBusiness.getBanMember(memberId);
                if (banMember != null)
                {
                    await command.ReplyGroupMessageWithAtAsync("该QQ号已有记录了");
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
                BanMemberPO banMember = banMemberBusiness.getBanMember(memberId);
                if (banMember is null)
                {
                    await command.ReplyGroupMessageWithAtAsync("该QQ号未有记录");
                    return;
                }
                banMemberBusiness.DelBanMember(banMember);
                BanMemberDatas.LoadDatas();
                await command.ReplyGroupMessageWithAtAsync("解除成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "enableMemberAsync异常");
                await Reporter.SendError(ex, "enableMemberAsync异常");
            }
        }


    }
}
