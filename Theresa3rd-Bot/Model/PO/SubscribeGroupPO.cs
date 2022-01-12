using SqlSugar;

namespace Theresa3rd_Bot.Model.PO
{
    [SugarTable("subscribe_group")]
    public class SubscribeGroupPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "群ID")]
        public long GroupId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "订阅ID")]
        public int SubscribeId { get; set; }
        
    }
}
