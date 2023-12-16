using SqlSugar;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("ban_pixiver")]
    public record BanPixiverPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "PIxiv用户ID")]
        public long PixiverId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime CreateDate { get; set; }

    }
}
