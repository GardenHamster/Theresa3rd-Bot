using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Util
{
    public static class RandomHelper
    {
        /// <summary>
        /// 获取一个范围在minValue与maxValue之间的随机数,包含maxValue
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int getRandomBetween(int minValue, int maxValue)
        {
            Random random = getRandom();
            return random.Next(minValue, maxValue + 1);
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <returns></returns>
        public static Random getRandom()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int seed = BitConverter.ToInt32(buffer, 0);
            return new Random(seed);
        }

        /// <summary>
        /// 获取随机内容
        /// </summary>
        /// <returns></returns>
        public static T getRandomItem<T>(List<T> list)
        {
            if (list is null || list.Count == 0) return default;
            int randomIndex = new Random().Next(0, list.Count);
            return list[randomIndex];
        }

    }
}
