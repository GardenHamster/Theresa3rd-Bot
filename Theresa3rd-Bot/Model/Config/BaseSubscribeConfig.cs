namespace Theresa3rd_Bot.Model.Config
{
    public class BaseSubscribeConfig
    {
        public bool Enable { get; set; }

        public string AddCommand { get; set; }

        public string RmCommand { get; set; }

        public string Template { get; set; }

        public int ScanInterval { get; set; }
    }
}
