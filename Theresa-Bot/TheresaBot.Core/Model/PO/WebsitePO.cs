using SqlSugar;

namespace TheresaBot.Core.Model.PO
{
    [SugarTable("website")]
    public record WebsitePO : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "编码")]
        public string Code { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "用户Id")]
        public long UserId { get; set; }

        [SugarColumn(IsNullable = false, Length = 5000, ColumnDescription = "cookie")]
        public string Cookie { get; set; }

        [SugarColumn(IsNullable = false, Length = 500, ColumnDescription = "Csrf-Token")]
        public string CsrfToken { get; set; }
        
        [SugarColumn(IsNullable = false, ColumnDescription = "cookie过期时间")]
        public DateTime CookieExpireDate { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "最后更新时间")]
        public DateTime UpdateDate { get; set; }

    }
}
