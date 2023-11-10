using System.Text.RegularExpressions;
using TheresaBot.Main.Helper;
using YamlDotNet.Serialization;

namespace TheresaBot.Main.Model.Config
{
    public record WelcomeConfig : BasePluginConfig
    {
        public string Template { get; set; }

        public List<WelcomeSpecial> Specials { get; set; } = new();

        public WelcomeSpecial GetSpecial(long groupId)
        {
            return Specials?.Where(m => m.ContainGroups.Contains(groupId)).FirstOrDefault();
        }

        public override WelcomeConfig FormatConfig()
        {
            if (Specials is null) Specials = new();
            foreach (var item in Specials) item?.FormatConfig();
            return this;
        }
    }

    public record WelcomeSpecial : BaseConfig
    {
        public List<long> Groups { get; set; } = new();

        public string Template { get; set; }

        [YamlIgnore]
        public List<long> ContainGroups => Groups?.ToSendableGroups() ?? new();

        public override BaseConfig FormatConfig()
        {
            if (Groups is null) Groups = new();
            return this;
        }
    }




}
