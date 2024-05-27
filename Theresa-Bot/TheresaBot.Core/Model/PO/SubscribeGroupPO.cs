using SqlSugar;

namespace TheresaBot.Core.Model.PO
{
    [SugarTable("subscribe_group")]
    public record SubscribeGroupPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "订阅ID")]
        public int SubscribeId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "群ID")]
        public long GroupId { get; set; }

    }
}
