using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Theresa3rd_Bot.Util
{
    public static class BusinessHelper
    {
        public static bool isMemberCooling(CQGroupMessageEventArgs e, int cdSecond)
        {
            if (cdSecond <= 0) return false;
            e.SendMessageWithAt(string.Format(" 功能冷却中，{0}秒后再来哦~", cdSecond));
            //Thread.Sleep(5 * 1000);
            //e.Message.RemoveMessage();
            return true;
        }

        public static bool isGroupCooling(CQGroupMessageEventArgs e, int cdSecond)
        {
            if (cdSecond <= 0) return false;
            e.SendMessageWithAt(string.Format(" 群功能冷却中，{0}秒后再来哦~", cdSecond));
            //Thread.Sleep(5 * 1000);
            //e.Message.RemoveMessage();
            return true;
        }

        public static bool isHanding(CQGroupMessageEventArgs e)
        {
            if (CoolingCache.isHanding(e.FromGroup.Id, e.FromQQ.Id) == false) return false;
            e.SendMessageWithAt(" 你的一个请求正在处理中，稍后再来吧");
            //Thread.Sleep(5 * 1000);
            //e.Message.RemoveMessage();
            return true;
        }

        public static bool isSTUseUp(CQGroupMessageEventArgs e)
        {
            if (Setting.Permissions.LimitlessGroups.Contains(e.FromGroup.Id)) return false;
            if (Setting.Member.LimitlessMemberIds.Contains(e.FromQQ.Id)) return false;
            int[] functionTypeArr = new int[] { FunctionType.PixivicST.TypeId, FunctionType.PixivGeneralST.TypeId, FunctionType.PixivR18ST.TypeId };
            int useCount = new FunctionRecordBusiness().getUsedCountToday(e.FromGroup.Id, e.FromQQ.Id, functionTypeArr);
            if (useCount < Setting.Robot.MaxSTUseOneDay) return false;
            e.SendMessageWithAt(" 你今天的最大使用次数已经达到上限了，明天再来吧");
            return true;
        }

        public static bool isCrystalEnough(CQGroupMessageEventArgs e,int crystalAmount, int crystalUseUp)
        {
            if (crystalAmount >= crystalUseUp) return true;
            e.SendMessageWithAt(" 水晶不够？快通过#签到，#洗甲板，#种包菜，#收包菜来获取水晶吧~");
            return false;
        }

        /// <summary>
        /// 从随机涩图文件夹中获取指定数量的涩图
        /// </summary>
        /// <param name="CQApi"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<FileInfo> getRandomSTList(CQApi CQApi,int count)
        {
            return getRandomFileInList(FilePath.getRandomImgSavePath(), count);
        }


        /// <summary>
        /// 从文件夹中获取指定数量的涩图
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<FileInfo> getRandomFileInList(string dirPath, int count)
        {
            List<FileInfo> stList = new List<FileInfo>();
            DirectoryInfo sourceDirectory = new DirectoryInfo(dirPath);
            DirectoryInfo[] directoryInfos = sourceDirectory.GetDirectories();
            while (stList.Count < count)
            {
                DirectoryInfo randomDirectory = directoryInfos.Length == 0 ? sourceDirectory : directoryInfos[RandomHelper.getRandomBetween(0, directoryInfos.Length - 1)];
                FileInfo[] fileInfos = randomDirectory.GetFiles();
                int randomFileIndex = RandomHelper.getRandomBetween(0, fileInfos.Length - 1);
                FileInfo randomFile = fileInfos[randomFileIndex];
                if (stList.Where(m => m.FullName == randomFile.FullName).ToList().Count > 0) continue;
                stList.Add(randomFile);
            }
            return stList;
        }

        public static int getSTLeftUseToday(CQGroupMessageEventArgs e, int diff = 1)
        {
            if (Setting.Permissions.LimitlessGroups.Contains(e.FromGroup.Id)) return Setting.Robot.MaxSTUseOneDay;
            FunctionRecordBusiness functionRecordBusiness = new FunctionRecordBusiness();
            int[] functionTypeArr = new int[] { FunctionType.PixivicST.TypeId, FunctionType.PixivGeneralST.TypeId, FunctionType.PixivR18ST.TypeId };
            int todayUseCount = functionRecordBusiness.getUsedCountToday(e.FromGroup.Id, e.FromQQ.Id, functionTypeArr);
            int leftToday = Setting.Robot.MaxSTUseOneDay - todayUseCount - diff;
            return leftToday < 0 ? 0 : leftToday;
        }

        public static bool isPixivCookieExpire(CQGroupMessageEventArgs e)
        {
            if (DateTime.Now <= Setting.Pixiv.CookieExpireDate) return false;
            e.SendMessageWithAt(CQApi.CQCode_Image("face/face06.gif").ToSendString() + "cookie过期了，让群主更新cookie吧~");
            return true;
        }

        public static bool isGroupAllowST(CQGroupMessageEventArgs e)
        {
            if (Setting.Permissions.STGroups.Contains(e.FromGroup.Id)) return true;
            e.SendMessageWithAt(" 这个功能暂时未开启哦~");
            return false;
        }

        public static bool isGroupAllowR18ST(CQGroupMessageEventArgs e)
        {
            if (Setting.Permissions.R18Groups.Contains(e.FromGroup.Id)) return true;
            e.SendMessageWithAt(" 这个功能暂时未开启哦~");
            return false;
        }

        public static bool isGroupAllowCustomST(CQGroupMessageEventArgs e)
        {
            if (Setting.Permissions.STCustomGroups.Contains(e.FromGroup.Id)) return true;
            e.SendMessageWithAt(" 自定义涩图功能暂时关闭，请直接使用 #涩图 命令");
            return false;
        }

        public static bool checkSTBanWord(CQGroupMessageEventArgs e)
        {
            string message = e.Message.Text.Trim().ToLower();
            if (message.Contains("r18") || message.Contains("r17")) return true;
            string banWord = StringHelper.isContainsWord(e.Message.Text, Setting.Word.BanSTKeyWord);
            return string.IsNullOrEmpty(banWord) == false;
        }

        public static void sendErrorMessage(CQApi CQApi, Exception ex, string message = "", bool limit = true)
        {
            try
            {
                if (ex.Message.Contains("操作超时")) return;
                if (limit && checkCanSendError() == false) return;
                StringBuilder errorBuilder = new StringBuilder();
                errorBuilder.AppendLine(string.Format("[{0}]：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message));
                errorBuilder.AppendLine(string.Format("[Message]：{0}", ex.Message));
                errorBuilder.AppendLine(string.Format("[StackTrace]：{0}", ex.StackTrace));
                CQApi.SendGroupMessage(Setting.Group.RobotGroupId, errorBuilder.ToString());
            }
            catch (Exception)
            {
            }
        }

        public static void sendErrorMessage(CQApi CQApi, string message = "", bool limit = true)
        {
            try
            {
                if (limit && checkCanSendError() == false) return;
                StringBuilder errorBuilder = new StringBuilder();
                errorBuilder.AppendLine(string.Format("[{0}]：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message));
                CQApi.SendGroupMessage(Setting.Group.RobotGroupId, errorBuilder.ToString());
            }
            catch (Exception)
            {
            }
        }

        public static bool checkCanSendError()
        {
            DateTime dateTime = DateTime.Now;
            if (Setting.Robot.LastErrorSendHour != dateTime.Hour)
            {
                Setting.Robot.LastErrorSendHour = dateTime.Hour;
                Setting.Robot.LastSendEachHour = Setting.Robot.ErrorSendEachHour;
            }
            if (Setting.Robot.LastSendEachHour <= 0) return false;
            Setting.Robot.LastSendEachHour--;
            return true;
        }


        public static DateTime getMorningClockStartTime()
        {
            DateTime nowDate = DateTime.Now;
            TimeSpan nowTime = nowDate.TimeOfDay;
            TimeSpan timeSpan = Setting.Clock.MorningClockStart;
            if (nowTime > Setting.Clock.NightClockStart) nowDate = nowDate.AddDays(1);
            DateTime dateTime = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            //CQHelper.CQLog.Info("getMorningClockStartTime", dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            return dateTime;
        }

        public static DateTime getMorningClockEndTime()
        {
            DateTime nowDate = DateTime.Now;
            TimeSpan nowTime = nowDate.TimeOfDay;
            TimeSpan timeSpan = Setting.Clock.MorningClockEnd;
            if (nowTime > Setting.Clock.NightClockStart) nowDate = nowDate.AddDays(1);
            DateTime dateTime = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            //CQHelper.CQLog.Info("getMorningClockEndTime", dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            return dateTime;
        }

        public static DateTime getNightClockStartTime()
        {
            DateTime nowDate = DateTime.Now;
            TimeSpan nowTime = nowDate.TimeOfDay;
            TimeSpan timeSpan = Setting.Clock.NightClockStart;
            if (nowTime < Setting.Clock.NightClockStart) nowDate = nowDate.AddDays(-1);
            DateTime dateTime = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            //CQHelper.CQLog.Info("getNightClockStartTime", dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            return dateTime;
        }

        public static DateTime getNightClockEndTime()
        {
            DateTime nowDate = DateTime.Now;
            TimeSpan nowTime = nowDate.TimeOfDay;
            TimeSpan timeSpan = Setting.Clock.NightClockEnd;
            if (nowTime > Setting.Clock.NightClockStart) nowDate = nowDate.AddDays(1);
            DateTime dateTime = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            //CQHelper.CQLog.Info("getNightClockEndTime", dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            return dateTime;
        }


    }
}
