namespace TheresaBot.Core.Game.Undercover
{
    /// <summary>
    /// 玩家阵营
    /// </summary>
    public class UCCamp
    {
        /// <summary>
        /// 阵营名称
        /// </summary>
        public string CampName { get; set; }

        public static readonly UCCamp None = new("未分配");

        public static readonly UCCamp Civilian = new("平民");

        public static readonly UCCamp Undercover = new("卧底");

        public static readonly UCCamp Whiteboard = new("白板");

        public UCCamp(string campName)
        {
            CampName = campName;
        }

    }
}
