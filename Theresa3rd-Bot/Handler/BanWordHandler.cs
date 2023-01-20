using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class BanWordHandler
    {
        private BanWordBusiness banWordBusiness;

        public BanWordHandler()
        {
            banWordBusiness = new BanWordBusiness();
        }

        public async Task disableSetuTagAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string tagStr = message.splitKeyWord(BotConfig.ManageConfig.DisableTagCommand);

                if (string.IsNullOrEmpty(tagStr))
                {
                    await session.SendGroupMessageWithAtAsync(args, "没有检测到要禁止的关键词，请确保指令格式正确");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.SetuTag, tagStr);
                if (dbBanWord != null)
                {
                    await session.SendGroupMessageWithAtAsync(args, "该关键词已有记录了");
                    return;
                }

                banWordBusiness.insertBanWord(tagStr, BanType.SetuTag, false);
                ConfigHelper.loadBanTag();
                await session.SendGroupMessageWithAtAsync(args, "记录成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "disableSetuAsync异常");
                throw;
            }
        }

        public async Task enableSetuTagAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string tagStr = message.splitKeyWord(BotConfig.ManageConfig.EnableTagCommand);

                if (string.IsNullOrEmpty(tagStr))
                {
                    await session.SendGroupMessageWithAtAsync(args, "没有检测到要解除的关键词，请确保指令格式正确");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.SetuTag, tagStr);
                if (dbBanWord == null)
                {
                    await session.SendGroupMessageWithAtAsync(args, "该关键词未有记录");
                    return;
                }

                banWordBusiness.delBanWord(BanType.SetuTag, tagStr);
                ConfigHelper.loadBanTag();
                await session.SendGroupMessageWithAtAsync(args, "解除成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "enableSetuAsync异常");
                throw;
            }
        }

        public async Task disableMemberAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string memberCode = message.splitKeyWord(BotConfig.ManageConfig.DisableMemberCommand);
                
                if (string.IsNullOrEmpty(memberCode))
                {
                    await session.SendGroupMessageWithAtAsync(args, "没有检测到要禁止的qq号，请确保指令格式正确");
                    return;
                }

                if (BotConfig.PermissionsConfig.SubscribeGroups.Contains(Convert.ToInt64(memberCode)))
                {
                    await session.SendGroupMessageWithAtAsync(args, "无法拉黑超级管理员");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.Member, memberCode);
                if (dbBanWord != null)
                {
                    await session.SendGroupMessageWithAtAsync(args, "该qq号已有记录了");
                    return;
                }

                banWordBusiness.insertBanWord(memberCode, BanType.Member, false);
                ConfigHelper.loadBanMember();
                await session.SendGroupMessageWithAtAsync(args, "记录成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "disableMemberAsync异常");
                throw;
            }
        }


        public async Task enableMemberAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string memberCode = message.splitKeyWord(BotConfig.ManageConfig.EnableMemberCommand);

                if (string.IsNullOrEmpty(memberCode))
                {
                    await session.SendGroupMessageWithAtAsync(args, "没有检测到要解除的qq号，请确保指令格式正确");
                    return;
                }

                BanWordPO dbBanWord = banWordBusiness.getBanWord(BanType.Member, memberCode);
                if (dbBanWord == null)
                {
                    await session.SendGroupMessageWithAtAsync(args, "该关键词未有记录");
                    return;
                }

                banWordBusiness.delBanWord(BanType.Member, memberCode);
                ConfigHelper.loadBanMember();
                await session.SendGroupMessageWithAtAsync(args, "解除成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "enableMemberAsync异常");
                throw;
            }
        }


    }
}
