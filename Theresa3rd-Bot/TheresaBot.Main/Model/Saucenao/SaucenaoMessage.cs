using TheresaBot.Main.Model.Content;

namespace TheresaBot.Main.Model.Saucenao
{
    public class SaucenaoMessage
    {
        public decimal Similar { get; set; }

        public List<BaseContent> GroupMsgs { get; set; }

        public List<BaseContent> TempMsgs { get; set; }

        public SaucenaoMessage(SaucenaoItem saucenaoItem, List<BaseContent> groupMsgs, List<BaseContent> tempMsgs)
        {
            this.GroupMsgs = groupMsgs;
            this.TempMsgs = tempMsgs;
            this.Similar = saucenaoItem.Similarity;
        }


    }
}
