using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Game.Undercover
{
    public class UCRound
    {
        /// <summary>
        /// 子轮(票数相同时的PK轮)
        /// </summary>
        public List<UCRound> SubRounds { get; private set; } = new();

        /// <summary>
        /// 发言记录
        /// </summary>
        public List<UCSpeech> Speechs { get; private set; } = new();

        /// <summary>
        /// 投票记录
        /// </summary>
        public List<UCVote> Votes { get; private set; } = new();

        /// <summary>
        /// 出局成员
        /// </summary>
        public List<UCPlayer> OutPlayers { get; private set; } = new();

        /// <summary>
        /// 添加一个成员发言记录，如果该成员已经发言，则返回null
        /// </summary>
        /// <param name="player"></param>
        /// <param name="relay"></param>
        /// <returns></returns>
        public UCSpeech AddPlayerSpeech(UCPlayer player, GroupRelay relay)
        {
            lock (Speechs)
            {
                var record = GetPlayerSpeech(player);
                if (record is not null) return null;
                record = new UCSpeech(player, relay);
                Speechs.Add(record);
                return record;
            }
        }

        /// <summary>
        /// 添加一个成员投票记录，如果该成员已经投票，则返回null
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public UCVote AddPlayerVote(UCPlayer voter, UCPlayer target)
        {
            lock (Speechs)
            {
                var vote = GetPlayerVote(voter);
                if (vote is not null) return null;
                vote = new UCVote(voter, target);
                Votes.Add(vote);
                return vote;
            }
        }

        /// <summary>
        /// 等待玩家的发言
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task WaitForSpeech(UCGame game, UCPlayer player, int waitSeconds)
        {
            int lastSeconds = waitSeconds;
            while (lastSeconds > 0)
            {
                var record = GetPlayerSpeech(player);
                if (record is not null) return;
                await game.CheckEndedAndDelay(1000);
                lastSeconds--;
            }
            throw new GameEndException($"玩家{player.MemberName}未能在指定时间内发言，游戏结束");
        }

        /// <summary>
        /// 等待所有玩家投票完毕
        /// </summary>
        /// <returns></returns>
        public async Task WaitForVote(UCGame game, List<UCPlayer> votePlayers, int waitSeconds)
        {
            int lastSeconds = waitSeconds;
            while (lastSeconds > 0)
            {
                if (Votes.Count >= votePlayers.Count) return;
                await game.CheckEndedAndDelay(1000);
                lastSeconds--;
            }
            throw new GameEndException($"部分玩家在指定时间内未进行投票，游戏结束");
        }

        /// <summary>
        /// 创建一个子轮
        /// </summary>
        /// <returns></returns>
        public UCRound CreateSubRound()
        {
            var subRound = new UCRound();
            SubRounds.Add(subRound);
            return subRound;
        }

        /// <summary>
        /// 轮结束
        /// </summary>
        /// <param name="outPlayer"></param>
        public void End(UCPlayer outPlayer)
        {
            outPlayer.SetOut();
            OutPlayers.Add(outPlayer);
        }

        /// <summary>
        /// 获取被投票数最多玩家
        /// </summary>
        /// <returns></returns>
        public List<UCVoteResult> GetMaxVotes()
        {
            var resultList = Votes.GroupBy(o => o.Target).Select(o => new UCVoteResult(o.Key, o.Count())).ToList();
            return resultList.Maxs(o => o.VoteNum);
        }

        /// <summary>
        /// 获取玩家发言记录
        /// </summary>
        /// <returns></returns>
        private UCSpeech GetPlayerSpeech(UCPlayer player)
        {
            lock (Speechs)
            {
                return Speechs.FirstOrDefault(o => o.Player.MemberId == player.MemberId);
            }
        }

        /// <summary>
        /// 获取玩家投票记录
        /// </summary>
        /// <returns></returns>
        private UCVote GetPlayerVote(UCPlayer player)
        {
            lock (Speechs)
            {
                return Votes.FirstOrDefault(o => o.Voter.MemberId == player.MemberId);
            }
        }


        



    }
}
