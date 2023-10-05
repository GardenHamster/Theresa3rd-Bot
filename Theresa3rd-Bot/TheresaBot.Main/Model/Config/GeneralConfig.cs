using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Model.Config
{
    public record GeneralConfig : BaseConfig
    {
        public List<string> Prefixs { get; private set; }

        public string TempPath { get; private set; }

        public string FontPath { get; private set; }

        public List<long> ErrorGroups { get; private set; }

        public string ErrorMsg { get; private set; }

        public string ClearCron { get; private set; }

        public string ErrorImgPath { get; private set; }

        public string DisableMsg { get; private set; }

        public string NoPermissionsMsg { get; private set; }

        public string ManagersRequiredMsg { get; private set; }

        public string SetuCustomDisableMsg { get; private set; }

        public bool SendRelevantCommands { get; private set; }

        public string DefaultPrefix => Prefixs.FirstOrDefault() ?? string.Empty;

        public override GeneralConfig FormatConfig()
        {
            this.Prefixs = this.Prefixs?.Trim() ?? new();
            return this;
        }

    }
}
