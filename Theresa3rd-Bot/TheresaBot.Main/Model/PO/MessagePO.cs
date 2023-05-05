using SqlSugar;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("message")]
    public class MessagePO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "消息编号")]
        public long MsgCode { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送群ID")]
        public long GroupId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送人ID")]
        public long MemberId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "bot发出的消息")]
        public bool SendOut { get; set; }

        [SugarColumn(IsNullable = false, Length = 1000, ColumnDescription = "发送内容")]
        public string Message { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送时间")]
        public DateTime CreateDate { get; set; }
    }
}
