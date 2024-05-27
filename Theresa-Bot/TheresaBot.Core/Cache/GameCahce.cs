using TheresaBot.Core.Command;
using TheresaBot.Core.Exceptions;
using TheresaBot.Core.Game;
using TheresaBot.Core.Relay;

namespace TheresaBot.Core.Cache
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
        /// 处理游戏消息
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public static bool HandleGameMessage(GroupRelay relay)
        {
            var groupId = relay.GroupId;
            var memberId = relay.MemberId;
            var game = GetGameByGroup(groupId);
            if (game is null) return false;
            if (game.IsEnded) return false;
            if (game.IsMemberJoined(memberId) == false) return false;
            return game.HandleGameMessageAsync(relay).Result;
        }

        /// <summary>
        /// 处理游戏消息
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public static bool HandleGameMessage(PrivateRelay relay)
        {
            var isHandled = false;
            var memberId = relay.MemberId;
            var gameList = GetGameByMember(memberId).Where(o => o.IsEnded == false).ToList();
            if (gameList.Count == 0) return false;
            foreach (var game in gameList)
            {
                isHandled |= game.HandleGameMessageAsync(relay).Result;
            }
            return isHandled;
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

        /// <summary>
        /// 获取群内的游戏
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static List<BaseGame> GetGameByMember(long memberId)
        {
            lock (GameDic)
            {
                return GameDic.Select(o => o.Value).Where(o => o.IsMemberJoined(memberId)).ToList();
            }
        }


    }
}
