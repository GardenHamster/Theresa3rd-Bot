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
        /// 游戏开始时间
        /// </summary>
        public DateTime? CreateTime { get; private set; }
        /// <summary>
        /// 游戏结束时间
        /// </summary>
        public DateTime? EndTime { get; private set; }
        /// <summary>
        /// 自由加入模式
        /// </summary>
        public bool FreeToJoin { get; private set; }
        /// <summary>
        /// 最小加入人数
        /// </summary>
        public abstract int MinPlayer { get; protected set; }
        /// <summary>
        /// 组队时间
        /// </summary>
        public abstract int MatchSecond { get; protected set; }
        /// <summary>
        /// 玩家列表
        /// </summary>
        public List<T> Players { get; protected set; } = new();
        /// <summary>
        /// 获取所有玩家成员QQ号
        /// </summary>
        public List<long> MemberIds => Players.Select(o => o.MemberId).Distinct().ToList();
        /// <summary>
        /// 游戏启动钩子
        /// </summary>
        /// <returns></returns>
        public abstract Task GameCreatedAsync();
        /// <summary>
        /// 玩家匹配完毕钩子
        /// </summary>
        /// <returns></returns>
        public abstract Task PlayerWaitingCompletedAsync();
        /// <summary>
        /// 游戏进行中钩子
        /// </summary>
        /// <returns></returns>
        public abstract Task GameProcessingAsync();
        /// <summary>
        /// 游戏结束钩子
        /// </summary>
        /// <returns></returns>
        public abstract Task GameEndingAsync();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="command"></param>
        /// <param name="session"></param>
        /// <param name="reporter"></param>
        public BaseGroupGame(GroupCommand command, BaseSession session, BaseReporter reporter) : base(command, session, reporter)
        {
            FreeToJoin = false;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="command"></param>
        /// <param name="session"></param>
        /// <param name="reporter"></param>
        public BaseGroupGame(GroupCommand command, BaseSession session, BaseReporter reporter, bool freeToJoin) : base(command, session, reporter)
        {
            FreeToJoin = freeToJoin;
        }

        /// <summary>
        /// 启动游戏线程并等待处理完成
        /// </summary>
        /// <returns></returns>
        public override async Task StartProcessing()
        {
            Task task = Task.Run(async () =>
            {
                try
                {
                    CreateTime = DateTime.Now;
                    await GameCreatedAsync();
                    Console.WriteLine($"{GameName}游戏已创建...");
                    if (FreeToJoin)
                    {
                        await PlayerWaitingAsync();
                        await CheckEnded();
                        await PlayerWaitingCompletedAsync();
                    }
                    else
                    {
                        await PlayerMatchingAsync();
                    }
                    await CheckEnded();
                    await Session.SendGroupMessageAsync(GroupId, $"玩家集结完毕，游戏将在5秒后开始...");
                    await CheckEndedAndDelay(5000);
                    IsStarted = true;
                    Console.WriteLine($"{GameName}游戏已开始...");
                    await GameProcessingAsync();
                    Console.WriteLine($"{GameName}游戏已完成...");
                    await CheckEnded();
                    await GameEndingAsync();
                    Console.WriteLine($"{GameName}游戏已结束...");
                }
                catch (GameStopException)
                {
                    await Session.SendGroupMessageAsync(GroupId, $"游戏已停止...");
                }
                catch (GameEndException ex)
                {
                    await Session.SendGroupMessageAsync(GroupId, ex.RemindMessage);
                    await Task.Delay(1000);
                    await Session.SendGroupMessageAsync(GroupId, $"游戏已结束...");
                }
                catch (GameException ex)
                {
                    await Session.SendGroupMessageAsync(GroupId, ex.RemindMessage);
                }
                finally
                {
                    IsEnded = true;
                    EndTime = DateTime.Now;
                }
            });
            GameTask = task;
            await GameTask;
        }

        /// <summary>
        /// 等待玩家线程
        /// </summary>
        /// <returns></returns>
        public virtual async Task PlayerMatchingAsync()
        {
            int waitSeconds = MatchSecond;
            string joinCommandStr = BotConfig.GameConfig.JoinCommands.JoinCommands();
            List<BaseContent> remindContents = new List<BaseContent>();
            remindContents.Add(new PlainContent($"距离游戏开始剩余人数为：{MinPlayer - Players.Count}个"));
            remindContents.Add(new PlainContent($"游戏匹配时长为 {MatchSecond} 秒，匹配时间内未达到该人数游戏将会终止"));
            remindContents.Add(new PlainContent($"发送群指令 【{joinCommandStr}】 可以加入该游戏"));
            await Session.SendGroupMessageAsync(GroupId, remindContents);
            while (Players.Count < MinPlayer)
            {
                await CheckEnded();
                if (waitSeconds < 0) throw new GameException("匹配超时，游戏已停止");
                waitSeconds = waitSeconds - 1;
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// 等待玩家线程
        /// </summary>
        /// <returns></returns>
        public virtual async Task PlayerWaitingAsync()
        {
            int waitSeconds = MatchSecond;
            string joinCommandStr = BotConfig.GameConfig.JoinCommands.JoinCommands();
            List<BaseContent> remindContents = new List<BaseContent>();
            remindContents.Add(new PlainContent($"正在等待玩家加入，游戏匹配时长为 {MatchSecond} 秒，最低玩家人数为 {MinPlayer} 人"));
            remindContents.Add(new PlainContent($"发送群指令 【{joinCommandStr}】 可以加入该游戏"));
            await Session.SendGroupMessageAsync(GroupId, remindContents);
            while (Players.Count < MinPlayer)
            {
                await CheckEnded();
                waitSeconds = waitSeconds - 1;
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// 玩家加入游戏判断逻辑
        /// </summary>
        /// <param name="player"></param>
        /// <exception cref="GameException"></exception>
        public async Task PlayerJoinAsync(T player)
        {
            lock (Players)
            {
                if (IsEnded)
                {
                    throw new GameException("游戏已经结束了~");
                }
                if (IsStarted)
                {
                    throw new GameException("游戏已经开始了~");
                }
                if (Players.Count >= MinPlayer)
                {
                    throw new GameException("游戏已经满员了~");
                }
                if (Players.Any(o => o.MemberId == player.MemberId))
                {
                    throw new GameException("你已经加入该游戏了，请耐心等待游戏开始");
                }
                AddPlayer(player);
            }
            if (FreeToJoin)
            {
                await Session.SendGroupMessageAsync(GroupId, $"加入成功！请耐心等待游戏开始");
            }
            else
            {
                await Session.SendGroupMessageAsync(GroupId, $"加入成功！当前加入人数为 {Players.Count}/{MinPlayer} 人");
            }
        }

        /// <summary>
        /// 向游戏中添加一个玩家
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayer(T player)
        {
            player.SetPlayerId(Players.Count + 1);
            Players.Add(player);
        }

        /// <summary>
        /// 判断游戏是否已经被停止，如果已停止则跳出线程
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GameStopException"></exception>
        public async Task CheckEnded()
        {
            if (IsEnded) throw new GameStopException();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 判断游戏是否已经被停止并且睡眠，如果已停止则跳出线程
        /// </summary>
        /// <param name="daily"></param>
        /// <returns></returns>
        /// <exception cref="GameStopException"></exception>
        public async Task CheckEndedAndDelay(int daily = 0)
        {
            if (IsEnded) throw new GameStopException();
            if (daily > 0) await Task.Delay(daily);
        }


    }
}
