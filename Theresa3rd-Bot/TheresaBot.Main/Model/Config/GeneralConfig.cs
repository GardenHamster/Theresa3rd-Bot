namespace TheresaBot.Main.Model.Config
{
    public class GeneralConfig
    {
        public string Prefix { get; set; }

        public string DownloadPath { get; set; }

        public List<long> ErrorGroups { get; set; }

        public string ErrorMsg { get; set; }

        public string DownErrorImgPath { get; set; }

        public string DisableMsg { get; set; }

        public string NoPermissionsMsg { get; set; }

        public string ManagersRequiredMsg { get; set; }

        public string SetuCustomDisableMsg { get; set; }

        public bool SendRelevantCommands { get; set; }
    }
}
