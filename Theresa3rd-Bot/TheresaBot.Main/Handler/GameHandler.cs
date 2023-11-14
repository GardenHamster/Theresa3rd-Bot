using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Game;
using TheresaBot.Main.Game.Undercover;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Process;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type.GameOptions;

namespace TheresaBot.Main.Handler
{
    internal class GameHandler : BaseHandler
    {
        public GameHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
        }

        public async Task CreateUndercover(GroupCommand command)
        {
            UCGameMode gameMode = UCGameMode.Default;
            UndercoverGame game = new UndercoverGame(command, Session, Reporter);
            ProcessInfo processInfo = ProcessCache.CreateProcess(command);
            StepInfo modeStep = processInfo.CreateStep("请在60秒内发送数字选择游戏模式", CheckUCModeAsync);
            await processInfo.StartProcessing();
            gameMode = modeStep.AnswerForEnum<UCGameMode>();
            if (gameMode == UCGameMode.Customize)
            {

            }
            BaseGame baseGame = GameCahce.CreateGame(command,game);
            baseGame.StartProcessing();
        }

        private async Task<UCGameMode> CheckUCModeAsync(string value)
        {
            int modeId;
            if (int.TryParse(value, out modeId) == false)
            {
                throw new ProcessException("模式不在范围内");
            }
            if (Enum.IsDefined(typeof(UCGameMode), modeId) == false)
            {
                throw new ProcessException("模式不在范围内");
            }
            return await Task.FromResult((UCGameMode)modeId);
        }


    }
}
