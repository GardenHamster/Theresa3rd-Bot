using System.Text;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    internal class MenuHandler : BaseHandler
    {
        public MenuHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
        }

        public async Task SendMenuAsync(GroupCommand command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BotConfig.MenuConfig?.Template) == false)
                {
                    List<BaseContent> templateList = BotConfig.MenuConfig.Template.SplitToChainAsync();
                    await command.ReplyGroupMessageWithQuoteAsync(templateList);
                    return;
                }

                await command.ReplyGroupMessageWithQuoteAsync(GetMemberMenu());

                if (command.MemberId.IsSuperManager())
                {
                    await Task.Delay(1000);
                    await command.ReplyGroupMessageWithQuoteAsync(GetManagerMenu());
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "菜单发送失败");
                await Reporter.SendError(ex, "菜单发送失败");
            }
        }

        private string GetMemberMenu()
        {
            StringBuilder menuBuilder = new StringBuilder();
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.SetuConfig.Pixiv.Commands)}】获取pixiv涩图");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.SetuConfig.Lolicon.Commands)}】获取Lolicon涩图");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.SetuConfig.Lolisuki.Commands)}】获取Lolisuki涩图");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.SetuConfig.Local.Commands)}】获取本地文件夹中的涩图");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.SaucenaoConfig.Commands)}】搜索原图，并返回详细信息");
            menuBuilder.AppendLine($"【{PixivRankingCommands()}】获取pixiv日榜");
            menuBuilder.Append($"【详细参数阅读文档】{BotConfig.BotHomepage}");
            return menuBuilder.ToString();
        }

        private string GetManagerMenu()
        {
            StringBuilder menuBuilder = new StringBuilder();
            menuBuilder.AppendLine($"超级管理员的功能如下：");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.SubscribeConfig.Miyoushe.AddCommands)}】订阅米游社用户");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.SubscribeConfig.PixivUser.AddCommands)}】订阅P站画师");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.SubscribeConfig.PixivUser.SyncCommands)}】订阅所有P站已关注的画师");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.SubscribeConfig.PixivTag.AddCommands)}】订阅P站标签");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.ManageConfig.ListSubCommands)}】查询订阅");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.ManageConfig.RemoveSubCommands)}】取消订阅");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.ManageConfig.DisableMemberCommands)}】拉黑成员");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.ManageConfig.EnableMemberCommands)}】解除拉黑");
            menuBuilder.AppendLine($"【{JoinCommands(BotConfig.ManageConfig.DisableTagCommands)}】屏蔽涩图标签");
            menuBuilder.Append($"【{JoinCommands(BotConfig.ManageConfig.EnableTagCommands)}】解除屏蔽");
            return menuBuilder.ToString();
        }

        private string JoinCommands(List<string> commands)
        {
            return $"{BotConfig.GeneralConfig.DefaultPrefix}{string.Join('/', commands)}";
        }

        private string PixivRankingCommands()
        {
            string dailyCommand = BotConfig.PixivRankingConfig.Daily?.Commands?.FirstOrDefault();
            string aiCommand = BotConfig.PixivRankingConfig.DailyAI?.Commands?.FirstOrDefault();
            string maleCommand = BotConfig.PixivRankingConfig.Male?.Commands?.FirstOrDefault();
            string weeklyCommand = BotConfig.PixivRankingConfig.Weekly?.Commands?.FirstOrDefault();
            string monthlyCommand = BotConfig.PixivRankingConfig.Monthly?.Commands?.FirstOrDefault();
            List<string> commands = new() { dailyCommand, aiCommand, maleCommand, weeklyCommand, monthlyCommand };
            commands = commands.Where(o => o is not null).Distinct().ToList();
            return $"{BotConfig.GeneralConfig.DefaultPrefix}{String.Join('/', commands)}";
        }



    }
}
