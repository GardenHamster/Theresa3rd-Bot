using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Core.Model.PO
{
    [SugarTable("pixiv_tag")]
    public record PixivTagPO : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "日文标签")]
        public string Tag { get; set; } = string.Empty;

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "简体中文")]
        public string Zh { get; set; } = string.Empty;

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "繁体中文")]
        public string ZhTw { get; set; } = string.Empty;

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "英文")]
        public string En { get; set; } = string.Empty;

        [SugarColumn(IsNullable = false, Length = 200, ColumnDescription = "韩文")]
        public string Ko { get; set; } = string.Empty;

        public PixivTagPO() { }

        public PixivTagPO(string tag)
        {
            this.Tag = tag;
        }

    }
}
