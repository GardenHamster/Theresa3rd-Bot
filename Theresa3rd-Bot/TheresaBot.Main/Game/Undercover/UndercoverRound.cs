using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Game.Undercover
{
    public class UndercoverRound
    {
        /// <summary>
        /// 发言记录
        /// </summary>
        public List<UndercoverRecord> Records { get; private set; } = new();

        /// <summary>
        /// 投票记录
        /// </summary>
        public List<UndercoverVote> Votes { get; private set; } = new();

        /// <summary>
        /// 出局成员
        /// </summary>
        public List<UndercoverPlayer> OutPlayers { get; private set; } = new();

        /// <summary>
        /// 添加一个成员发言记录，如果该成员已经发言，则返回null
        /// </summary>
        /// <param name="player"></param>
        /// <param name="relay"></param>
        /// <returns></returns>
        public UndercoverRecord AddPlayerRecord(UndercoverPlayer player, GroupRelay relay)
        {
            lock (Records)
            {
                var record = GetPlayerRecord(player);
                if (record is not null) return null;
                record = new UndercoverRecord(player, relay);
                Records.Add(record);
                return record;
            }
        }

        /// <summary>
        /// 添加一个成员投票记录，如果该成员已经投票，则返回null
        /// </summary>
        /// <param name="player"></param>
        /// <param name="relay"></param>
        /// <returns></returns>
        public UndercoverVote AddPlayerVote(GroupRelay relay)
        {
            lock (Records)
            {
                
            }
        }

        /// <summary>
        /// 等待玩家的发言
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task<UndercoverRecord> WaitForSpeech(UndercoverPlayer player, int waitSeconds)
        {
            int lastSeconds = waitSeconds;
            while (lastSeconds > 0)
            {
                var record = GetPlayerRecord(player);
                if (record is not null) return record;
                await Task.Delay(1000);
                lastSeconds--;
            }
            return null;
        }

        /// <summary>
        /// 等待所有玩家投票完毕
        /// </summary>
        /// <returns></returns>
        public async Task WaitForVote(int waitSeconds)
        {
            int lastSeconds = waitSeconds;
            while (lastSeconds > 0)
            {
                if (Votes.Count >= Records.Count) return;
                await Task.Delay(1000);
                lastSeconds--;
            }
            throw new GameEndException("部分玩家在指定时间内未进行投票，游戏结束");
        }

        /// <summary>
        /// 获取玩家发言记录
        /// </summary>
        /// <returns></returns>
        public UndercoverRecord GetPlayerRecord(UndercoverPlayer player)
        {
            lock (Records)
            {
                return Records.FirstOrDefault(o => o.Player.MemberId == player.MemberId);
            }
        }

        /// <summary>
        /// 获取玩家发言记录
        /// </summary>
        /// <returns></returns>
        public UndercoverRecord GetPlayerRecord(long memberId)
        {
            lock (Records)
            {
                return Records.FirstOrDefault(o => o.Player.MemberId == memberId);
            }
        }

        

    }
}
