namespace TheresaBot.Main.Exceptions
{
    public class GameStopException : GameException
    {
        public GameStopException() : base("游戏已停止")
        {
        }
    }
}
