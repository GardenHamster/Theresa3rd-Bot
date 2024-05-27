using TheresaBot.Core.Game;

namespace TheresaBot.Core.Exceptions
{
    /// <summary>
    /// 由于某位玩家未遵循游戏规则导致的游戏终止的Exception
    /// </summary>
    public class GameFailedException : GameException
    {
        public List<BasePlayer> FailedPlayers { get; set; }

        public GameFailedException(List<BasePlayer> failedPlayers, string message) : base(message)
        {
            FailedPlayers = failedPlayers;
        }

        public GameFailedException(BasePlayer failedPlayer, string message) : base(message)
        {
            FailedPlayers = new List<BasePlayer>() { failedPlayer };
        }

    }
}
