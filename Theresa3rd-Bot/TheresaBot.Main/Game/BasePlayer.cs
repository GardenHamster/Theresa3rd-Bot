using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Game
{
    public abstract class BasePlayer
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public long MemberId { get; private set; }
    }
}
