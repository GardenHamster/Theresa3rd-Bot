namespace TheresaBot.Main.Game.Undercover
{
    public class UCPlayerCamp
    {
        public string TypeName { get; set; }

        public UCPlayerCamp(string typeName)
        {
            TypeName = typeName;
        }

        public static readonly UCPlayerCamp None = new("未分配");

        public static readonly UCPlayerCamp Civilian = new("平民");

        public static readonly UCPlayerCamp Undercover = new("卧底");

        public static readonly UCPlayerCamp Whiteboard = new("白板");

    }
}
