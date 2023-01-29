using System;
using System.Threading.Tasks;
using Theresa3rd_Bot.BotPlatform.Base.Command;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Command;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class BanWordHandler : BaseHandler
    {
        private BanWordBusiness banWordBusiness;

        public BanWordHandler()
        {
            banWordBusiness = new BanWordBusiness();
        }

        public async Task disableSetuTagAsync(GroupCommand command)
        {
            try
            {
                string tagStr = BotCommand.KeyWord;

                if (string.IsNullOrEmpty(tagStr))
                {
                    await BotCommand.ReplyGroupMessageWithAtAsync("没有检测到要禁止的关键词，请确保指令格式正确");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.SetuTag, tagStr);
                if (dbBanWord != null)
                {
                    await BotCommand.ReplyGroupMessageWithAtAsync("该关键词已有记录了");
                    return;
                }

                banWordBusiness.insertBanWord(tagStr, BanType.SetuTag, false);
                ConfigHelper.loadBanTag();
                await BotCommand.ReplyGroupMessageWithAtAsync("记录成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "disableSetuTagAsync异常");
                ReportHelper.SendError(ex, "disableSetuTagAsync异常");
            }
        }

        public async Task enableSetuTagAsync(GroupCommand command)
        {
            try
            {
                string tagStr = BotCommand.KeyWord;

                if (string.IsNullOrEmpty(tagStr))
                {
                    await BotCommand.ReplyGroupMessageWithAtAsync("没有检测到要解除的关键词，请确保指令格式正确");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.SetuTag, tagStr);
                if (dbBanWord is null)
                {
                    await BotCommand.ReplyGroupMessageWithAtAsync("该关键词未有记录");
                    return;
                }

                banWordBusiness.delBanWord(BanType.SetuTag, tagStr);
                ConfigHelper.loadBanTag();
                await BotCommand.ReplyGroupMessageWithAtAsync("解除成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "enableSetuAsync异常");
                ReportHelper.SendError(ex, "enableSetuAsync异常");
            }
        }

        public async Task disableMemberAsync(GroupCommand command)
        {
            try
            {
                string memberCode = BotCommand.KeyWord;
                
                if (string.IsNullOrEmpty(memberCode))
                {
                    await BotCommand.ReplyGroupMessageWithAtAsync("没有检测到要禁止的qq号，请确保指令格式正确");
                    return;
                }

                if (BotConfig.PermissionsConfig.SubscribeGroups.Contains(Convert.ToInt64(memberCode)))
                {
                    await BotCommand.ReplyGroupMessageWithAtAsync("无法拉黑超级管理员");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.Member, memberCode);
                if (dbBanWord != null)
                {
                    await BotCommand.ReplyGroupMessageWithAtAsync("该qq号已有记录了");
                    return;
                }

                banWordBusiness.insertBanWord(memberCode, BanType.Member, false);
                ConfigHelper.loadBanMember();
                await BotCommand.ReplyGroupMessageWithAtAsync("记录成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "disableMemberAsync异常");
                ReportHelper.SendError(ex, "disableMemberAsync异常");
            }
        }

        public async Task enableMemberAsync(GroupCommand command)
        {
            try
            {
                string memberCode = BotCommand.KeyWord;

                if (string.IsNullOrEmpty(memberCode))
                {
                    await BotCommand.ReplyGroupMessageWithAtAsync("没有检测到要解除的qq号，请确保指令格式正确");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.Member, memberCode);
                if (dbBanWord is null)
                {
                    await BotCommand.ReplyGroupMessageWithAtAsync("该关键词未有记录");
                    return;
                }

                banWordBusiness.delBanWord(BanType.Member, memberCode);
                ConfigHelper.loadBanMember();
                await BotCommand.ReplyGroupMessageWithAtAsync("解除成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "enableMemberAsync异常");
                ReportHelper.SendError(ex, "enableMemberAsync异常");
            }
        }


    }
}
