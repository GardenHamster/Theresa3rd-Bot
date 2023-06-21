using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Model.Config
{
    public class GeneralConfig
    {
        public List<string> Prefixs { get; private set; }

        public string DownloadPath { get; private set; }

        public List<long> ErrorGroups { get; private set; }

        public string ErrorMsg { get; private set; }

        public string DownPathCleanCron { get; set; }

        public string DownErrorImgPath { get; private set; }

        public string DisableMsg { get; private set; }

        public string NoPermissionsMsg { get; private set; }

        public string ManagersRequiredMsg { get; private set; }

        public string SetuCustomDisableMsg { get; private set; }

        public bool SendRelevantCommands { get; private set; }

        public GeneralConfig FormatConfig()
        {
            this.Prefixs = this.Prefixs?.Trim() ?? new();
            return this;
        }

    }
}
