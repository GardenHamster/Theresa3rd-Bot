namespace TheresaBot.Main.Exceptions
{
    public class GameStopedException : GameException
    {
        public GameStopedException() : base("游戏已停止")
        {
        }
    }
}
