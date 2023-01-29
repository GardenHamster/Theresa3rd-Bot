namespace TheresaBot.Main.Model.Saucenao
{
    public class SaucenaoMessage
    {
        public decimal Similar { get; set; }

        public List<IChatMessage> GroupMsgs { get; set; }

        public List<IChatMessage> TempMsgs { get; set; }

        public SaucenaoMessage(SaucenaoItem saucenaoItem, List<IChatMessage> groupMsgs, List<IChatMessage> tempMsgs)
        {
            this.GroupMsgs = groupMsgs;
            this.TempMsgs = tempMsgs;
            this.Similar = saucenaoItem.Similarity;
        }


    }
}
