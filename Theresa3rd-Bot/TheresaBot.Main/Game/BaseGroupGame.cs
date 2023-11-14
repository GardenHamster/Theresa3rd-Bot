using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Game
{
    /// <summary>
    /// 组队游戏，需要自愿报名参加的游戏
    /// </summary>
    public abstract class BaseGroupGame : BaseGame
    {
        /// <summary>
        /// 加入游戏中的成员
        /// </summary>
        public List<long> MemberIds { get; protected set; }
        /// <summary>
        /// 最小加入人数
        /// </summary>
        public int MinMember { get; protected set; }
        /// <summary>
        /// 最多加入人数
        /// </summary>
        public int MaxMember { get; protected set; }
        /// <summary>
        /// 组队时间
        /// </summary>
        public int MatchSecond { get; protected set; }
        /// <summary>
        /// 游戏线程
        /// </summary>
        /// <returns></returns>
        public abstract Task HandleGameAsync();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="command"></param>
        /// <param name="session"></param>
        /// <param name="reporter"></param>
        public BaseGroupGame(GroupCommand command, BaseSession session, BaseReporter reporter) : base(command, session, reporter)
        {
            
        }

        /// <summary>
        /// 启动游戏线程并等待处理完成
        /// </summary>
        /// <returns></returns>
        public override async Task StartProcessing()
        {
            Task task = Task.Run(async () =>
            {
                Console.WriteLine("谁是卧底游戏已创建...");
                await WaitPlayerAsync();
                Console.WriteLine("谁是卧底游戏已开始...");
                await HandleGameAsync();
                Console.WriteLine("谁是卧底游戏已结束...");
            });
            GameTask = task;
            await GameTask;
        }

        /// <summary>
        /// 等待玩家线程
        /// </summary>
        /// <returns></returns>
        public virtual Task WaitPlayerAsync()
        {
            List<BaseContent> remindContents=new List<BaseContent>();
            remindContents.Add(new PlainContent($"{GameName}游戏创建完毕"));
            remindContents.Add(new PlainContent($"发送群指令{BotConfig.GameConfig.Undercover}"));
            Session.SendGroupMessageAsync(GroupId, remindContents);
        }

    }
}
