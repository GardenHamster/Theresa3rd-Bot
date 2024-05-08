using SqlSugar;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("dictionary")]
    public record DictionaryPO : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "关键词")]
        public string Words { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "词类型")]
        public DictionaryType WordType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "子类型")]
        public int SubType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime CreateDate { get; set; }

    }
}
