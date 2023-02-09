using TheresaBot.Main.Model.Type;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Mode
{
    public class PixivRankingMode: BaseModel<PixivRankingType>
    {
        public PixivRankingMode(PixivRankingType type, string code, string name) : base(type, code, name) { }

        public static readonly PixivRankingMode Daily = new(PixivRankingType.Daily, "daily", "日榜");
        public static readonly PixivRankingMode DailyR18 = new(PixivRankingType.Daily, "daily_r18", "R18日榜");
        public static readonly PixivRankingMode DailyAI = new(PixivRankingType.DailyAI, "daily_ai", "AI日榜");
        public static readonly PixivRankingMode DailyAIR18 = new(PixivRankingType.DailyAI, "daily_r18_ai", "R18AI日榜");
        public static readonly PixivRankingMode Male = new(PixivRankingType.Male, "male", "LSP日榜");
        public static readonly PixivRankingMode MaleR18 = new(PixivRankingType.Male, "male_r18", "R18LSP日榜");
        public static readonly PixivRankingMode Weekly = new(PixivRankingType.Weekly, "weekly", "周榜");
        public static readonly PixivRankingMode WeeklyR18 = new(PixivRankingType.Weekly, "weekly_r18", "R18周榜");
        public static readonly PixivRankingMode Monthly = new(PixivRankingType.Monthly, "monthly", "月榜");
    }
}
