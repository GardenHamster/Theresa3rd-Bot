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
using TheresaBot.Main.Helper;
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
            try
            {
                UndercoverGame ucGame = new UndercoverGame(command, Session, Reporter);
                UCGameMode gameMode = await GetGameModeAsync(command);
                if (gameMode == UCGameMode.Customize)
                {
                    int[] nums = await GetCharacterNums(command);
                    ucGame = new UndercoverGame(command, Session, Reporter, nums[0], nums[1], nums[2]);
                }
                GameCahce.CreateGame(command, ucGame);
                Task gameTask = ucGame.StartProcessing();
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (GameException ex)
            {
                await command.ReplyGroupMessageWithQuoteAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "Undercover游戏异常");
            }
        }

        private async Task<UCGameMode> GetGameModeAsync(GroupCommand command)
        {
            ProcessInfo processInfo = ProcessCache.CreateProcess(command);
            StepInfo modeStep = processInfo.CreateStep("请在60秒内发送数字选择游戏模式", CheckUCModeAsync);
            await processInfo.StartProcessing();
            return modeStep.AnswerForEnum<UCGameMode>();
        }

        private async Task<int[]> GetCharacterNums(GroupCommand command)
        {
            ProcessInfo processInfo = ProcessCache.CreateProcess(command);
            StepInfo modeStep = processInfo.CreateStep("请在60秒内发送 平民 卧底 白板 的数量，每个数字之间用空格隔开", CheckCharacterNumsAsync);
            await processInfo.StartProcessing();
            return modeStep.AnswerForEnum<UCGameMode>();
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

        private async Task<int[]> CheckCharacterNumsAsync(string value)
        {
            int civNum = 0, ucNum = 0, wbNum = 0;
            var splitArr = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (splitArr.Length < 1)
            {
                throw new ProcessException("至少指定平民的数量");
            }
            if (splitArr.Length > 0 && !int.TryParse(splitArr[0], out civNum))
            {
                throw new ProcessException("平民数量必须为数字");
            }
            if (splitArr.Length > 1 && !int.TryParse(splitArr[1], out ucNum))
            {
                throw new ProcessException("卧底数量必须为数字");
            }
            if (splitArr.Length > 2 && !int.TryParse(splitArr[2], out wbNum))
            {
                throw new ProcessException("白板数量必须为数字");
            }
            if (civNum + ucNum + wbNum < 3)
            {
                throw new ProcessException("平民+卧底+白板数量必须在3人及以上");
            }
            return await Task.FromResult(new int[] { civNum, ucNum, wbNum });
        }


    }
}
