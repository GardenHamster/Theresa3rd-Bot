using SqlSugar;
using System.Text;
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
        /// 本轮可以发言的玩家
        /// </summary>
        public List<UCPlayer> SpeechPlayers { get; private set; } = new();

        /// <summary>
        /// 本轮可以投票的玩家
        /// </summary>
        public List<UCPlayer> VotePlayers { get; private set; } = new();

        /// <summary>
        /// 本轮的投票对象
        /// </summary>
        public List<UCPlayer> VoteTargets { get; private set; } = new();

        /// <summary>
        /// 出局成员
        /// </summary>
        public List<UCPlayer> OutPlayers { get; private set; } = new();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="speechPlayers"></param>
        /// <param name="votePlayers"></param>
        public UCRound(List<UCPlayer> speechPlayers, List<UCPlayer> votePlayers)
        {
            SpeechPlayers = speechPlayers.ToList();
            VotePlayers = votePlayers.ToList();
            VoteTargets = speechPlayers.ToList();
        }

        /// <summary>
        /// 判断本轮中某位玩家是否已经发言
        /// </summary>
        /// <returns></returns>
        public bool IsPlayerSpeeched(long memberId)
        {
            return Speechs.Any(o => o.Player.MemberId == memberId);
        }

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
                lastSeconds--;
                await game.CheckEndedAndDelay(1000);
                var speech = GetPlayerSpeech(player);
                if (speech is null) continue;
                if (await game.CheckSpeech(speech)) return;
            }
            throw new GameFailedException($"玩家{player.MemberName}未能在指定时间内发言，游戏结束");
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
                lastSeconds--;
                await game.CheckEndedAndDelay(1000);
                if (Votes.Count >= votePlayers.Count) return;
            }
            throw new GameFailedException($"部分玩家在指定时间内未进行投票，游戏结束");
        }

        /// <summary>
        /// 创建一个子轮
        /// </summary>
        /// <returns></returns>
        public UCRound CreateSubRound(List<UCPlayer> speechPlayers, List<UCPlayer> votePlayers)
        {
            var subRound = new UCRound(speechPlayers, votePlayers);
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
            return GetVotes().Maxs(o => o.VoteNum);
        }

        /// <summary>
        /// 获取被投票数最多玩家
        /// </summary>
        /// <returns></returns>
        public List<UCVoteResult> GetVotes()
        {
            return Votes.GroupBy(o => o.Target).Select(o => new UCVoteResult(o.Key, o.Count())).ToList();
        }

        /// <summary>
        /// 获取包含子轮的发言记录
        /// </summary>
        /// <param name="str"></param>
        /// <param name="similarity">0~1的小数</param>
        /// <returns></returns>
        public List<UCSpeech> GetSimilarSpeechs(string str, decimal similarity)
        {
            var speechs = new List<UCSpeech>();
            foreach (var speech in Speechs)
            {
                if (str.IsSimilar(speech.Content, similarity))
                {
                    speechs.Add(speech);
                }
            }
            foreach (var round in SubRounds)
            {
                speechs.AddRange(round.GetSimilarSpeechs(str, similarity));
            }
            return speechs;
        }

        /// <summary>
        /// 根据QQ获取玩家
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public UCPlayer GetVoter(long memberId)
        {
            return VotePlayers.FirstOrDefault(o => o.MemberId == memberId);
        }

        /// <summary>
        /// 根据Id获取玩家
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public UCPlayer GetVoteTarget(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId)) return null;
            int id = playerId.Trim().ToInt();
            if (id <= 0) return null;
            return VoteTargets.FirstOrDefault(o => o.PlayerId == id);
        }

        /// <summary>
        /// 获取玩家列表
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public string ListVoteTargets()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var player in VoteTargets)
            {
                if (builder.Length > 0) builder.AppendLine();
                builder.Append($"{player.PlayerId}：{player.MemberName}");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 返回票数统计信息
        /// </summary>
        /// <returns></returns>
        public string ListVotedCount()
        {
            var builder = new StringBuilder();
            foreach (var player in VoteTargets)
            {
                if (builder.Length > 0) builder.AppendLine();
                var count = Votes.Where(o => o.Target.MemberId == player.MemberId).Count();
                builder.Append($"{player.MemberName}：{count}票");
            }
            return builder.ToString();
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
