using SqlSugar;

namespace TheresaBot.Core.Model.PO
{
    [SugarTable("pixiv_collection")]
    public record PixivCollectionPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "PixivId")]
        public int PixivId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "分类等级")]
        public int Level { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "作品标题")]
        public string Title { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "画师id")]
        public int UserId { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "画师名")]
        public string UserName { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "作品页数")]
        public int Pages { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "Thumb尺寸图片地址")]
        public string Thumb { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "Small尺寸图片地址")]
        public string Small { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "Regular尺寸图片地址")]
        public string Regular { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "Original尺寸图片地址")]
        public string Original { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "是否为动图")]
        public bool IsGif { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "是否为R18")]
        public bool IsR18 { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "是否为原创")]
        public bool IsOriginal { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "是否为AI")]
        public bool IsAI { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "作品长")]
        public int Width { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "作品高")]
        public int Height { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "本地文件夹路径")]
        public string LocalPath { get; set; }

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "OSS文件夹路径")]
        public string OSSPath { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "投稿日期")]
        public DateTime CreateDate { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime AddDate { get; set; }
    }
}
