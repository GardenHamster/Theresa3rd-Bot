using SqlSugar;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("message_pixiv")]
    public class MessagePixivPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "消息ID")]
        public int MsgId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "PixivId")]
        public int PixivId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "画师")]
        public int UserId { get; set; }

        [SugarColumn(IsNullable = false, Length = 100, ColumnDescription = "作品标题")]
        public string Title { get; set; }

        [SugarColumn(IsNullable = false, Length = 100, ColumnDescription = "画师名称")]
        public string UserName { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送时间")]
        public DateTime CreateDate { get; set; }
    }
}
