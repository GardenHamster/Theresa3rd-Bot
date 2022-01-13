using SqlSugar;
using System;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.PO
{
    [SugarTable("subscribe")]
    public class SubscribePO : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "订阅编号")]
        public string SubscribeCode { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "订阅名称")]
        public string SubscribeName { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "订阅类型")]
        public SubscribeType SubscribeType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "订阅子类型")]
        public SubscribeSubType SubscribeSubType { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "订阅描述")]
        public string SubscribeDescription { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "tinyint", ColumnDescription = "是否直播中")]
        public bool Isliving { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime CreateDate { get; set; }

    }
}
