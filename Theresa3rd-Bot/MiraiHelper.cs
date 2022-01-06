using Microsoft.Extensions.DependencyInjection;
using Mirai.CSharp.Builders;
using Mirai.CSharp.HttpApi.Builder;
using Mirai.CSharp.HttpApi.Invoking;
using Mirai.CSharp.HttpApi.Options;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Event;
using Theresa3rd_Bot.Model.Config;

namespace Theresa3rd_Bot
{
    public static class MiraiHelper
    {
        public static IServiceProvider services;

        public static IServiceScope scope;

        public static IMiraiHttpSession session;

        public static async Task ConnectMirai()
        {
            services = new ServiceCollection().AddMiraiBaseFramework()   // 表示使用基于基础框架的构建器
                                                               .Services
                                                               .AddDefaultMiraiHttpFramework() // 表示使用 mirai-api-http 实现的构建器
                                                               .AddInvoker<MiraiHttpMessageHandlerInvoker>() // 使用默认的调度器
                                                               .AddHandler<BotInvitedJoinGroupEvent>()
                                                               .AddHandler<FriendMessageEvent>()
                                                               .AddHandler<GroupApplyEvent>()
                                                               .AddHandler<GroupMessageEvent>()
                                                               .AddHandler<NewFriendApplyEvent>()
                                                               .AddHandler<GroupMemberJoinedEvent>()
                                                               .AddClient<MiraiHttpSession>() // 使用默认的客户端
                                                               .Services
                                                               .Configure<MiraiHttpSessionOptions>(options =>
                                                               {
                                                                   options.Host = BotConfig.MiraiConfig.Host;
                                                                   options.Port = BotConfig.MiraiConfig.Port;
                                                                   options.AuthKey = BotConfig.MiraiConfig.AuthKey;
                                                               })
                                                               .AddLogging()
                                                               .BuildServiceProvider();
            scope = services.CreateScope();
            services = scope.ServiceProvider;
            session = services.GetRequiredService<IMiraiHttpSession>(); // 大部分服务都基于接口注册, 请使用接口作为类型解析
            await session.ConnectAsync(BotConfig.MiraiConfig.BotQQ);

            while (true)
            {
                if (Console.ReadLine() == "exit")
                {
                    break;
                }
            }
        }


    }
}
