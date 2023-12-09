using TheresaBot.Main.Common;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Helper
{
    public static class BotHelper
    {
        /// <summary>
        /// 获取机器人信息
        /// </summary>
        /// <returns></returns>
        public static async Task LoadBotProfileAsync(BaseSession session)
        {
            try
            {
                BotConfig.BotInfos = await session.GetBotInfosAsync();
                LogHelper.Info($"Bot信息加载完毕，QQ={BotConfig.BotInfos.QQ}，Nick={BotConfig.BotInfos.NickName}");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Bot信息加载失败");
                throw;
            }
        }

        /// <summary>
        /// 获取群列表
        /// </summary>
        /// <returns></returns>
        public static async Task LoadGroupInfosAsync(BaseSession session)
        {
            try
            {
                var config = BotConfig.PermissionsConfig;
                var groupInfos = await session.GetGroupInfosAsync();
                var usableGroups = groupInfos.Select(o => o.GroupId).ToList();
                var acceptGroups = config.AcceptGroups.Contains(0) ? usableGroups : config.AcceptGroups;
                BotConfig.GroupInfos = groupInfos.ToList();
                BotConfig.AcceptGroups = acceptGroups.ToList();
                LogHelper.Info($"群列表加载完毕，共加载群号 {usableGroups.Count} 个，其中已启用群号 {acceptGroups.Count} 个");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群列表加载失败");
                throw;
            }
        }


    }
}
