using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Game;
using TheresaBot.Main.Game.Undercover;
using TheresaBot.Main.Model.Process;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Cache
{
    public static class GameCahce
    {
        private static Dictionary<long, BaseGame> GameDic = new Dictionary<long, BaseGame>();

        public static bool HandleGame(GroupRelay relay)
        {
            lock (GameDic)
            {
                long groupId = relay.GroupId;
                if (!GameDic.ContainsKey(groupId)) return false;
                BaseGame game = GameDic[groupId];
                if (game is null) return false;
                if (game.IsEnded) return false;
                return game.HandleProcessing(relay);
            }
        }

        /// <summary>
        /// 创建一个谁是卧底游戏
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="ProcessException"></exception>
        public static UndercoverGame CreateUndercoverGame(GroupCommand command)
        {
            lock (GameDic)
            {
                long groupId = command.GroupId;
                BaseGame baseGame = GetGameByGroup(groupId);
                if (baseGame is null || baseGame.IsEnded)
                {
                    var newGame = new UndercoverGame();
                    GameDic[groupId] = baseGame;
                    return newGame;
                }
                throw new GameException("群内的另一个游戏正在进行中");
            }
        }

        private static BaseGame GetGameByGroup(long groupId)
        {
            lock (GameDic)
            {
                if (GameDic.ContainsKey(groupId))
                {
                    return GameDic[groupId];
                }
                else
                {
                    return null;
                }
            }
        }


    }
}
