using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Game;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Cache
{
    public static class GameCahce
    {
        private static Dictionary<long, BaseGame> GameDic = new Dictionary<long, BaseGame>();

        /// <summary>
        /// 判断一个群是否正在进行游戏中
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static bool IsGaming(long groupId)
        {
            lock (GameDic)
            {
                var game = GetGameByGroup(groupId);
                if (game is null) return false;
                if (game.IsEnded) return false;
                return true;
            }
        }

        /// <summary>
        /// 创建并缓存一个游戏
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public static bool HandleGameMessage(GroupRelay relay)
        {
            lock (GameDic)
            {
                long groupId = relay.GroupId;
                long memberId = relay.MemberId;
                if (!GameDic.ContainsKey(groupId)) return false;
                BaseGame game = GameDic[groupId];
                if (game is null) return false;
                if (game.IsEnded) return false;
                if (game.IsMemberJoined(memberId) == false) return false;
                return game.HandleGameMessage(relay);
            }
        }

        /// <summary>
        /// 创建一个游戏
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="ProcessException"></exception>
        public static void CreateGame(GroupCommand command, BaseGame newGame)
        {
            lock (GameDic)
            {
                long groupId = command.GroupId;
                BaseGame baseGame = GetGameByGroup(groupId);
                if (baseGame is null || baseGame.IsEnded)
                {
                    GameDic[groupId] = newGame;
                    return;
                }
                throw new GameException("群内的另一个游戏正在进行中");
            }
        }

        /// <summary>
        /// 获取群内的游戏
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static BaseGame GetGameByGroup(long groupId)
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
