using SqlSugar;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.PO
{
    [SugarTable("subscribe_record")]
    public record SubscribeRecordPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "订阅ID")]
        public int SubscribeId { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "标题")]
        public string Title { get; set; }

        [SugarColumn(IsNullable = false, Length = 500, ColumnDescription = "内容")]
        public string Content { get; set; }

        [SugarColumn(IsNullable = false, Length = 500, ColumnDescription = "描述")]
        public string Describe { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "封面路径")]
        public string CoverUrl { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "连接路径")]
        public string LinkUrl { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "文章类型")]
        public SubscribeDynamicType DynamicType { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, IndexGroupNameList = new string[] { "index_dynamicCode" }, ColumnDescription = "文章编码,唯一标识")]
        public string DynamicCode { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime CreateDate { get; set; }

        public SubscribeRecordPO() { }

        public SubscribeRecordPO(int subscribeId)
        {
            this.Describe = "";
            this.SubscribeId = subscribeId;
            this.CreateDate = DateTime.Now;
        }


    }
}
