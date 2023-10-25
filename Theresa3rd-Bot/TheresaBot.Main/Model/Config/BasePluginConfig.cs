using YamlDotNet.Serialization;

namespace TheresaBot.Main.Model.Config
{
    public abstract record BasePluginConfig : BaseConfig
    {
        [YamlMember(Order = -100)]
        public bool Enable { get; set; }
    }
}
