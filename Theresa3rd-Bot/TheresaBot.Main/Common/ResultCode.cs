using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Common
{
    public static class ResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static readonly int Success = 0;

        /// <summary>
        /// 错误
        /// </summary>
        public static readonly int Error = -1;

        /// <summary>
        /// 未登录
        /// </summary>
        public static readonly int NoLogin = -100;

        /// <summary>
        /// 参数错误
        /// </summary>
        public static readonly int ParamError = -101;

    }
}
