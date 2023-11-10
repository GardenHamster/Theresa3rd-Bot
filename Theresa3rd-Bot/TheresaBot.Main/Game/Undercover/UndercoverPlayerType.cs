using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Game.Undercover
{
    public class UndercoverPlayerType
    {
        public string TypeName { get; set; }

        public UndercoverPlayerType(string typeName)
        {
            TypeName = typeName;
        }

        public static readonly UndercoverPlayerType Civilian = new("平民");

        public static readonly UndercoverPlayerType Undercover = new("卧底");

        public static readonly UndercoverPlayerType Whiteboard = new("白板");

    }
}
