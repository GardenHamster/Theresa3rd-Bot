using System.Xml;
using System.Xml.Linq;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Datas
{
    public static class RunningDatas
    {
        /// <summary>
        /// 更新锁
        /// </summary>
        private static object UpdateLock { get; } = new();

        /// <summary>
        /// 累计运行时间
        /// </summary>
        public static int TotalSeconds { get; private set; }

        /// <summary>
        /// 本次启动时间
        /// </summary>
        public static DateTime StartTime { get; } = DateTime.Now;

        /// <summary>
        /// 本次启动时间
        /// </summary>
        public static long RunningSeconds => DateTime.Now.SecondDiff(StartTime);

        /// <summary>
        /// 本次启动时间
        /// </summary>
        public static long StartAt => StartTime.ToTimeStamp();

        /// <summary>
        /// 上一次启动时间
        /// </summary>
        public static long LastStartAt { get; private set; }

        /// <summary>
        /// 上一次启动时间
        /// </summary>
        public static DateTime LastStartTime => LastStartAt.ToDateTime();

        /// <summary>
        /// 累加运行时间(秒)
        /// </summary>
        public static void AddRunningSeconds(int seconds)
        {
            lock (UpdateLock)
            {
                TotalSeconds += seconds;
                SaveDatas();
            }
        }

        /// <summary>
        /// 是否发送启动消息
        /// </summary>
        /// <returns></returns>
        public static bool IsSendStartupMessage()
        {
            if (LastStartAt <= 0) return true;
            int diff = DateTime.Now.SecondDiff(LastStartTime);
            return diff > 60 * 60;
        }

        /// <summary>
        /// 从本地文件中加载统计数据
        /// </summary>
        public static void LoadDatas()
        {
            try
            {
                string xmlPath = DataPath.GetRunningPath();
                if (File.Exists(xmlPath) == false) return;
                var doc = XDocument.Load(xmlPath);
                LastStartAt = doc.Root?.Element("LastStartAt")?.Value?.ToInt() ?? 0;
                TotalSeconds = doc.Root?.Element("TotalSeconds")?.Value?.ToInt() ?? 0;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Data文件加载失败");
            }
        }

        /// <summary>
        /// 保存统计数据到本地文件
        /// </summary>
        public static void SaveDatas()
        {
            try
            {
                string xmlPath = DataPath.GetRunningPath();
                XmlWriterSettings setting = new XmlWriterSettings() { Indent = true };
                using XmlWriter writer = XmlWriter.Create(xmlPath, setting);
                XDocument doc = new XDocument(
                    new XElement("Data",
                        new XElement("LastStartAt", StartAt),
                        new XElement("TotalSeconds", TotalSeconds)
                    )
                );
                doc.Save(writer);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Data文件保存失败");
            }
        }

    }
}
