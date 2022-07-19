using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public abstract class BaseHandler
    {
        /// <summary>
        /// 检查是否拥有自定义涩图权限
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> CheckSetuCustomEnableAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SetuCustomGroups.Contains(args.Sender.Group.Id)) return true;
            await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.SetuCustomDisableMsg, " 自定义功能已关闭");
            return false;
        }

        /// <summary>
        /// 获取今日涩图可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static long GetSetuLeftToday(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.SetuConfig.MaxDaily == 0) return 0;
            if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(args.Sender.Group.Id)) return BotConfig.SetuConfig.MaxDaily;
            RequestRecordBusiness requestRecordBusiness = new RequestRecordBusiness();
            int todayUseCount = requestRecordBusiness.getUsedCountToday(args.Sender.Group.Id, args.Sender.Id, CommandType.Setu);
            long leftToday = BotConfig.SetuConfig.MaxDaily - todayUseCount - 1;
            return leftToday < 0 ? 0 : leftToday;
        }

        /// <summary>
        /// 获取今日原图可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int GetSaucenaoLeftToday(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.SaucenaoConfig.MaxDaily == 0) return 0;
            RequestRecordBusiness requestRecordBusiness = new RequestRecordBusiness();
            int todayUseCount = requestRecordBusiness.getUsedCountToday(args.Sender.Group.Id, args.Sender.Id, CommandType.Saucenao);
            int leftToday = BotConfig.SaucenaoConfig.MaxDaily - todayUseCount - 1;
            return leftToday < 0 ? 0 : leftToday;
        }


    }
}
