using SqlSugar;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("word_cloud")]
    public class WordCloudPO : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 20, ColumnDescription = "关键词")]
        public string Word { get; set; }

        [SugarColumn(IsNullable = false, Length = 20, ColumnDescription = "词类型")]
        public WordType WordType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加日期")]
        public DateTime CreateDate { get; set; }

    }
}
