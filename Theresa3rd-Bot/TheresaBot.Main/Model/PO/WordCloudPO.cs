using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.PO
{
    [SugarTable("word_cloud")]
    public class WordCloudPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "群ID")]
        public long GroupId { get; set; }

        [SugarColumn(IsNullable = false, Length = 20, ColumnDescription = "关键词")]
        public string Word { get; set; }

        [SugarColumn(IsNullable = false, Length = 20, ColumnDescription = "权重")]
        public double Weight { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加日期")]
        public DateTime CreateDate { get; set; }

    }
}
