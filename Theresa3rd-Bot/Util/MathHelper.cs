using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Util
{
    public static class MathHelper
    {
        /// <summary>
        /// 根据byte计算mb
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double getMbWithByte(double bytes)
        {
            return Math.Round(bytes / 1024 / 1024, 2);
        }

        /// <summary>
        /// 根据逗号拆分字符串,返回一个long集合
        /// </summary>
        /// <param name="idStrs"></param>
        /// <returns></returns>
        public static List<long> splitListFromStr(string idStrs)
        {
            List<long> idList = new List<long>();
            if (string.IsNullOrEmpty(idStrs)) return idList;
            string[] idArr = idStrs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (idArr.Length == 0) return idList;
            foreach (string item in idArr)
            {
                long id = 0;
                string idStr = item.Trim();
                if (string.IsNullOrEmpty(idStr)) continue;
                if (long.TryParse(idStr, out id) == false) continue;
                idList.Add(id);
            }
            return idList;
        }

        /// <summary>
        /// 计算比率,返回指定小数位
        /// </summary>
        /// <param name="num1">被除数</param>
        /// <param name="num2">除数</param>
        /// <param name="keep">保留多少位小数</param>
        /// <returns></returns>
        public static double getRate(int num1, int num2, int keep)
        {
            double rate = Convert.ToDouble(num1) / num2;
            return Math.Round(rate, keep);
        }

        /// <summary>
        /// 计算百分比,返回字符串
        /// </summary>
        /// <param name="num1">被除数</param>
        /// <param name="num2">除数</param>
        /// <param name="keep">保留多少位小数</param>
        /// <returns></returns>
        public static string getRateStr(int num1, int num2, int keep)
        {
            return getRate(num1, num2, keep) * 100 + "%";
        }

    }
}
