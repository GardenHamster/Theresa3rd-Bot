using SqlSugar;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("sugar_tag")]
    internal record SugarTag : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 100, ColumnDescription = "关键词")]
        public string KeyWords { get; set; }

        [SugarColumn(IsNullable = false, Length = 300, ColumnDescription = "对应的标签")]
        public string SearchTags { get; set; }
    }
}
