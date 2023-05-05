using SqlSugar;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("message_image")]
    public class MessageImagePO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "消息ID")]
        public int MsgId { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "图片Http地址")]
        public string HttpUrl { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送时间")]
        public DateTime CreateDate { get; set; }
    }
}
