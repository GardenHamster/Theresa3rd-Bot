using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Game.Undercover;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type.GameOptions;

namespace TheresaBot.Main.Handler
{
    internal class UndercoverHandler : GameHandler
    {
        public UndercoverHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
        }

        public async Task SendPrivateWords(GroupCommand command)
        {
            var game = GameCahce.GetGameByGroup(command.GroupId);
            if (game is null || game.IsEnded || game is not UndercoverGame ucGame)
            {
                await command.ReplyGroupMessageWithQuoteAsync("游戏未开始，无法获取词条");
                return;
            }
            var player = ucGame.Players?.FirstOrDefault(o => o.MemberId == command.MemberId);
            if (player is null)
            {
                await command.ReplyGroupMessageWithQuoteAsync("你还未加入游戏，无法获取词条");
                return;
            }
            if (player.PlayerType == UndercoverPlayerType.None)
            {
                await command.ReplyGroupMessageWithQuoteAsync("词条还未派发，请耐心等待...");
                return;
            }
            var contents = new List<BaseContent> {
                new PlainContent(player.GetWordMessage())
            };
            await command.SendTempMessageAsync(contents);
            await Task.Delay(1000);
            await command.ReplyGroupMessageWithQuoteAsync("词条已私发，请查看私聊消息");
        }

        public async Task CreateUndercover(GroupCommand command)
        {
            try
            {
                UndercoverGame ucGame = new UndercoverGame(command, Session, Reporter);
                UCGameMode gameMode = await AskGameModeAsync(command);
                if (gameMode == UCGameMode.Customize)
                {
                    int[] nums = await AskCharacterNums(command);
                    ucGame = new UndercoverGame(command, Session, Reporter, nums[0], nums[1], nums[2]);
                }
                GameCahce.CreateGame(command, ucGame);
                await ucGame.StartProcessing();
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

        private async Task<UCGameMode> AskGameModeAsync(GroupCommand command)
        {
            var processInfo = ProcessCache.CreateProcess(command);
            var modeStep = processInfo.CreateStep("请在60秒内发送数字选择游戏模式", CheckUCModeAsync);
            await processInfo.StartProcessing();
            return modeStep.Answer;
        }

        private async Task<int[]> AskCharacterNums(GroupCommand command)
        {
            var processInfo = ProcessCache.CreateProcess(command);
            var modeStep = processInfo.CreateStep("请在60秒内发送 平民 卧底 白板 的数量，每个数字之间用空格隔开", CheckCharacterNumsAsync);
            await processInfo.StartProcessing();
            return modeStep.Answer;
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
