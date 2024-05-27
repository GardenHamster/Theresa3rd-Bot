namespace TheresaBot.Core.Exceptions
{
    /// <summary>
    /// 使用终止游戏指令强制终止游戏后触发的Exception
    /// </summary>
    public class GameStopedException : GameException
    {
        public GameStopedException() : base("游戏已停止")
        {
        }
    }
}
