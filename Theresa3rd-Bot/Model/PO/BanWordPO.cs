using SqlSugar;
using System;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.PO
{
    [SugarTable("ban_word")]
    public class BanWordPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "禁止类型")]
        public BanType BanType { get; set; }

        [SugarColumn(IsNullable = false, Length = 100, ColumnDescription = "关键词")]
        public string KeyWord { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "tinyint", ColumnDescription = "是否正则表达式")]
        public bool IsRegular { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime CreateDate { get; set; }
    }
}
