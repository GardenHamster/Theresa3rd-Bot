using SqlSugar;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("ban_tag")]
    public record BanTagPO : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 100, ColumnDescription = "关键词")]
        public string Keyword { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "tinyint", ColumnDescription = "是否全词匹配")]
        public bool FullMatch { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "tinyint", ColumnDescription = "是否正则表达式")]
        public bool IsRegular { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime CreateDate { get; set; }

    }
}
