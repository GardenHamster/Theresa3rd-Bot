namespace Theresa3rd_Bot.Model.Config
{
    public class SaucenaoConfig
    {
        public bool Enable { get; set; }

        public string Command { get; set; }

        public string DisableMsg { get; set; }

        public string NotFoundMsg { get; set; }

        public string ErrorMsg { get; set; }

        public string ProcessingMsg { get; set; }

        public int MemberCD { get; set; }

        public int MaxDaily { get; set; }

        public int MaxReceive { get; set; }

        public decimal MinSimilarity { get; set; }

        public bool PullOrigin { get; set; }

        public decimal WithdrawOver { get; set; }

        public int RevokeInterval { get; set; }

    }
}
