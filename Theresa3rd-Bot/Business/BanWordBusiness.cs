using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Dao;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class BanWordBusiness
    {
        private BanWordDao banWordDao;

        public BanWordBusiness()
        {
            this.banWordDao = new BanWordDao();
        }

        public async Task disableSetuAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                long groupId = args.Sender.Group.Id;
                string keyWord = message.splitKeyWord(BotConfig.SetuConfig.DisableTagCommand);

                if (string.IsNullOrEmpty(keyWord))
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 没有检测到要禁止的关键词，请确保指令格式正确"));
                    return;
                }

                BanWordPO dbBanWord = banWordDao.getBanWord(BanType.Setu, groupId, keyWord);
                if (dbBanWord != null)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 该关键词已有记录了"));
                    return;
                }

                insertBanWord(keyWord, BanType.Setu, groupId, false);
                ConfigHelper.loadBanWord();
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 记录成功"));
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "disableSetuAsync异常");
                throw;
            }
        }

        public async Task enableSetuAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                long groupId = args.Sender.Group.Id;
                string keyWord = message.splitKeyWord(BotConfig.SetuConfig.EnableTagCommand);

                if (string.IsNullOrEmpty(keyWord))
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 没有检测到要禁止的关键词，请确保指令格式正确"));
                    return;
                }

                BanWordPO dbBanWord = banWordDao.getBanWord(BanType.Setu, groupId, keyWord);
                if (dbBanWord == null)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 该关键词未有记录"));
                    return;
                }

                banWordDao.delBanWord(BanType.Setu, groupId, keyWord);
                ConfigHelper.loadBanWord();
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 记录成功"));
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "enableSetuAsync异常");
                throw;
            }
        }

        public Dictionary<long, List<BanWordPO>> getBanSetuMap()
        {
            Dictionary<long, List<BanWordPO>> banSetuMap = new Dictionary<long, List<BanWordPO>>();
            List<BanWordPO> banList = banWordDao.getListByType(BanType.Setu);
            if (banList.Count == 0) return banSetuMap;
            foreach (var item in banList)
            {
                long groupId = item.GroupId;
                if (banSetuMap.ContainsKey(groupId) == false) banSetuMap[groupId] = new List<BanWordPO>();
                banSetuMap[groupId].Add(item);
            }
            return banSetuMap;
        }


        private BanWordPO insertBanWord(string keyword, BanType type, long groupId, bool isRegular)
        {
            BanWordPO banWord = new BanWordPO();
            banWord.KeyWord = keyword;
            banWord.BanType = type;
            banWord.GroupId = groupId;
            banWord.IsRegular = isRegular;
            banWord.CreateDate = DateTime.Now;
            return banWordDao.Insert(banWord);
        }




    }
}
