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
        public static async Task ConnectMirai()
        {
            // 一套能用的框架, 必须要注册至少一个 Invoker, Parser, Client 和 Handler
            // Invoker 负责消息调度
            // Parser 负责解析消息到具体接口以便调度器调用相关 Handler 下的处理方法
            // Client 负责收发原始数据
            IServiceProvider services = new ServiceCollection().AddMiraiBaseFramework()   // 表示使用基于基础框架的构建器
                                                               .Services
                                                               .AddDefaultMiraiHttpFramework() // 表示使用 mirai-api-http 实现的构建器
                                                               //.ResolveParser<DynamicPlugin>() // 只提前解析 DynamicPlugin 将要用到的消息解析器
                                                               .AddInvoker<MiraiHttpMessageHandlerInvoker>() // 使用默认的调度器
                                                               .AddHandler<BotInvitedJoinGroupEvent>()
                                                               .AddHandler<FriendMessageEvent>()
                                                               .AddHandler<GroupApplyEvent>()
                                                               .AddHandler<GroupMessageEvent>()
                                                               .AddHandler<NewFriendApplyEvent>()
                                                               .AddClient<MiraiHttpSession>() // 使用默认的客户端
                                                               .Services
                                                               // 由于 MiraiHttpSession 使用 IOptions<MiraiHttpSessionOptions>, 其作为 Singleton 被注册
                                                               // 配置此项将配置基于此 IServiceProvider 全局的连接配置
                                                               // 如果你想一个作用域一个配置的话
                                                               // 自行做一个实现类, 继承MiraiHttpSession, 构造参数中使用 IOptionsSnapshot<MiraiHttpSessionOptions>
                                                               // 并将其传递给父类的构造参数
                                                               // 然后在每一个作用域中!先!配置好 IOptionsSnapshot<MiraiHttpSessionOptions>, 再尝试获取 IMiraiHttpSession
                                                               .Configure<MiraiHttpSessionOptions>(options =>
                                                               {
                                                                   options.Host = SettingConfig.MiraiConfig.Host;
                                                                   options.Port = SettingConfig.MiraiConfig.Port;
                                                                   options.AuthKey = SettingConfig.MiraiConfig.AuthKey;
                                                               })
                                                               .AddLogging()
                                                               .BuildServiceProvider();
            IServiceScope scope = services.CreateScope();
            services = scope.ServiceProvider;
            IMiraiHttpSession session = services.GetRequiredService<IMiraiHttpSession>(); // 大部分服务都基于接口注册, 请使用接口作为类型解析
            
            await session.ConnectAsync(SettingConfig.MiraiConfig.BotQQ); // 填入期望连接到的机器人QQ号
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
