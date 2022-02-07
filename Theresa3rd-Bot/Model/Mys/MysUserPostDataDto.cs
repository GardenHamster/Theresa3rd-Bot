using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Mys
{
    public class MysUserPostDataDto
    {
        public List<MysUserPostDto> list { get; set; }
    }

    public class MysUserPostDto
    {
        public MysUserExportPostDto post { get; set; }
    }

    public class MysUserExportPostDto
    {
        public string content { get; set; }
        public int created_at { get; set; }
        public List<string> images { get; set; }
        public string subject { get; set; }
        public string uid { get; set; }
        public string post_id { get; set; }
    }
}
