using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class BLBLHistoryDto
    {
        public int code { get; set; }
        public BLBLHistoryData data { get; set; }
    }

    public class BLBLHistoryData
    {
        public List<BLBLHistoryCard> cards { get; set; }
    }

    public class BLBLHistoryCard
    {
        public string card { get; set; }
        public BLBLCardDesc desc { get; set; }
        public BLBLHistoryExtra extra { get; set; }
    }

    public class BLBLCardDesc
    {
        /// <summary>
        /// 赞
        /// </summary>
        public int like { get; set; }
        /// <summary>
        /// 投稿id
        /// </summary>
        public string rid_str { get; set; }
        /// <summary>
        /// 投稿类型
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int uid { get; set; }
        /// <summary>
        /// 阅读数
        /// </summary>
        public int view { get; set; }
        /// <summary>
        /// 投稿视频编号
        /// </summary>
        public string bvid { get; set; }

        /// <summary>
        /// 动态id
        /// </summary>
        public string dynamic_id_str { get; set; }

        /// <summary>
        /// 转载原文信息
        /// </summary>
        public BLBLCardDesc origin { get; set; }
    }

    public class BLBLHistoryExtra
    {
        public string is_space_top { get; set; }
    }

}
