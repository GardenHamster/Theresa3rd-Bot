namespace TheresaBot.Main.Model.Lolicon
{
    public class LoliconParamV2
    {
        public int r18 { get; set; }

        public bool excludeAI { get; set; }

        public int num { get; set; }

        public string[]? tag { get; set; }

        public string proxy { get; set; }

        public LoliconParamV2(int r18, bool excludeAI, int num, string proxy, string[]? tag)
        {
            this.r18 = r18;
            this.excludeAI = excludeAI;
            this.num = num;
            this.tag = tag;
            this.proxy = proxy;
        }

    }
}
