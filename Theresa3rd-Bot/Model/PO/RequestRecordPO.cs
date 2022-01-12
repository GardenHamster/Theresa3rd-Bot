using SqlSugar;
using System;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.PO
{
    [SugarTable("request_record")]
    public class RequestRecordPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "群ID")]
        public long GroupId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "群员ID")]
        public long MemberId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "指令类型")]
        public CommandType CommandType { get; set; }

        [SugarColumn(IsNullable = false, Length = 100, ColumnDescription = "消息记录")]
        public string SendWord { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime CreateDate { get; set; }

    }
}
