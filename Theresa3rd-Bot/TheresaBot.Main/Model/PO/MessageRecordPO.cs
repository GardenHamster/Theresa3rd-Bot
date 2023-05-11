using SqlSugar;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("message_record")]
    public class MessageRecordPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "消息编号")]
        public long MessageId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送群ID")]
        public long GroupId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送人ID")]
        public long MemberId { get; set; }

        [SugarColumn(IsNullable = false, Length = 1000, ColumnDescription = "发送文本")]
        public string MessageText { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送时间")]
        public DateTime CreateDate { get; set; }

    }
}
