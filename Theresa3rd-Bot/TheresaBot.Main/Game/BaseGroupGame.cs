using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Game
{
    /// <summary>
    /// 组队游戏，需要自愿报名参加的游戏
    /// </summary>
    public abstract class BaseGroupGame<T> : BaseGame where T : BasePlayer
    {
        /// <summary>
        /// 最小加入人数
        /// </summary>
        public int MinPlayer { get; protected set; }
        /// <summary>
        /// 组队时间
        /// </summary>
        public int MatchSecond { get; protected set; }
        /// <summary>
        /// 玩家列表
        /// </summary>
        public List<T> Players { get; protected set; } = new();
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
        public virtual async Task WaitPlayerAsync()
        {
            int waitSeconds = MatchSecond;
            string prefix = BotConfig.DefaultPrefix;
            string commandStr = BotConfig.GameConfig.Undercover.CreateCommands.JoinCommands(prefix);
            List<BaseContent> remindContents=new List<BaseContent>();
            remindContents.Add(new PlainContent($"{GameName}游戏创建完毕"));
            remindContents.Add(new PlainContent($"距离游戏开始所需人数为：{MinPlayer}个"));
            remindContents.Add(new PlainContent($"游戏匹配时长为{MatchSecond}秒，指定时间内未达到该人数游戏将会终止"));
            remindContents.Add(new PlainContent($"发送群指令 {commandStr} 可以加入游戏"));
            await Session.SendGroupMessageAsync(GroupId, remindContents);
            while (Players.Count < MinPlayer)
            {
                if (waitSeconds < 0) throw new GameException("匹配超时，游戏已停止");
                waitSeconds = waitSeconds - 1;
                await Task.Delay(1000);
            }
        }

        public virtual void PlayerJoinAsync(T player)
        {
            lock (Players)
            {
                if (Players.Count >= MinPlayer)
                {
                    throw new GameException("游戏已经满员了");
                }
                if (Players.Any(o => o.MemberId == player.MemberId))
                {
                    throw new GameException("你已经加入游戏了，请耐心等待游戏开始");
                }
                Players.Add(player);
            }
        }


    }
}
