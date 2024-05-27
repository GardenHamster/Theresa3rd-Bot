using SqlSugar;

namespace TheresaBot.Core.Model.PO
{
    [SugarTable("sugar_tag")]
    public record SugarTagPO : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 100, ColumnDescription = "关键词")]
        public string KeyWord { get; set; }

        [SugarColumn(IsNullable = false, Length = 300, ColumnDescription = "绑定的标签")]
        public string BindTags { get; set; }
    }
}
