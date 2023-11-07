using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.VO
{
    public record SubscribeDataVo
    {
        public int PixivUserSubs { get; set; }

        public int PixivTagSubs { get; set; }

        public int MysUserSubs { get; set; }

    }
}
