using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Game.Undercover
{
    public class UndercoverGame : BaseGroupGame
    {
        /// <summary>
        /// 发言时长(秒)
        /// </summary>
        public int SpeakingSeconds { get; set; } = 120;
        /// <summary>
        /// 平民数量
        /// </summary>
        public int CivilianAmount { get; set; } = 2;
        /// <summary>
        /// 卧底数量
        /// </summary>
        public int UndercoverAmount { get; set; } = 1;
        /// <summary>
        /// 白板数量
        /// </summary>
        public int WhiteboardAmount { get; set; } = 0;
        /// <summary>
        /// 玩家
        /// </summary>
        public List<UndercoverPlayer> Players { get; private set; } = new();
        /// <summary>
        /// 游戏轮
        /// </summary>
        public List<UndercoverRound> Rounds { get; private set; } = new();

        /// <summary>
        /// 创建一个标准游戏
        /// </summary>
        public UndercoverGame()
        {
            CivilianAmount = 2;
            UndercoverAmount = 1;
            WhiteboardAmount = 0;
            MinMember = CivilianAmount + UndercoverAmount + WhiteboardAmount;
        }

        /// <summary>
        /// 创建一个自定义游戏
        /// </summary>
        /// <param name="civNum"></param>
        /// <param name="ucNum"></param>
        /// <param name="wbNum"></param>
        public UndercoverGame(int civNum, int ucNum, int wbNum)
        {
            CivilianAmount = civNum;
            UndercoverAmount = ucNum;
            WhiteboardAmount = wbNum;
            MinMember = CivilianAmount + UndercoverAmount + WhiteboardAmount;
        }

        /// <summary>
        /// 启动游戏线程并等待处理完成
        /// </summary>
        /// <returns></returns>
        public override async Task StartProcessing()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 处理游戏进程
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public override bool HandleProcessing(GroupRelay relay)
        {
            return false;
        }

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

    }
}
