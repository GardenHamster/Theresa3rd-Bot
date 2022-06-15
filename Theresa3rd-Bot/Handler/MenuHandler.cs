using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class MenuHandler
    {
        public async Task sendMenuAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            try
            {
                long groupId = args.Sender.Group.Id;
                long memberId = args.Sender.Id;

                if (string.IsNullOrWhiteSpace(BotConfig.MenuConfig?.Template) == false)
                {
                    List<IChatMessage> templateList = session.SplitToChainAsync(BotConfig.MenuConfig.Template).Result;
                    await session.SendMessageWithAtAsync(args, templateList);
                    return;
                }

                string prefix = BotConfig.GeneralConfig.Prefix;
                StringBuilder menuBuilder = new StringBuilder();
                menuBuilder.AppendLine($"");
                menuBuilder.AppendLine($"目前实现的功能如下：");
                menuBuilder.AppendLine($"{prefix}{BotConfig.SetuConfig.Pixiv.Command}[标签/pid]?：从pixiv中搜索一张涩图");
                menuBuilder.AppendLine($"{prefix}{BotConfig.SetuConfig.Lolicon.Command}[标签]?：从Lolicon中搜索一张涩图");
                menuBuilder.AppendLine($"");
                menuBuilder.AppendLine($"超级管理员的功能如下：");
                menuBuilder.AppendLine($"{prefix}{BotConfig.SubscribeConfig.Mihoyo.AddCommand}：订阅米游社用户");
                menuBuilder.AppendLine($"{prefix}{BotConfig.SubscribeConfig.Mihoyo.RmCommand}：退订米游社用户");
                menuBuilder.AppendLine($"{prefix}{BotConfig.SubscribeConfig.PixivUser.AddCommand}：订阅P站画师");
                menuBuilder.AppendLine($"{prefix}{BotConfig.SubscribeConfig.PixivUser.SyncCommand}：订阅所有P站已关注的画师");
                menuBuilder.AppendLine($"{prefix}{BotConfig.SubscribeConfig.PixivUser.RmCommand}：退订P站画师");
                menuBuilder.AppendLine($"{prefix}{BotConfig.SubscribeConfig.PixivTag.AddCommand}：订阅P站标签");
                menuBuilder.AppendLine($"{prefix}{BotConfig.SubscribeConfig.PixivTag.RmCommand}：退订P站标签");
                menuBuilder.AppendLine($"{prefix}{BotConfig.SetuConfig.DisableTagCommand}：禁止搜索一个pixiv标签");
                menuBuilder.AppendLine($"{prefix}{BotConfig.SetuConfig.EnableTagCommand}：允许搜索一个pixiv标签");
                await session.SendMessageWithAtAsync(args, new PlainMessage(menuBuilder.ToString()));
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "sendMenuAsync异常");
                throw;
            }
        }




    }
}
