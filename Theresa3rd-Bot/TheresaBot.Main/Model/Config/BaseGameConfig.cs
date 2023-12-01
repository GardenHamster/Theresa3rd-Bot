using YamlDotNet.Serialization;

namespace TheresaBot.Main.Model.Config
{
    public abstract record BaseGameConfig : BaseConfig
    {
        [YamlMember(Order = -100)]
        public bool Enable { get; set; }
    }
}
