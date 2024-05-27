using SqlSugar;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.PO
{
    [SugarTable(tableName: "subscribe")]
    public record SubscribePO : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 100, ColumnDescription = "订阅编号")]
        public string SubscribeCode { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "订阅名称")]
        public string SubscribeName { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "订阅类型")]
        public SubscribeType SubscribeType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "订阅子类型")]
        public int SubscribeSubType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime CreateDate { get; set; }

        public SubscribePO()
        {
            this.SubscribeName = string.Empty;
            this.CreateDate = DateTime.Now;
        }

    }
}
