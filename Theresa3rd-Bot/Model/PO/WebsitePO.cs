using SqlSugar;
using System;

namespace Theresa3rd_Bot.Model.PO
{
    [SugarTable("website")]
    public class WebsitePO : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "编码")]
        public string Code { get; set; }

        [SugarColumn(IsNullable = false, Length = 2000, ColumnDescription = "cookie")]
        public string Cookie { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "cookie过期时间")]
        public DateTime CookieExpireDate { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "最后更新时间")]
        public DateTime UpdateDate { get; set; }

    }
}
