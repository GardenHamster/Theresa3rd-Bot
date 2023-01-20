namespace Theresa3rd_Bot.Model.Lolisuki
{
    public class LolisukiParam
    {
        public int r18 { get; set; }

        public int num { get; set; }

        public string proxy { get; set; }

        public string[] tag { get; set; }

        public string level { get; set; }

        public int full { get; set; }

        public int ai { get; set; }

        public LolisukiParam(int r18, int ai, int num, string proxy, string[] tag, string level, int full)
        {
            this.r18 = r18;
            this.ai = ai;
            this.num = num;
            this.proxy = proxy;
            this.tag = tag;
            this.level = level;
            this.full = full;
        }

    }
}
