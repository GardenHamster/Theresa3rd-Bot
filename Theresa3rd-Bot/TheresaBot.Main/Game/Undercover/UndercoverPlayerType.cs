namespace TheresaBot.Main.Game.Undercover
{
    public class UndercoverPlayerType
    {
        public string TypeName { get; set; }

        public UndercoverPlayerType(string typeName)
        {
            TypeName = typeName;
        }

        public static readonly UndercoverPlayerType None = new("未分配");

        public static readonly UndercoverPlayerType Civilian = new("平民");

        public static readonly UndercoverPlayerType Undercover = new("卧底");

        public static readonly UndercoverPlayerType Whiteboard = new("白板");

    }
}
