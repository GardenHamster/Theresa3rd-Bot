using SqlSugar;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("message_record")]
    public class MessageRecordPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "消息ID")]
        public long MsgId { get; set; }

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

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "包含的图片地址")]
        public string ImageUrls { get; set; }

        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "包含的PixivId")]
        public string PixivId { get; set; }
    }
}
