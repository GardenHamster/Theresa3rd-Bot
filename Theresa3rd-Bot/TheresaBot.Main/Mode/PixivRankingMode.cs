using TheresaBot.Main.Model.Type;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Mode
{
    public class PixivRankingMode
    {
        public PixivRankingType Type { get; set; }
        public string Code { get; init; }
        public string Name { get; init; }
        public string R18Code { get; init; }
        public string R18Name { get; init; }

        public PixivRankingMode(PixivRankingType type, string code, string name, string r18_code = "", string r18_name = "")
        {
            this.Type = type;
            this.Code = code;
            this.Name = name;
            this.R18Code = r18_code;
            this.R18Name = r18_name;
        }

        public static readonly PixivRankingMode Daily = new(PixivRankingType.Daily, "daily", "日榜", "daily_r18", "R18日榜");
        public static readonly PixivRankingMode DailyAI = new(PixivRankingType.DailyAI, "daily_ai", "AI日榜", "daily_r18_ai", "R18AI日榜");
        public static readonly PixivRankingMode Male = new(PixivRankingType.Male, "male", "LSP日榜", "male_r18", "R18LSP日榜");
        public static readonly PixivRankingMode Weekly = new(PixivRankingType.Weekly, "weekly", "周榜", "weekly_r18", "R18周榜");
        public static readonly PixivRankingMode Monthly = new(PixivRankingType.Monthly, "monthly", "月榜");

    }
}
