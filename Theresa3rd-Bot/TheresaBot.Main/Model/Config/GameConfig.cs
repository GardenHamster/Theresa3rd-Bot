using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Config
{
    public record GameConfig : BasePluginConfig
    {
        public List<string> JoinCommands { get; set; } = new();

        public List<string> EndCommands { get; set; } = new();

        public UndercoverConfig Undercover { get; set; }

        public override BaseConfig FormatConfig()
        {
            if (JoinCommands is null) JoinCommands = new();
            if (EndCommands is null) EndCommands = new();
            Undercover?.FormatConfig();
            return this;
        }
    }

    public record UndercoverConfig : BaseGameConfig
    {
        public List<string> CreateCommands { get; set; } = new();

        public override BaseConfig FormatConfig()
        {
            if (CreateCommands is null) CreateCommands = new();
            return this;
        }
    }


}
