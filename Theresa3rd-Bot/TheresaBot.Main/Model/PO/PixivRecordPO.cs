using SqlSugar;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("pixiv_record")]
    public class PixivRecordPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "消息ID")]
        public long MessageId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送群ID")]
        public long GroupId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "PixivId")]
        public int PixivId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "画师Id")]
        public int UserId { get; set; }

        [SugarColumn(IsNullable = false, Length = 100, ColumnDescription = "作品标题")]
        public string Title { get; set; }

        [SugarColumn(IsNullable = false, Length = 100, ColumnDescription = "画师名称")]
        public string UserName { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送时间")]
        public DateTime CreateDate { get; set; }
    }
}
