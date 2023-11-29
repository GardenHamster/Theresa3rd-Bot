using System.Numerics;
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
        /// 阵营角色数量比例(平民:卧底:白板)
        /// </summary>
        public int[] CampScales { get; init; }
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
        public UCWordPO GameWords { get; private set; }
        /// <summary>
        /// 平民词条
        /// </summary>
        public string CivWords { get; private set; }
        /// <summary>
        /// 卧底词条
        /// </summary>
        public string UCWords { get; private set; }
        /// <summary>
        /// 获胜阵营
        /// </summary>
        public UCCamp WinedCamp { get; private set; } = UCCamp.None;
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
        /// 游戏词条
        /// </summary>
        public UndercoverConfig UCConfig => BotConfig.GameConfig.Undercover;
        /// <summary>
        /// 存活的玩家
        /// </summary>
        public List<UCPlayer> LivePlayers => Players.Where(o => o.IsOut == false).ToList();
        /// <summary>
        /// 存活的平民
        /// </summary>
        public List<UCPlayer> LiveCivilians => Players.Where(o => o.IsOut == false && o.PlayerCamp == UCCamp.Civilian).ToList();
        /// <summary>
        /// 存活的卧底
        /// </summary>
        public List<UCPlayer> LiveUndercovers => Players.Where(o => o.IsOut == false && o.PlayerCamp == UCCamp.Undercover).ToList();
        /// <summary>
        /// 存活的白板
        /// </summary>
        public List<UCPlayer> LiveWhiteboards => Players.Where(o => o.IsOut == false && o.PlayerCamp == UCCamp.Whiteboard).ToList();
        /// <summary>
        /// 最小加入人数
        /// </summary>
        public override int MinPlayer { get; protected set; } = 4;
        /// <summary>
        /// 组队时间
        /// </summary>
        public override int MatchSecond { get; protected set; } = 120;

        /// <summary>
        /// 创建一个标准游戏
        /// </summary>
        public UCGame(GroupCommand command, BaseSession session, BaseReporter reporter) : base(command, session, reporter)
        {
            CivAmount = 3;
            UcAmount = 1;
            WbAmount = 0;
            MinPlayer = CivAmount + UcAmount + WbAmount;
            InitGame();
        }

        /// <summary>
        /// 创建一个自定义游戏
        /// </summary>
        /// <param name="civNum"></param>
        /// <param name="ucNum"></param>
        /// <param name="wbNum"></param>
        public UCGame(GroupCommand command, BaseSession session, BaseReporter reporter, int civNum, int ucNum, int wbNum) : base(command, session, reporter)
        {
            CivAmount = civNum >= 0 ? civNum : 0;
            UcAmount = ucNum >= 0 ? ucNum : 0;
            WbAmount = wbNum >= 0 ? wbNum : 0;
            MinPlayer = CivAmount + UcAmount + WbAmount;
            InitGame();
        }

        /// <summary>
        /// 创建一个自由加入的游戏
        /// </summary>
        /// <param name="civNum"></param>
        /// <param name="ucNum"></param>
        /// <param name="wbNum"></param>
        public UCGame(GroupCommand command, BaseSession session, BaseReporter reporter, int[] campScales) : base(command, session, reporter, true)
        {
            MinPlayer = 4;
            CampScales = campScales;
            InitGame();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <exception cref="GameException"></exception>
        private void InitGame()
        {
            MatchSecond = BotConfig.GameConfig.Undercover.MatchSeconds;
            if (MinPlayer < 4) throw new GameException("游戏创建失败，游戏至少需要4名玩家");
            if (MinPlayer < 5 && WbAmount > 0) throw new GameException("游戏创建失败，玩家达到5人及以上才可以加入白板");
            if (MatchSecond < 10) MatchSecond = 10;
        }

        /// <summary>
        /// 平民是否已经获胜(所有卧底和白板已经出局)
        /// </summary>
        public bool IsCivilianWin(ref string winMsg)
        {
            if (Players.Where(o => o.PlayerCamp != UCCamp.Civilian).All(o => o.IsOut))
            {
                winMsg = "所有卧底和白板出局，平民获胜！";
                return true;
            }
            return false;
        }

        /// <summary>
        /// 卧底是否已经获胜(所有平民白板出局或者剩下2人时还存在一个卧底)
        /// </summary>
        public bool IsUndercoverWin(ref string winMsg)
        {
            if (Players.Where(o => o.PlayerCamp != UCCamp.Undercover).All(o => o.IsOut))
            {
                winMsg = "所有平民和白板出局，卧底获胜！";
                return true;
            }
            if (LivePlayers.Count <= 2 && LiveUndercovers.Count >= 1)
            {
                winMsg = "剩余2人存活且卧底存活，卧底获胜！";
                return true;
            }
            return false;
        }

        /// <summary>
        /// 白板是否已经获胜(所有卧底出局且白板存活)
        /// </summary>
        public bool IsWhiteboardWin(ref string winMsg)
        {
            if (Players.Where(o => o.PlayerCamp == UCCamp.Undercover).All(o => o.IsOut) && LiveWhiteboards.Count >= 1)
            {
                winMsg = "所有卧底出局且白板存活，白板获胜！";
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否已有某个阵容获胜
        /// </summary>
        public async Task CheckSomeoneWinAsync()
        {
            string winMsg = string.Empty;
            if (IsCivilianWin(ref winMsg))
            {
                WinedCamp = UCCamp.Civilian;
                await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, winMsg);
                throw new GameFinishedException();
            }
            if (IsUndercoverWin(ref winMsg))
            {
                WinedCamp = UCCamp.Undercover;
                await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, winMsg);
                throw new GameFinishedException();
            }
            if (IsWhiteboardWin(ref winMsg))
            {
                WinedCamp = UCCamp.Whiteboard;
                await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, winMsg);
                throw new GameFinishedException();
            }
        }

        /// <summary>
        /// 处理游戏消息
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public override async Task<bool> HandleGameMessageAsync(GroupRelay relay)
        {
            if (IsEnded) return false;
            if (IsStarted == false) return false;
            if (CurrentRound is null) return false;
            if (SpeakingPlayer is not null)
            {
                return await HandleSpeechMessageAsync(relay);
            }
            if (IsVoting)
            {
                return await HandleVoteMessage(relay);
            }
            return false;
        }

        /// <summary>
        /// 处理发言消息
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        private async Task<bool> HandleSpeechMessageAsync(GroupRelay relay)
        {
            if (SpeakingPlayer.MemberId != relay.MemberId) return false;
            if (relay.IsAt == false && relay.IsQuote == false) return false;
            if (relay.IsQuote && WaitingQuote is not null && relay.QuoteMsgId != 0 && relay.QuoteMsgId != WaitingQuote.MessageId) return false;
            if (CurrentRound.IsPlayerSpeeched(relay.MemberId)) return false;
            var similarSpeechs = GetSimilarSpeech(relay, UCConfig.MaxSimilarity / 100);
            if (similarSpeechs.Count > 0)
            {
                List<BaseContent> remindContents = new List<BaseContent>();
                remindContents.Add(new PlainContent($"存在相似的历史发言，请重新发言~"));
                remindContents.Add(new PlainContent(GetSimilarContent(similarSpeechs)));
                await Session.SendGroupMessageWithAtAsync(GroupId, relay.MemberId, remindContents);
                return true;
            }
            CurrentRound.AddPlayerSpeech(SpeakingPlayer, relay);
            return true;
        }

        /// <summary>
        /// 处理投票消息
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        private async Task<bool> HandleVoteMessage(GroupRelay relay)
        {
            var voter = CurrentRound.GetVoter(relay.MemberId);
            if (voter is null) return false;
            if (voter.IsOut) return false;
            var target = CurrentRound.GetVoteTarget(relay.Message);
            if (target is null) return false;
            if (target.IsOut) return false;
            if (voter.MemberId == target.MemberId)
            {
                await Session.SendGroupMessageWithAtAsync(GroupId, relay.MemberId, "不能对自己进行投票，请重新投票");
                return true;
            }
            var vote = CurrentRound.AddPlayerVote(voter, target);
            if (vote is not null)
            {
                List<BaseContent> contents = new List<BaseContent>();
                contents.Add(new PlainContent("投票成功！当前票数为："));
                contents.Add(new PlainContent(CurrentRound.ListVotedCount()));
                await Session.SendGroupMessageWithAtAsync(GroupId, relay.MemberId, contents);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 游戏创建完毕事件
        /// </summary>
        /// <returns></returns>
        public override async Task GameCreatedAsync(GroupCommand command)
        {
            AddPlayer(new UCPlayer(command.MemberId, command.MemberName));
            await Session.SendGroupMessageWithAtAsync(GroupId, command.MemberId, $"{GameName}游戏创建完毕，已为你自动加入游戏...");
            await Task.Delay(1000);
            await Session.SendGroupTemplateAsync(GroupId, UCConfig.RuleMsg);
        }

        /// <summary>
        /// 玩家匹配完毕事件
        /// </summary>
        /// <returns></returns>
        public override async Task PlayerWaitingCompletedAsync()
        {
            int players = Players.Count;
            if (players < MinPlayer) throw new GameException($"游戏匹配失败，最低加入人数为{MinPlayer}人~");
            if (CampScales is null) throw new GameException($"游戏匹配失败，未指定阵营人数比例~");
            int civScale = CampScales[0];
            int ucScale = CampScales[1];
            int wbScale = CampScales[2];
            int sumScale = civScale + ucScale + wbScale;
            if (players < sumScale) throw new GameException($"阵营分配失败，当前游戏人数不足{sumScale}人~");
            int groups = players / sumScale;
            CivAmount = groups * civScale + players % sumScale;
            UcAmount = groups * ucScale;
            WbAmount = groups * wbScale;
            await Task.CompletedTask;
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
            await FetchWords();//获取随机词条
            await CheckEnded();
            await DistWords();//派发随机身份
            await CheckEnded();
            await SendWords();//私聊发送词条
            await CheckEnded();
            await Session.SendGroupMessageAsync(GroupId, $"词条派发完毕，请各位查看私聊\r\n如未收到私聊，请尝试添加本Bot为好友，然后在群内发送 {UCConfig.SendWordCommands.JoinCommands()} 重新私发词条。\r\n请各位组织语言，发言环节将在{UCConfig.PrepareSeconds}秒后开始...");
            await Task.Delay(UCConfig.PrepareSeconds * 1000);
            await CheckEnded();
            while (true)
            {
                var parentRound = new UCRound(LivePlayers, LivePlayers);
                var parentMemberIds = parentRound.VotePlayers.Select(o => o.MemberId).ToList();
                CurrentRound = parentRound;
                GameRounds.Add(CurrentRound);
                await Session.SendGroupMessageWithAtAsync(GroupId, parentMemberIds, $"现在开始第 {GameRounds.Count} 轮发言，请各位在收到艾特消息后再进行发言...");
                await CheckEndedAndDelay(1000);
                await PlayersSpeech(CurrentRound);//发言环节
                await CheckEndedAndDelay(1000);
                await Session.SendGroupMessageAsync(GroupId, "本轮发言完毕，以下是本轮的发言记录");
                await CheckEndedAndDelay(1000);
                await SendSpeechHistory(CurrentRound);
                await CheckEndedAndDelay(1000);
                var voteResults = await PlayersVote(CurrentRound);//投票环节
                while (voteResults.Count > 1)
                {
                    await CheckEnded();
                    var subPlayers = voteResults.Select(o => o.Player).ToList();
                    CurrentRound = CurrentRound.CreateSubRound(subPlayers, LivePlayers);
                    var subMemberIds = CurrentRound.VotePlayers.Select(o => o.MemberId).ToList();
                    await Session.SendGroupMessageWithAtAsync(GroupId, subMemberIds, $"投票后仍有{subMemberIds.Count}位玩家票数相同，请{subMemberIds.Count}位玩家按照提示继续发言...");
                    await CheckEndedAndDelay(1000);
                    await PlayersSpeech(CurrentRound);//发言环节
                    await CheckEndedAndDelay(1000);
                    await Session.SendGroupMessageAsync(GroupId, "本轮发言完毕，以下是轮的发言记录");
                    await CheckEndedAndDelay(1000);
                    await SendSpeechHistory(CurrentRound);
                    await CheckEndedAndDelay(1000);
                    voteResults = await PlayersVote(CurrentRound);//投票环节
                }

                var outPlayer = voteResults.First().Player;
                parentRound.End(outPlayer);
                await Session.SendGroupMessageAsync(GroupId, $"玩家{outPlayer.NameAndQQ}出局");
                await CheckSomeoneWinAsync();//检查是否有某个阵营已经获胜
                await CheckEndedAndDelay(1000);
                await Session.SendGroupMessageAsync(GroupId, $"游戏继续...");
                await CheckEndedAndDelay(1000);
            }
        }

        /// <summary>
        /// 游戏结束阶段事件
        /// </summary>
        /// <returns></returns>
        public override async Task GameFinishingAsync()
        {
            if (WinedCamp == UCCamp.None) throw new GameException("游戏异常，未指定获胜阵营");
            await Session.SendGroupMessageAsync(GroupId, GetCampInfos());
            await Task.Delay(1000);
            if (UCConfig.MuteSeconds > 0)
            {
                var failPlayers = Players.Where(o => o.PlayerCamp != WinedCamp).ToList();
                var failMemberIds = failPlayers.Select(o => o.MemberId).ToList();
                await Session.SendGroupMessageAsync(GroupId, $"非获胜阵营将自动禁言{UCConfig.MuteSeconds}秒");
                await Task.Delay(1000);
                await Session.MuteGroupMemberAsync(GroupId, failMemberIds, UCConfig.MuteSeconds);
            }
        }

        /// <summary>
        /// 判断玩家是否已经加入游戏
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public override bool IsMemberJoined(long memberId)
        {
            return Players.Any(o => o.MemberId == memberId);
        }

        /// <summary>
        /// 随机获取词条
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GameException"></exception>
        private async Task FetchWords()
        {
            var excludeMembers = MemberIds.Where(o => o.IsSuperManager() == false).ToList();
            GameWords = new UCWordService().GetRandomWord(excludeMembers);
            if (GameWords is null) throw new GameException("无法获取词条，请艾特管理员添加和审批更多词条");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 派发随机身份
        /// </summary>
        /// <returns></returns>
        private async Task DistWords()
        {
            if (RandomHelper.RandomBetween(0, 99) < 50)
            {
                UCWords = GameWords.Word1;
                CivWords = GameWords.Word2;
            }
            else
            {
                UCWords = GameWords.Word2;
                CivWords = GameWords.Word1;
            }
            for (int i = 0; i < WbAmount; i++)
            {
                //前两名玩家不能设置为白板
                var player = Players.Skip(2).Where(o => o.PlayerCamp == UCCamp.None).ToList().RandomItem();
                player.PlayerCamp = UCCamp.Whiteboard;
                player.PlayerWord = string.Empty;
            }
            for (int i = 0; i < CivAmount; i++)
            {
                var player = Players.Where(o => o.PlayerCamp == UCCamp.None).ToList().RandomItem();
                player.PlayerCamp = UCCamp.Civilian;
                player.PlayerWord = CivWords;
            }
            for (int i = 0; i < UcAmount; i++)
            {
                var player = Players.Where(o => o.PlayerCamp == UCCamp.None).ToList().RandomItem();
                player.PlayerCamp = UCCamp.Undercover;
                player.PlayerWord = UCWords;
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 私聊发送词条
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 发言流程
        /// </summary>
        /// <param name="round"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 检查发言
        /// </summary>
        /// <param name="speech"></param>
        /// <returns></returns>
        /// <exception cref="GameFinishedException"></exception>
        public async Task<bool> CheckSpeech(UCSpeech speech)
        {
            var player = speech.Player;
            if (player.PlayerCamp == UCCamp.Whiteboard && speech.Content.Contains(CivWords))
            {
                await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, $"白板成功猜出了平民的词组");
                throw new GameFinishedException();
            }
            if (player.PlayerCamp == UCCamp.Whiteboard && speech.Content.Contains(UCWords))
            {
                await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, $"白板成功猜出了卧底的词组");
                throw new GameFinishedException();
            }
            if (player.PlayerCamp != UCCamp.Whiteboard && speech.Content.Contains(player.PlayerWord))
            {
                player.SetOut();
                await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, $"玩家{player.NameAndQQ}的发言中包含了自己的词条，判定出局");
                return true;
            }
            return true;
        }

        /// <summary>
        /// 根据百分比获取相似的发言
        /// </summary>
        /// <param name="content"></param>
        /// <param name="similarity">0~1的小数</param>
        /// <returns></returns>
        private List<UCSpeech> GetSimilarSpeech(GroupRelay relay, decimal similarity)
        {
            var speechs = new List<UCSpeech>();
            foreach (var round in GameRounds)
            {
                speechs.AddRange(round.GetSimilarSpeechs(relay.Message, similarity));
            }
            return speechs;
        }

        /// <summary>
        /// 获取相似的发言记录
        /// </summary>
        /// <param name="similarSpeechs"></param>
        /// <returns></returns>
        private string GetSimilarContent(List<UCSpeech> similarSpeechs)
        {
            var builder = new StringBuilder();
            foreach (var speech in similarSpeechs)
            {
                if (builder.Length > 0) builder.AppendLine();
                builder.Append($"{speech.Player.NameAndQQ}：{speech.Content}");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 发送某一轮的发言记录
        /// </summary>
        /// <param name="round"></param>
        /// <returns></returns>
        private async Task SendSpeechHistory(UCRound round)
        {
            var contents = new List<ForwardContent>();
            foreach (var speech in round.Speechs)
            {
                var player = speech.Player;
                var content = new ForwardContent(player.MemberId, player.MemberName, speech.Content);
                contents.Add(content);
            }
            await Session.SendGroupForwardAsync(GroupId, contents);
        }

        /// <summary>
        /// 投票流程
        /// </summary>
        /// <param name="round"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取阵营信息
        /// </summary>
        /// <returns></returns>
        private string GetCampInfos()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("本次游戏阵容如下：");
            var cvPlayer = Players.Where(o => o.PlayerCamp == UCCamp.Civilian).ToList();
            var ucPlayer = Players.Where(o => o.PlayerCamp == UCCamp.Undercover).ToList();
            var wbPlayer = Players.Where(o => o.PlayerCamp == UCCamp.Whiteboard).ToList();
            string cvInfo = GetCampInfo(cvPlayer, "平民");
            string ucInfo = GetCampInfo(ucPlayer, "卧底");
            string wbInfo = GetCampInfo(wbPlayer, "白板");
            if (!string.IsNullOrEmpty(cvInfo)) builder.AppendLine(cvInfo);
            if (!string.IsNullOrEmpty(ucInfo)) builder.AppendLine(ucInfo);
            if (!string.IsNullOrEmpty(wbInfo)) builder.Append(wbInfo);
            return builder.ToString();
        }

        /// <summary>
        /// 获取阵营信息
        /// </summary>
        /// <param name="players"></param>
        /// <param name="campName"></param>
        /// <returns></returns>
        private string GetCampInfo(List<UCPlayer> players, string campName)
        {
            if (players.Count == 0) return string.Empty;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{campName}：");
            for (int i = 0; i < players.Count; i++)
            {
                if (i > 0) builder.AppendLine();
                builder.Append($"  {players[i].MemberName}({players[i].MemberId})");
            }
            return builder.ToString();
        }

    }
}
