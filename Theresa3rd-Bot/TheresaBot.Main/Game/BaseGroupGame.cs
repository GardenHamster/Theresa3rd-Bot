using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Game.Undercover;
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
        /// 获取所有玩家成员QQ号
        /// </summary>
        public List<long> MemberIds => Players.Select(o => o.MemberId).Distinct().ToList();
        /// <summary>
        /// 游戏线程
        /// </summary>
        /// <returns></returns>
        public abstract Task GameProcessingAsync();

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
                try
                {
                    Console.WriteLine($"{GameName}游戏已创建...");
                    await WaitPlayerAsync();
                    if (IsEnded) throw new GameStopException();
                    await Session.SendGroupMessageAsync(GroupId, $"玩家集结完毕，游戏将在5秒后开始...");
                    await Task.Delay(5000);
                    if (IsEnded) throw new GameStopException();
                    Console.WriteLine($"{GameName}游戏已开始...");
                    await GameProcessingAsync();
                    Console.WriteLine($"{GameName}游戏已结束...");
                }
                catch (GameStopException)
                {
                    await Session.SendGroupMessageAsync(GroupId, $"游戏已停止...");
                }
                catch (GameEndException ex)
                {
                    await Session.SendGroupMessageAsync(GroupId, ex.RemindMessage);
                }
                catch (GameException ex)
                {
                    await Session.SendGroupMessageAsync(GroupId, ex.RemindMessage);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"{GameName}游戏异常");
                    await Session.SendGroupMessageAsync(GroupId, $"{GameName}游戏异常");
                    await Reporter.SendError(ex, $"{GameName}游戏异常");
                }
                finally
                {
                    IsEnded = true;
                }
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
            string commandStr = BotConfig.GameConfig.Undercover.CreateCommands.JoinCommands();
            List<BaseContent> remindContents = new List<BaseContent>();
            remindContents.Add(new PlainContent($"{GameName}游戏创建完毕"));
            remindContents.Add(new PlainContent($"距离游戏开始所需人数为：{MinPlayer}个"));
            remindContents.Add(new PlainContent($"游戏匹配时长为{MatchSecond}秒，指定时间内未达到该人数游戏将会终止"));
            remindContents.Add(new PlainContent($"发送群指令 {commandStr} 可以加入游戏"));
            await Session.SendGroupMessageAsync(GroupId, remindContents);
            while (Players.Count < MinPlayer)
            {
                if (IsEnded) throw new GameStopException();
                if (waitSeconds < 0) throw new GameException("匹配超时，游戏已停止");
                waitSeconds = waitSeconds - 1;
                await Task.Delay(1000);
            }
        }

        public virtual void PlayerJoinAsync(T player)
        {
            lock (Players)
            {
                if (IsEnded)
                {
                    throw new GameStopException();
                }
                if (Players.Count >= MinPlayer)
                {
                    throw new GameException("游戏已经满员了");
                }
                if (Players.Any(o => o.MemberId == player.MemberId))
                {
                    throw new GameException("你已经加入游戏了，请耐心等待游戏开始");
                }
                player.PlayerId = Players.Count + 1;
                Players.Add(player);
            }
        }

        /// <summary>
        /// 根据Id获取玩家
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        protected T GetPlayer(string playerIdStr)
        {
            if (string.IsNullOrWhiteSpace(playerIdStr)) return null;
            int playerId = playerIdStr.Trim().ToInt();
            if (playerId <= 0) return null;
            return Players.FirstOrDefault(o => o.PlayerId == playerId);
        }

        /// <summary>
        /// 根据QQ获取玩家
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        protected T GetPlayer(long memberId)
        {
            return Players.FirstOrDefault(o => o.MemberId == memberId);
        }


    }
}
