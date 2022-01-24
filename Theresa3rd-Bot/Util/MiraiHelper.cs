using Microsoft.Extensions.DependencyInjection;
using Mirai.CSharp.Builders;
using Mirai.CSharp.HttpApi.Builder;
using Mirai.CSharp.HttpApi.Invoking;
using Mirai.CSharp.HttpApi.Options;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Event;

namespace Theresa3rd_Bot.Util
{
    public static class MiraiHelper
    {
        public static IServiceProvider Services;

        public static IServiceScope Scope;

        public static IMiraiHttpSession Session;

        public static async Task ConnectMirai()
        {
            try
            {
                Services = new ServiceCollection().AddMiraiBaseFramework()
                                                               .Services
                                                               .AddDefaultMiraiHttpFramework()
                                                               .AddInvoker<MiraiHttpMessageHandlerInvoker>()
                                                               .AddHandler<BotInvitedJoinGroupEvent>()
                                                               .AddHandler<FriendMessageEvent>()
                                                               .AddHandler<GroupApplyEvent>()
                                                               .AddHandler<GroupMessageEvent>()
                                                               .AddHandler<NewFriendApplyEvent>()
                                                               .AddHandler<GroupMemberJoinedEvent>()
                                                               .AddClient<MiraiHttpSession>()
                                                               .Services
                                                               .Configure<MiraiHttpSessionOptions>(options =>
                                                               {
                                                                   options.Host = BotConfig.MiraiConfig.Host;
                                                                   options.Port = BotConfig.MiraiConfig.Port;
                                                                   options.AuthKey = BotConfig.MiraiConfig.AuthKey;
                                                                   options.SuppressAwaitMessageInvoker = true;
                                                               })
                                                               .AddLogging()
                                                               .BuildServiceProvider();
                Scope = Services.CreateAsyncScope();
                Services = Scope.ServiceProvider;
                Session = Services.GetRequiredService<IMiraiHttpSession>();
                await Session.ConnectAsync(BotConfig.MiraiConfig.BotQQ);
                while (true)
                {
                    if (Console.ReadLine() == "exit")
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex,"连接到mcl失败");
            }
        }

    }
}
