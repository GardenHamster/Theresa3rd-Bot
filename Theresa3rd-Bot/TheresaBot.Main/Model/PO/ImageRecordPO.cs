using SqlSugar;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("image_record")]
    public class ImageRecordPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "消息ID")]
        public long MessageId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "Bot平台")]
        public PlatformType PlatformType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送群ID")]
        public long GroupId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送人ID")]
        public long MemberId { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "图片Http地址")]
        public string HttpUrl { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "发送时间")]
        public DateTime CreateDate { get; set; }
    }
}
