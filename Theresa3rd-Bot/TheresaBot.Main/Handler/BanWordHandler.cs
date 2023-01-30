using TheresaBot.Main.Business;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    public class BanWordHandler : BaseHandler
    {
        private BanWordBusiness banWordBusiness;

        public BanWordHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            banWordBusiness = new BanWordBusiness();
        }

        public async Task disableSetuTagAsync(GroupCommand command)
        {
            try
            {
                string tagStr = command.KeyWord;

                if (string.IsNullOrEmpty(tagStr))
                {
                    await command.ReplyGroupMessageWithAtAsync("没有检测到要禁止的关键词，请确保指令格式正确");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.SetuTag, tagStr);
                if (dbBanWord != null)
                {
                    await command.ReplyGroupMessageWithAtAsync("该关键词已有记录了");
                    return;
                }

                banWordBusiness.insertBanWord(tagStr, BanType.SetuTag, false);
                ConfigHelper.loadBanTag();
                await command.ReplyGroupMessageWithAtAsync("记录成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "disableSetuTagAsync异常");
                Reporter.SendError(ex, "disableSetuTagAsync异常");
            }
        }

        public async Task enableSetuTagAsync(GroupCommand command)
        {
            try
            {
                string tagStr = command.KeyWord;

                if (string.IsNullOrEmpty(tagStr))
                {
                    await command.ReplyGroupMessageWithAtAsync("没有检测到要解除的关键词，请确保指令格式正确");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.SetuTag, tagStr);
                if (dbBanWord is null)
                {
                    await command.ReplyGroupMessageWithAtAsync("该关键词未有记录");
                    return;
                }

                banWordBusiness.delBanWord(BanType.SetuTag, tagStr);
                ConfigHelper.loadBanTag();
                await command.ReplyGroupMessageWithAtAsync("解除成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "enableSetuAsync异常");
                Reporter.SendError(ex, "enableSetuAsync异常");
            }
        }

        public async Task disableMemberAsync(GroupCommand command)
        {
            try
            {
                string memberCode = command.KeyWord;

                if (string.IsNullOrEmpty(memberCode))
                {
                    await command.ReplyGroupMessageWithAtAsync("没有检测到要禁止的qq号，请确保指令格式正确");
                    return;
                }

                if (BotConfig.PermissionsConfig.SubscribeGroups.Contains(Convert.ToInt64(memberCode)))
                {
                    await command.ReplyGroupMessageWithAtAsync("无法拉黑超级管理员");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.Member, memberCode);
                if (dbBanWord != null)
                {
                    await command.ReplyGroupMessageWithAtAsync("该qq号已有记录了");
                    return;
                }

                banWordBusiness.insertBanWord(memberCode, BanType.Member, false);
                ConfigHelper.loadBanMember();
                await command.ReplyGroupMessageWithAtAsync("记录成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "disableMemberAsync异常");
                Reporter.SendError(ex, "disableMemberAsync异常");
            }
        }

        public async Task enableMemberAsync(GroupCommand command)
        {
            try
            {
                string memberCode = command.KeyWord;

                if (string.IsNullOrEmpty(memberCode))
                {
                    await command.ReplyGroupMessageWithAtAsync("没有检测到要解除的qq号，请确保指令格式正确");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.Member, memberCode);
                if (dbBanWord is null)
                {
                    await command.ReplyGroupMessageWithAtAsync("该关键词未有记录");
                    return;
                }

                banWordBusiness.delBanWord(BanType.Member, memberCode);
                ConfigHelper.loadBanMember();
                await command.ReplyGroupMessageWithAtAsync("解除成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "enableMemberAsync异常");
                Reporter.SendError(ex, "enableMemberAsync异常");
            }
        }


    }
}
