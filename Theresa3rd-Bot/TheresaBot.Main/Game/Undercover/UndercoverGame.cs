using TheresaBot.Main.Command;
using TheresaBot.Main.Relay;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Services;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Config;
using System.Text;
using TheresaBot.Main.Model.Content;

namespace TheresaBot.Main.Game.Undercover
{
    public class UndercoverGame : BaseGroupGame<UndercoverPlayer>
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
        public List<UndercoverRound> GameRounds { get; private set; } = new();
        /// <summary>
        /// 当前轮
        /// </summary>
        public UndercoverRound CurrentRound { get; private set; }
        /// <summary>
        /// 当前发言玩家
        /// </summary>
        public UndercoverPlayer SpeakingPlayer { get; private set; }
        /// <summary>
        /// 是否正在投票环节中
        /// </summary>
        public bool IsVoting { get; private set; }
        /// <summary>
        /// 存活的玩家
        /// </summary>
        public List<UndercoverPlayer> LivePlayers => Players.Where(o => o.IsOut == false).ToList();
        /// <summary>
        /// 游戏词条
        /// </summary>
        public UndercoverConfig UCConfig => BotConfig.GameConfig?.Undercover;
        /// <summary>
        /// 是否已有某个阵容获胜
        /// </summary>
        public bool IsSomeoneWin => IsCivilianWin || IsUndercoverWin || IsWhiteboardWin;
        /// <summary>
        /// 平民是否已经获胜
        /// </summary>
        public bool IsCivilianWin => Players.Where(o => o.PlayerType == UndercoverPlayerType.Undercover).All(o => o.IsOut);
        /// <summary>
        /// 卧底是否已经获胜
        /// </summary>
        public bool IsUndercoverWin => Players.Where(o => o.IsOut == false).All(o => o.PlayerType == UndercoverPlayerType.Undercover);
        /// <summary>
        /// 白板是否已经获胜
        /// </summary>
        public bool IsWhiteboardWin => Players.Where(o => o.IsOut == false).All(o => o.PlayerType == UndercoverPlayerType.Whiteboard);

        /// <summary>
        /// 创建一个标准游戏
        /// </summary>
        public UndercoverGame(GroupCommand command, BaseSession session, BaseReporter reporter) : base(command, session, reporter)
        {
            CivAmount = 2;
            UcAmount = 1;
            WbAmount = 0;
            MinPlayer = CivAmount + UcAmount + WbAmount;
        }

        /// <summary>
        /// 创建一个自定义游戏
        /// </summary>
        /// <param name="civNum"></param>
        /// <param name="ucNum"></param>
        /// <param name="wbNum"></param>
        public UndercoverGame(GroupCommand command, BaseSession session, BaseReporter reporter, int civNum, int ucNum, int wbNum) : base(command, session, reporter)
        {
            CivAmount = civNum;
            UcAmount = ucNum;
            WbAmount = wbNum;
            MinPlayer = CivAmount + UcAmount + WbAmount;
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
                if (CurrentRound is null) return false;
                if (SpeakingPlayer is not null)
                {
                    if (SpeakingPlayer.MemberId != relay.MemberId) return false;
                    var record = CurrentRound.AddPlayerRecord(SpeakingPlayer, relay);
                    return record is not null;
                }
                if (IsVoting)
                {
                    var voter = GetPlayer(relay.MemberId);
                    if (voter is null) return false;
                    var target = GetPlayer(relay.Message);
                    if (target is null) return false;
                    if (voter.MemberId == target.MemberId)
                    {
                        Session.SendGroupMessageWithAtAsync(GroupId, relay.MemberId, "不能对自己进行投票");
                        return true;
                    }
                    var vote = CurrentRound.AddPlayerVote(voter, target);
                    return vote is not null;
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
            if (IsEnded) throw new GameStopException();
            await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, "游戏开始，正在获取词条...");
            await FetchWords();//随机获取词条
            if (IsEnded) throw new GameStopException();
            await DistWords();//派发随机身份
            if (IsEnded) throw new GameStopException();
            await SendWords();//私聊发送词条
            if (IsEnded) throw new GameStopException();
            await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, $"词条派发完毕，请各位查看私聊，如未收到私聊，请尝试添加本Bot为好友，然后在群内发送 {UCConfig.WordCommands.JoinCommands()} 重新私发词条。请各位组织语言，发言环节将在{UCConfig.PrepareSeconds}秒后开始...");
            await Task.Delay(UCConfig.PrepareSeconds * 1000);
            while (IsSomeoneWin == false)
            {
                CurrentRound = new UndercoverRound();
                GameRounds.Add(CurrentRound);
                await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, $"现在开始第{GameRounds.Count + 1}轮发言，请各位在收到艾特消息后再进行发言...");
                await PlayersSpeech();//发言环节
                await Task.Delay(1000);
                await PlayersVote();//投票环节
                await Task.Delay(1000);
            }
        }

        private async Task PlayersVote()
        {
            List<BaseContent> contents=new List<BaseContent>();
            contents.Add(new PlainContent($"下面是投票环节，请各位在{UCConfig.VotingSeconds}秒内发送数字选择投票对象"));

            await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, );
            await CurrentRound.WaitForVote(LivePlayers,UCConfig.VotingSeconds);
        }

        private async Task PlayersSpeech()
        {
            foreach (var player in Players)
            {
                SpeakingPlayer = player;
                await Session.SendGroupMessageWithAtAsync(GroupId, player.MemberId, $"请在{UCConfig.SpeakingSeconds}秒内发送文字描述你的内容");
                await CurrentRound.WaitForSpeech(player, UCConfig.SpeakingSeconds);
                SpeakingPlayer = null;
                await Task.Delay(1000);
            }
        }

        private async Task FetchWords()
        {
            UCWord = new UCWordService().GetRandomWord(MemberIds);
            if (UCWord is null) throw new GameException("无法获取词条，请艾特管理员添加和审批更多词条");
            await Task.CompletedTask;
        }

        private async Task DistWords()
        {
            for (int i = 0; i < CivAmount; i++)
            {
                if (IsEnded) throw new GameStopException();
                var player = Players.Where(o => o.PlayerType == UndercoverPlayerType.None).ToList().RandomItem();
                player.PlayerType = UndercoverPlayerType.Civilian;
                player.PlayerWord = UCWord.Word1;
            }
            for (int i = 0; i < UcAmount; i++)
            {
                if (IsEnded) throw new GameStopException();
                var player = Players.Where(o => o.PlayerType == UndercoverPlayerType.None).ToList().RandomItem();
                player.PlayerType = UndercoverPlayerType.Undercover;
                player.PlayerWord = UCWord.Word2;
            }
            for (int i = 0; i < WbAmount; i++)
            {
                if (IsEnded) throw new GameStopException();
                var player = Players.Where(o => o.PlayerType == UndercoverPlayerType.None).ToList().RandomItem();
                player.PlayerType = UndercoverPlayerType.Whiteboard;
                player.PlayerWord = string.Empty;
            }
            await Task.CompletedTask;
        }

        private async Task SendWords()
        {
            await Session.SendGroupMessageWithAtAsync(GroupId, MemberIds, "正在派发词条，请各位玩家查看私聊获取自己的词条...");
            foreach (var player in Players)
            {
                if (IsEnded) throw new GameStopException();
                await Session.SendTempMessageAsync(GroupId, player.MemberId, player.GetWordMessage());
                await Task.Delay(1000);
            }
        }









    }
}
