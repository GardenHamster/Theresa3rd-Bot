using TheresaBot.Main.Command;
using TheresaBot.Main.Relay;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Game
{
    public abstract class BaseGame
    {
        protected GroupCommand Command { get; set; }

        protected BaseSession Session { get; set; }

        protected BaseReporter Reporter { get; set; }
        /// <summary>
        /// 是否已经结束
        /// </summary>
        public bool IsStarted { get; protected set; }
        /// <summary>
        /// 是否已经结束
        /// </summary>
        public bool IsEnded { get; protected set; }
        /// <summary>
        /// 游戏所在群
        /// </summary>
        public long GroupId { get; set; }
        /// <summary>
        /// 游戏主线程
        /// </summary>
        public Task GameTask { get; set; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        public abstract string GameName { get; }
        /// <summary>
        /// 开始游戏线程
        /// </summary>
        /// <returns></returns>
        public abstract Task StartProcessing();
        /// <summary>
        /// 处理游戏消息
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public abstract Task<bool> HandleGameMessageAsync(GroupRelay relay);
        /// <summary>
        /// 处理游戏消息
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public abstract Task<bool> HandleGameMessageAsync(PrivateRelay relay);
        /// <summary>
        /// 判断某个成员是否已经加入游戏
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public abstract bool IsMemberJoined(long memberId);
        /// <summary>
        /// 强制开始游戏
        /// </summary>
        public abstract Task ForceStart(GroupCommand command);
        /// <summary>
        /// 强制终止游戏
        /// </summary>
        public abstract Task ForceStop(GroupCommand command);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="session"></param>
        /// <param name="reporter"></param>
        public BaseGame(GroupCommand command, BaseSession session, BaseReporter reporter)
        {
            Command = command;
            Session = session;
            Reporter = reporter;
            GroupId = command.GroupId;
        }

    }
}
