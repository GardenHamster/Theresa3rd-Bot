using System.Text;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Relay;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Game.Undercover
{
    public class UCGame : BaseGroupGame<UCPlayer>
    {
        /// <summary>
        /// 游戏名称
        /// </summary>
        public override string GameName { get; } = "谁是卧底";
        /// <summary>
        /// 平民数量
        /// </summary>
        public int CivAmount { get; private set; } = 2;
        /// <summary>
        /// 卧底数量
        /// </summary>
        public int UcAmount { get; private set; } = 1;
        /// <summary>
        /// 白板数量
        /// </summary>
        public int WbAmount { get; private set; } = 0;
        /// <summary>
        /// 游戏词条
        /// </summary>
        public UCWordPO UCWord { get; private set; }
        /// <summary>
        /// 轮合集
        /// </summary>
        public List<UCRound> GameRounds { get; private set; } = new();
        /// <summary>
        /// 当前轮
        /// </summary>
        public UCRound CurrentRound { get; private set; }
        /// <summary>
        /// 当前发言玩家
        /// </summary>
        public UCPlayer SpeakingPlayer { get; private set; }
        /// <summary>
        /// 当前等待被回复的消息
        /// </summary>
        public BaseResult WaitingQuote { get; private set; }
        /// <summary>
        /// 是否正在投票环节中
        /// </summary>
        public bool IsVoting { get; private set; }
        /// <summary>
        /// 存活的玩家
        /// </summary>
        public List<UCPlayer> LivePlayers => Players.Where(o => o.IsOut == false).ToList();
        /// <summary>
        /// 游戏词条
        /// </summary>
        public UndercoverConfig UCConfig => BotConfig.GameConfig.Undercover;
        /// <summary>
        /// 是否已有某个阵容获胜
        /// </summary>
        public bool IsSomeoneWin => IsCivilianWin || IsUndercoverWin || IsWhiteboardWin;
        /// <summary>
        /// 平民是否已经获胜
        /// </summary>
        public bool IsCivilianWin => Players.Where(o => o.PlayerCamp == UCPlayerCamp.Undercover).All(o => o.IsOut);
        /// <summary>
        /// 卧底是否已经获胜
        /// </summary>
        public bool IsUndercoverWin => Players.Where(o => o.IsOut == false).All(o => o.PlayerCamp == UCPlayerCamp.Undercover);
        /// <summary>
        /// 白板是否已经获胜
        /// </summary>
        public bool IsWhiteboardWin => Players.Where(o => o.IsOut == false).All(o => o.PlayerCamp == UCPlayerCamp.Whiteboard);
        /// <summary>
        /// 最小加入人数
        /// </summary>
        public override int MinPlayer { get; protected set; } = 5;
        /// <summary>
        /// 组队时间
        /// </summary>
        public override int MatchSecond { get; protected set; } = 120;

        /// <summary>
        /// 创建一个标准游戏
        /// </summary>
        public UCGame(GroupCommand command, BaseSession session, BaseReporter reporter) : base(command, session, reporter)
        {
            CivAmount = 1;
            //CivAmount = 3;
            UcAmount = 1;
            WbAmount = 1;
            MinPlayer = CivAmount + UcAmount + WbAmount;
            MatchSecond = BotConfig.GameConfig.Undercover.MatchSeconds;
        }

        /// <summary>
        /// 创建一个自定义游戏
        /// </summary>
        /// <param name="civNum"></param>
        /// <param name="ucNum"></param>
        /// <param name="wbNum"></param>
        public UCGame(GroupCommand command, BaseSession session, BaseReporter reporter, int civNum, int ucNum, int wbNum) : base(command, session, reporter)
        {
            CivAmount = civNum;
            UcAmount = ucNum;
            WbAmount = wbNum;
            MinPlayer = CivAmount + UcAmount + WbAmount;
            MatchSecond = BotConfig.GameConfig.Undercover.MatchSeconds;
        }

        /// <summary>
        /// 处理游戏消息
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public override bool HandleGameMessage(GroupRelay relay)
        {
            lock (this)
            {
                if (IsEnded) return false;
                if (IsStarted == false) return false;
                if (CurrentRound is null) return false;
                if (SpeakingPlayer is not null)
                {
                    if (SpeakingPlayer.MemberId != relay.MemberId) return false;
                    if (relay.IsAt == false && relay.IsQuote == false) return false;
                    if (relay.IsQuote && WaitingQuote is not null && relay.QuoteMsgId != 0 && relay.QuoteMsgId != WaitingQuote.MessageId) return false;
                    var record = CurrentRound.AddPlayerSpeech(SpeakingPlayer, relay);
                    return record is not null;
                }
                if (IsVoting)
                {
                    var voter = CurrentRound.GetVoter(relay.MemberId);
                    if (voter is null) return false;
                    if (voter.IsOut) return false;
                    var target = CurrentRound.GetVoteTarget(relay.Message);
                    if (target is null) return false;
                    if (target.IsOut) return false;
                    if (voter.MemberId == target.MemberId)
                    {
                        Session.SendGroupMessageWithAtAsync(GroupId, relay.MemberId, "不能对自己进行投票，请重新投票");
                        return true;
                    }
                    var vote = CurrentRound.AddPlayerVote(voter, target);
                    if (vote is not null)
                    {
                        List<BaseContent> contents = new List<BaseContent>();
                        contents.Add(new PlainContent("投票成功！当前票数为："));
                        contents.Add(new PlainContent(CurrentRound.ListVotedCount()));
                        Session.SendGroupMessageWithAtAsync(GroupId, relay.MemberId, contents);
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 处理游戏流程
        /// </summary>
        /// <returns></returns>
        public override async Task GameProcessingAsync()
        {
            await CheckEnded();
            await Session.SendGroupMessageAsync(GroupId, "游戏开始，正在获取词条...");
            await CheckEnded();
            await FetchWords();//随机获取词条
            await CheckEnded();
            await DistWords();//派发随机身份
            await CheckEnded();
            await SendWords();//私聊发送词条
            await CheckEnded();
            await Session.SendGroupMessageAsync(GroupId, $"词条派发完毕，请各位查看私聊，如未收到私聊，请尝试添加本Bot为好友，然后在群内发送 {UCConfig.SendWordCommands.JoinCommands()} 重新私发词条。请各位组织语言，发言环节将在{UCConfig.PrepareSeconds}秒后开始...");
            await Task.Delay(UCConfig.PrepareSeconds * 1000);
            await CheckEnded();
            while (IsSomeoneWin == false)
            {
                CurrentRound = new UCRound(LivePlayers, LivePlayers);
                GameRounds.Add(CurrentRound);
                await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, $"现在开始第 {GameRounds.Count} 轮发言，请各位在收到艾特消息后再进行发言...");
                await CheckEndedAndDelay(1000);
                await PlayersSpeech(CurrentRound);//发言环节
                await CheckEndedAndDelay(1000);
                var voteResults = await PlayersVote(CurrentRound);//投票环节
                while (voteResults.Count > 1)
                {
                    await CheckEnded();
                    var subPlayers = voteResults.Select(o => o.Player).ToList();
                    var subMemberIds = voteResults.Select(o => o.Player.MemberId).ToList();
                    CurrentRound = CurrentRound.CreateSubRound(subPlayers,LivePlayers);
                    await Session.SendGroupMessageWithAtAsync(GroupId, subMemberIds, $"投票后仍有{subMemberIds.Count}位玩家票数相同，请{subMemberIds.Count}位玩家按照提示继续发言...");
                    await CheckEndedAndDelay(1000);
                    await PlayersSpeech(CurrentRound);//发言环节
                    await CheckEndedAndDelay(1000);
                    voteResults = await PlayersVote(CurrentRound);//投票环节
                }
                var outPlayer = voteResults.First().Player;
                CurrentRound.End(outPlayer);
                await CheckEndedAndDelay(1000);
            }
        }

        public override async Task GameEndingAsync()
        {
            var winMessage = string.Empty;
            var WinCamp = UCPlayerCamp.None;

            if (IsCivilianWin)
            {
                WinCamp = UCPlayerCamp.Civilian;
                winMessage = "平民干掉了所有卧底，平民获胜！";
            }
            else if (IsUndercoverWin)
            {
                WinCamp = UCPlayerCamp.Undercover;
                winMessage = "卧底干掉了所有人，卧底获胜！";
            }
            else if (IsWhiteboardWin)
            {
                WinCamp = UCPlayerCamp.Whiteboard;
                winMessage = "白板干掉了所有人，白板获胜！";
            }

            await Session.SendGroupMessageAsync(GroupId, $"游戏结束，{winMessage}");
            await Task.Delay(1000);

            await Session.SendGroupMessageAsync(GroupId, GetCampInfos());
            await Task.Delay(1000);

            if (UCConfig.MuteSeconds > 0)
            {
                var failPlayers = Players.Where(o => o.PlayerCamp != WinCamp).ToList();
                var failMemberIds = failPlayers.Select(o => o.MemberId).ToList();
                await Session.SendGroupMessageAsync(GroupId, $"非获胜阵营将自动禁言{UCConfig.MuteSeconds}秒");
                await Task.Delay(1000);
                await Session.MuteGroupMemberAsync(GroupId, failMemberIds, UCConfig.MuteSeconds);
            }
        }

        private async Task FetchWords()
        {
            var excludeMembers = MemberIds.Where(o => o.IsSuperManager() == false).ToList();
            UCWord = new UCWordService().GetRandomWord(excludeMembers);
            if (UCWord is null) throw new GameException("无法获取词条，请艾特管理员添加和审批更多词条");
            await Task.CompletedTask;
        }

        private async Task DistWords()
        {
            for (int i = 0; i < CivAmount; i++)
            {
                var player = Players.Where(o => o.PlayerCamp == UCPlayerCamp.None).ToList().RandomItem();
                player.PlayerCamp = UCPlayerCamp.Civilian;
                player.PlayerWord = UCWord.Word1;
            }
            for (int i = 0; i < UcAmount; i++)
            {
                var player = Players.Where(o => o.PlayerCamp == UCPlayerCamp.None).ToList().RandomItem();
                player.PlayerCamp = UCPlayerCamp.Undercover;
                player.PlayerWord = UCWord.Word2;
            }
            for (int i = 0; i < WbAmount; i++)
            {
                var player = Players.Where(o => o.PlayerCamp == UCPlayerCamp.None).ToList().RandomItem();
                player.PlayerCamp = UCPlayerCamp.Whiteboard;
                player.PlayerWord = string.Empty;
            }
            await Task.CompletedTask;
        }

        private async Task SendWords()
        {
            await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, "正在派发词条，请各位玩家查看私聊获取自己的词条...");
            foreach (var player in Players)
            {
                await CheckEnded();
                await Session.SendTempMessageAsync(GroupId, player.MemberId, player.GetWordMessage());
                await Task.Delay(1000);
            }
        }

        private async Task PlayersSpeech(UCRound round)
        {
            foreach (var player in round.SpeechPlayers)
            {
                try
                {
                    await CheckEnded();
                    SpeakingPlayer = player;
                    WaitingQuote = await Session.SendGroupMessageWithAtAsync(GroupId, player.MemberId, $"请在{UCConfig.SpeakingSeconds}秒内艾特或回复本条消息用文字描述你的内容");
                    await round.WaitForSpeech(this, player, UCConfig.SpeakingSeconds);
                    await Task.Delay(1000);
                }
                finally
                {
                    SpeakingPlayer = null;
                    WaitingQuote = null;
                }
            }
        }

        private async Task<List<UCVoteResult>> PlayersVote(UCRound round)
        {
            try
            {
                IsVoting = true;
                await CheckEnded();
                var votePlayers = round.VotePlayers;
                var contents = new List<BaseContent>();
                contents.Add(new PlainContent($"下面是投票环节，请各位在{UCConfig.VotingSeconds}秒内发送数字选择投票对象"));
                contents.Add(new PlainContent(CurrentRound.ListVoteTargets()));
                var voteMembers = votePlayers.Select(o => o.MemberId).ToList();
                await Session.SendGroupMessageWithAtAsync(GroupId, voteMembers, contents);
                await round.WaitForVote(this, votePlayers, UCConfig.VotingSeconds);
                return round.GetMaxVotes();
            }
            finally
            {
                IsVoting = false;
            }
        }

        private string GetCampInfos()
        {
            var builder = new StringBuilder();
            var cvPlayer = Players.Where(o => o.PlayerCamp == UCPlayerCamp.Civilian).ToList();
            var ucPlayer = Players.Where(o => o.PlayerCamp == UCPlayerCamp.Undercover).ToList();
            var wbPlayer = Players.Where(o => o.PlayerCamp == UCPlayerCamp.Whiteboard).ToList();
            string cvInfo = GetCampInfo(cvPlayer, "平民");
            string ucInfo = GetCampInfo(ucPlayer, "卧底");
            string wbInfo = GetCampInfo(wbPlayer, "白板");
            builder.AppendLine("本次游戏阵容如下：");
            if (!string.IsNullOrEmpty(cvInfo)) builder.AppendLine(cvInfo);
            if (!string.IsNullOrEmpty(ucInfo)) builder.AppendLine(ucInfo);
            if (!string.IsNullOrEmpty(wbInfo)) builder.AppendLine(wbInfo);
            return builder.ToString();
        }

        private string GetCampInfo(List<UCPlayer> players, string campName)
        {
            var builder = new StringBuilder();
            if (players.Count > 0)
            {
                builder.AppendLine($"{campName}：");
            }
            foreach (var player in players)
            {
                builder.AppendLine($"  {player.MemberName}({player.MemberId})");
            }
            return builder.ToString();
        }

    }
}
