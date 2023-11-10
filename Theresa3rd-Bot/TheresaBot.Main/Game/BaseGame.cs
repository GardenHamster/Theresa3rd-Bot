using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Game
{
    public abstract class BaseGame
    {
        public bool IsEnded { get; private set; }

        public abstract Task StartProcessing();

        public abstract bool HandleProcessing(GroupRelay relay);

    }
}
