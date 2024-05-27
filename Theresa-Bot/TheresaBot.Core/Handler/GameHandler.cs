using TheresaBot.Core.Cache;
using TheresaBot.Core.Command;
using TheresaBot.Core.Exceptions;
using TheresaBot.Core.Game.Undercover;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;

namespace TheresaBot.Core.Handler
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
                    await ucGame.PlayerJoinAsync(command, player);
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

        public async Task ForceStop(GroupCommand command)
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
                    await game.ForceStop(command);
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

        public async Task ForceStart(GroupCommand command)
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
                    await game.ForceStart(command);
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
