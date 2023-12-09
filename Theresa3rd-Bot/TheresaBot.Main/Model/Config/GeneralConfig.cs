using TheresaBot.Main.Helper;
using YamlDotNet.Serialization;

namespace TheresaBot.Main.Model.Config
{
    public record GeneralConfig : BaseConfig
    {
        public List<string> Prefixs { get; set; } = new();

        public string TempPath { get; set; }

        public string FontPath { get; set; }

        public List<long> ErrorGroups { get; set; } = new();

        public string ErrorMsg { get; set; }

        public string ClearCron { get; set; }

        public string ErrorImgPath { get; set; }

        public string DisableMsg { get; set; }

        public string NoPermissionsMsg { get; set; }

        public string ManagersRequiredMsg { get; set; }

        public string SetuCustomDisableMsg { get; set; }

        public bool SendRelevantCommands { get; set; }

        public bool AcceptFriendRequest { get; set; }

        public override GeneralConfig FormatConfig()
        {
            if (Prefixs is null) Prefixs = new();
            if (ErrorGroups is null) ErrorGroups = new();
            if (string.IsNullOrWhiteSpace(ClearCron)) ClearCron = "0 0 4 * * ?";
            Prefixs = Prefixs.Trim();
            return this;
        }

    }
}
