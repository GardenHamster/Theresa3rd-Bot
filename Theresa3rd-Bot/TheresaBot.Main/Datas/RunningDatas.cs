using System;
using System.Xml;
using System.Xml.Linq;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Data;
using TheresaBot.Main.Model.Pixiv;

namespace TheresaBot.Main.Datas
{
    internal static class RunningDatas
    {
        /// <summary>
        /// 启动时间
        /// </summary>
        public static DateTime StartTime { get; } = DateTime.Now;

        /// <summary>
        /// 运行数据
        /// </summary>
        public static CountData CountData { get; } = new CountData();

        /// <summary>
        /// 从本地文件中加载数据
        /// </summary>
        public static void LoadDatas()
        {
            LoadCountDatas();
        }

        /// <summary>
        /// 累加指令处理次数
        /// </summary>
        public static void AddHandleTimes()
        {
            lock (CountData)
            {
                CountData.TotalHandle++;
                SaveCountDatas();
            }
        }

        /// <summary>
        /// 累加Pixiv作品推送次数
        /// </summary>
        public static void AddTotalPixivPush(int count = 1)
        {
            lock (CountData)
            {
                CountData.TotalPixivPush += count;
                SaveCountDatas();
            }
        }

        /// <summary>
        /// 累加Pixiv作品扫描次数
        /// </summary>
        public static void AddTotalPixivScan(PixivScanReport report)
        {
            lock (CountData)
            {
                CountData.TotalPixivScan += report.ScanWork;
                CountData.TotalPixivScanError += report.ErrorWork;
                SaveCountDatas();
            }
        }

        /// <summary>
        /// 从本地文件中加载统计数据
        /// </summary>
        private static void LoadCountDatas()
        {
            try
            {
                string xmlPath = DataPath.GetCountPath();
                if (File.Exists(xmlPath) == false) return;
                var doc = XDocument.Load(xmlPath);
                CountData.TotalHandle = doc.Root?.Element("TotalHandle")?.Value?.ToInt() ?? 0;
                CountData.TotalPixivPush = doc.Root?.Element("TotalPixivPush")?.Value?.ToInt() ?? 0;
                CountData.TotalPixivScan = doc.Root?.Element("TotalPixivScan")?.Value?.ToInt() ?? 0;
                CountData.TotalPixivScanError = doc.Root?.Element("TotalPixivScanError")?.Value?.ToInt() ?? 0;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Data文件加载失败");
            }
        }

        /// <summary>
        /// 保存统计数据到本地文件
        /// </summary>
        private static void SaveCountDatas()
        {
            try
            {
                string xmlPath = DataPath.GetCountPath();
                XmlWriterSettings setting = new XmlWriterSettings() { Indent = true };
                using XmlWriter writer = XmlWriter.Create(xmlPath, setting);
                XDocument doc = new XDocument(
                    new XElement("Data",
                        new XElement("TotalHandle", CountData.TotalHandle),
                        new XElement("TotalPixivPush", CountData.TotalPixivPush),
                        new XElement("TotalPixivScan", CountData.TotalPixivScan),
                        new XElement("TotalPixivScanError", CountData.TotalPixivScanError)
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
