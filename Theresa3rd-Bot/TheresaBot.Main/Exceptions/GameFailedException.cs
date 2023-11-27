namespace TheresaBot.Main.Exceptions
{
    /// <summary>
    /// 由于某位玩家未遵循游戏规则导致的游戏终止的Exception
    /// </summary>
    public class GameFailedException : GameException
    {
        public GameFailedException(string message) : base(message) { }
    }
}
