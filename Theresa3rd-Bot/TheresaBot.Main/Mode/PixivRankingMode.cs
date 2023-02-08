using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Model.Type;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Mode
{
    public class PixivRankingMode: BaseModel<PixivRankingType>
    {
        public PixivRankingMode(PixivRankingType type, string code, string name) : base(type, code, name) { }

        public static readonly PixivRankingMode Daily = new(PixivRankingType.Daily, "daily", "日榜");

    }
}
