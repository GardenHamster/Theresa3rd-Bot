using YamlDotNet.Serialization;

namespace TheresaBot.Core.Model.Config
{
    public abstract record BaseGameConfig : BaseConfig
    {
        [YamlMember(Order = -100)]
        public bool Enable { get; set; }
    }
}
