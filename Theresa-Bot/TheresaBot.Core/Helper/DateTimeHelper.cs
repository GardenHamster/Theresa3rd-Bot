using System.Globalization;

namespace TheresaBot.Core.Helper
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        private static readonly DateTimeFormatInfo SimpleDateTimeFormat = new DateTimeFormatInfo()
        {
            ShortDatePattern = "yyyy-MM-dd HH:mm:ss"
        };

        /// <summary>
        /// 获取今天开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDayStart()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day);
        }

        /// <summary>
        /// 获取今天结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDayEnd()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
        }

        /// <summary>
        /// 获取本周开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetWeekStart()
        {
            DateTime now = DateTime.Now;
            DateTime temp = new DateTime(now.Year, now.Month, now.Day);
            int count = now.DayOfWeek - DayOfWeek.Monday;
            if (count == -1) count = 6;
            return temp.AddDays(-count);
        }

        /// <summary>
        /// 获取本周结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetWeekEnd()
        {
            DateTime now = DateTime.Now;
            DateTime temp = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            int count = now.DayOfWeek - DayOfWeek.Sunday;
            if (count != 0) count = 7 - count;
            return temp.AddDays(count);
        }

        /// <summary>
        /// 获取本周开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMonthStart()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, now.Month, 1, 0, 0, 0);
        }

        /// <summary>
        /// 获取本周结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMonthEnd()
        {
            DateTime now = DateTime.Now;
            DateTime temp = new DateTime(now.Year, now.Month, 1, 23, 59, 59);
            return temp.AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 获取本年开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetYearStart()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, 1, 1, 0, 0, 0);
        }

        /// <summary>
        /// 获取本年结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetYearEnd()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, 12, 31, 23, 59, 59);
        }

        /// <summary>
        /// 获取昨天开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetYesterdayStart()
        {
            return GetDayStart().AddDays(-1);
        }

        /// <summary>
        /// 获取昨天结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetYesterdayEnd()
        {
            return GetDayEnd().AddDays(-1);
        }

        // <summary>
        /// 获取上周开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLastWeekStart()
        {
            return GetWeekStart().AddDays(-7);
        }

        /// <summary>
        /// 获取上周结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLastWeekEnd()
        {
            return GetWeekEnd().AddDays(-7);
        }

        // <summary>
        /// 获取上月开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLastMonthStart()
        {
            DateTime now = DateTime.Now;
            DateTime temp = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
            return temp.AddMonths(-1);
        }

        /// <summary>
        /// 获取上月结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLastMonthEnd()
        {
            DateTime now = DateTime.Now;
            DateTime temp = new DateTime(now.Year, now.Month, 1, 23, 59, 59);
            return temp.AddDays(-1);
        }


        /// <summary>
        /// 根据一个总秒速,返回时:分:秒格式字符串
        /// </summary>
        /// <param name="totalSecond"></param>
        /// <returns></returns>
        public static string GetTimeStrBySeconds(int totalSecond)
        {
            string hour = (totalSecond / 3600).ToString().PadLeft(2, '0');
            string minute = ((totalSecond % 3600) / 60).ToString().PadLeft(2, '0');
            string second = (totalSecond % 60).ToString().PadLeft(2, '0');
            return string.Format("{0}:{1}:{2}", hour, minute, second);
        }

        /// <summary>
        /// 获取两个日期的秒数差值
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <returns></returns>
        public static int SecondDiff(this DateTime timeEnd, DateTime timeStart)
        {
            TimeSpan timeSpanStart = new TimeSpan(timeStart.Ticks);
            TimeSpan timeSpanEnd = new TimeSpan(timeEnd.Ticks);
            TimeSpan timeDiff = timeSpanEnd.Subtract(timeSpanStart);
            return (int)timeDiff.TotalSeconds;
        }

        /// <summary>
        /// 获取两个日期的秒数差值
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpanDiff(DateTime timeStart, DateTime timeEnd)
        {
            TimeSpan timeSpanStart = new TimeSpan(timeStart.Ticks);
            TimeSpan timeSpanEnd = new TimeSpan(timeEnd.Ticks);
            return timeSpanEnd.Subtract(timeSpanStart);
        }

        /// <summary>
        /// 获取两个日期的天数差值
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <returns></returns>
        public static int GetDayDiff(DateTime timeStart, DateTime timeEnd)
        {
            TimeSpan timeSpanStart = new TimeSpan(timeStart.Ticks);
            TimeSpan timeSpanEnd = new TimeSpan(timeEnd.Ticks);
            TimeSpan timeDiff = timeSpanEnd.Subtract(timeSpanStart);
            return (int)timeDiff.TotalDays;
        }

        /// <summary>
        /// 获取当前时间n秒后的实例
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeAfterMinutes(int minutes)
        {
            return DateTime.Now.AddMinutes(minutes);
        }

        /// <summary>
        /// DateTime转换为Unix时间戳(秒)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToTimeStamp(this DateTime dateTime)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.ToUniversalTime() - startTime).TotalSeconds;
        }

        /// <summary>
        /// Unix时间戳(秒)转DateTime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long timeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(timeStamp).ToLocalTime();
            return dateTime;
        }

        /// <summary>
        /// 获取当天是本星期的第几天
        /// </summary>
        /// <returns></returns>
        public static int GetDayOfWeek()
        {
            DayOfWeek dayOfWeek = DateTime.Today.DayOfWeek;
            if (dayOfWeek == DayOfWeek.Monday) return 1;
            if (dayOfWeek == DayOfWeek.Tuesday) return 2;
            if (dayOfWeek == DayOfWeek.Wednesday) return 3;
            if (dayOfWeek == DayOfWeek.Thursday) return 4;
            if (dayOfWeek == DayOfWeek.Friday) return 5;
            if (dayOfWeek == DayOfWeek.Saturday) return 6;
            if (dayOfWeek == DayOfWeek.Sunday) return 7;
            return 0;
        }

        /// <summary>
        /// 将一个DateTime格式化为时间字符串
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSimpleString(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 将一个字符串转换为DateTime
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string str, string formatStr = "")
        {
            try
            {
                if (string.IsNullOrEmpty(formatStr))
                {
                    return Convert.ToDateTime(str, SimpleDateTimeFormat);
                }
                DateTimeFormatInfo formatInfo = new DateTimeFormatInfo()
                {
                    ShortDatePattern = formatStr
                };
                return Convert.ToDateTime(str, formatInfo);
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
