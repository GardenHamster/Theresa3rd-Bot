namespace Theresa3rd_Bot.Model.Config
{
    public class SaucenaoConfig
    {
        public bool Enable { get; set; }

        public string Command { get; set; }

        public string DisableMsg { get; set; }

        public string NotFoundMsg { get; set; }

        public string ErrorMsg { get; set; }

        public string ProcessingMag { get; set; }

        public string Template { get; set; }

        public int GroupCD { get; set; }

        public int MemberCD { get; set; }

        public int MaxDaily { get; set; }

        public int WaitSeconds { get; set; }

        public int MaxReceive { get; set; }

    }
}
