namespace TheresaBot.Main.Helper
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
        /// 通过总数和每页数量计算总页数
        /// </summary>
        /// <param name="total"></param>
        /// <param name="eachPage"></param>
        /// <returns></returns>
        public static int getMaxPage(int total, int eachPage)
        {
            return (int)Math.Ceiling(Convert.ToDecimal(total) / eachPage);
        }

        /// <summary>
        /// 将double转换为百分比字符串
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string toPercent(this double number)
        {
            number = Convert.ToDouble((int)(number * 100)) / 100;
            return number.ToString() + "%";
        }

    }
}
