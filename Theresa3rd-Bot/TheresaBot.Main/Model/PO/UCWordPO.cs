using SqlSugar;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("uc_word")]
    public record UCWordPO : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 10, ColumnDescription = "词条1")]
        public string Word1 { get; set; }

        [SugarColumn(IsNullable = false, Length = 10, ColumnDescription = "词条2")]
        public string Word2 { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "是否已经审核")]
        public bool IsAuthorized { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加人")]
        public long CreateMember { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime CreateDate { get; set; }
    }
}
