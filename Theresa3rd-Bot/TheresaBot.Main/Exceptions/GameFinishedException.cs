namespace TheresaBot.Main.Exceptions
{
    /// <summary>
    /// 某位玩家在遵循游戏规则的前提下提前结束了游戏触发的Exception
    /// </summary>
    public class GameFinishedException : GameException
    {
        public GameFinishedException() : base("游戏结束") { }
    }
}
