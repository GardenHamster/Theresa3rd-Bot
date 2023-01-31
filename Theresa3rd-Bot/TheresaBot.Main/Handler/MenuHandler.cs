using System.Text;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    public class MenuHandler : BaseHandler
    {
        public MenuHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
        }

        public async Task sendMenuAsync(GroupCommand command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BotConfig.MenuConfig?.Template) == false)
                {
                    List<BaseContent> templateList = BotConfig.MenuConfig.Template.SplitToChainAsync(SendTarget.Group);
                    await command.ReplyGroupMessageWithAtAsync(templateList);
                    return;
                }

                await command.ReplyGroupMessageAsync(getMemberMenu());

                if (command.MemberId.IsSuperManager())
                {
                    await Task.Delay(1000);
                    await command.ReplyGroupMessageAsync(getManagerMenu());
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "菜单发送失败");
                Reporter.SendError(ex, "菜单发送失败");
            }
        }

        private string getMemberMenu()
        {
            string prefix = BotConfig.GeneralConfig.Prefix;
            StringBuilder menuBuilder = new StringBuilder();
            menuBuilder.AppendLine($"目前实现的功能如下：");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.SetuConfig.Pixiv.Commands)}[标签/pid]?】 从pixiv中搜索一张涩图");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.SetuConfig.Lolicon.Commands)}[标签]?】 从Lolicon中搜索一张涩图");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.SetuConfig.Lolisuki.Commands)}[标签]?】 从Lolisuki中搜索一张涩图");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.SaucenaoConfig.Commands)}[图片]?】 尝试用Saucenao查找来源，并返回原图等信息");
            menuBuilder.AppendLine($"使用实例请参考：https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/Menu.md");
            return menuBuilder.ToString();
        }

        private string getManagerMenu()
        {
            string prefix = BotConfig.GeneralConfig.Prefix;
            StringBuilder menuBuilder = new StringBuilder();
            menuBuilder.AppendLine($"超级管理员的功能如下：");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.SubscribeConfig.Miyoushe.AddCommands)}】 订阅米游社用户");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.SubscribeConfig.Miyoushe.RmCommands)}】 退订米游社用户");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.SubscribeConfig.PixivUser.AddCommands)}】 订阅P站画师");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.SubscribeConfig.PixivUser.SyncCommands)}】 订阅所有P站已关注的画师");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.SubscribeConfig.PixivUser.RmCommands)}】 退订P站画师");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.SubscribeConfig.PixivTag.AddCommands)}】 订阅P站标签");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.SubscribeConfig.PixivTag.RmCommands)}】 退订P站标签");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.ManageConfig.DisableMemberCommands)}】 拉黑一个成员，不处理该成员的指令");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.ManageConfig.EnableMemberCommands)}】 解禁一个成员，允许该成员使用指令");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.ManageConfig.DisableTagCommands)}】 禁止搜索一个pixiv标签");
            menuBuilder.AppendLine($"【{getCommandStr(BotConfig.ManageConfig.EnableTagCommands)}】 允许搜索一个pixiv标签");
            return menuBuilder.ToString();
        }

        private string getCommandStr(List<string> commands)
        {
            StringBuilder commandBuilder = new StringBuilder();
            foreach (string command in commands)
            {
                if (commandBuilder.Length > 0) commandBuilder.Append('/');
                commandBuilder.Append($"#{command}");
            }
            return commandBuilder.ToString();
        }

    }
}
