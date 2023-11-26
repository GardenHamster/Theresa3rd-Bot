using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Game.Undercover;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    internal class GameHandler : BaseHandler
    {
        public GameHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
        }

        public async Task JoinGame(GroupCommand command)
        {
            try
            {
                var game = GameCahce.GetGameByGroup(command.GroupId);
                if (game is null || game.IsEnded)
                {
                    await command.ReplyGroupMessageWithQuoteAsync("加入失败，目前没有可以加入的游戏");
                    return;
                }
                if (game is UCGame ucGame)
                {
                    UCPlayer player = new UCPlayer(command.MemberId, command.MemberNick);
                    await ucGame.PlayerJoinAsync(player);
                    return;
                }
                throw new Exception("加入失败，未定义游戏加入方法");
            }
            catch (GameException ex)
            {
                await command.ReplyGroupMessageWithQuoteAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "加入Undercover游戏异常");
            }
        }

        public async Task StopGame(GroupCommand command)
        {
            try
            {
                var game = GameCahce.GetGameByGroup(command.GroupId);
                if (game is null || game.IsEnded)
                {
                    await command.ReplyGroupMessageWithQuoteAsync("当前群目前没有可以正在进行中的游戏");
                }
                else
                {
                    game.Stop();
                    await command.ReplyGroupMessageWithQuoteAsync("游戏已停止~");
                }
            }
            catch (GameException ex)
            {
                await command.ReplyGroupMessageWithQuoteAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "加入Undercover游戏异常");
            }
        }

    }
}
