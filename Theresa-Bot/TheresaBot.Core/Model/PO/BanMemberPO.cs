using SqlSugar;

namespace TheresaBot.Core.Model.PO
{
    [SugarTable("ban_member")]
    public record BanMemberPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "QQ号")]
        public long MemberId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime CreateDate { get; set; }

    }
}
