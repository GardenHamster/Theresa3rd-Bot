using SqlSugar;

namespace TheresaBot.Core.Model.PO
{
    [SugarTable("pixiv_collection_tag")]
    public record PixivCollectionTagPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "收藏ID")]
        public int CollectionId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "PixivTagId")]
        public int TagId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "是否为用户额外添加的标签")]
        public bool IsExtra { get; set; } = false;
    }
}
