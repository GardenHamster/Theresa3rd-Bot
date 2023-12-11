using System.Xml;
using System.Xml.Linq;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Data;
using TheresaBot.Main.Model.Pixiv;

namespace TheresaBot.Main.Datas
{
    public static class CountDatas
    {
        /// <summary>
        /// 本次运行数据
        /// </summary>
        public static CountData SingleCount { get; } = new CountData();

        /// <summary>
        /// 历史运行数据
        /// </summary>
        public static CountData TotalCount { get; } = new CountData();

        /// <summary>
        /// 累加指令处理次数
        /// </summary>
        public static void AddHandleTimes()
        {
            lock (TotalCount)
            {
                SingleCount.HandleTimes++;
                TotalCount.HandleTimes++;
                SaveDatas();
            }
        }

        /// <summary>
        /// 累加Pixiv作品推送次数
        /// </summary>
        public static void AddPixivPushTimes(PixivScanReport report)
        {
            lock (TotalCount)
            {
                SingleCount.PixivPushTimes += report.PushWork;
                TotalCount.PixivPushTimes += report.PushWork;
                SaveDatas();
            }
        }

        /// <summary>
        /// 累加Pixiv作品扫描次数
        /// </summary>
        public static void AddPixivScanTimes(PixivScanReport report)
        {
            lock (TotalCount)
            {
                SingleCount.PixivScanTimes += report.ScanWork;
                SingleCount.PixivScanError += report.ErrorWork;
                TotalCount.PixivScanTimes += report.ScanWork;
                TotalCount.PixivScanError += report.ErrorWork;
                SaveDatas();
            }
        }

        /// <summary>
        /// 从本地文件中加载统计数据
        /// </summary>
        public static void LoadDatas()
        {
            try
            {
                string xmlPath = DataPath.GetCountPath();
                if (File.Exists(xmlPath) == false) return;
                var doc = XDocument.Load(xmlPath);
                TotalCount.HandleTimes = doc.Root?.Element("HandleTimes")?.Value?.ToInt() ?? 0;
                TotalCount.PixivPushTimes = doc.Root?.Element("PixivPushTimes")?.Value?.ToInt() ?? 0;
                TotalCount.PixivScanTimes = doc.Root?.Element("PixivScanTimes")?.Value?.ToInt() ?? 0;
                TotalCount.PixivScanError = doc.Root?.Element("PixivScanError")?.Value?.ToInt() ?? 0;
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
                string xmlPath = DataPath.GetCountPath();
                XmlWriterSettings setting = new XmlWriterSettings() { Indent = true };
                using XmlWriter writer = XmlWriter.Create(xmlPath, setting);
                XDocument doc = new XDocument(
                    new XElement("Data",
                        new XElement("HandleTimes", TotalCount.HandleTimes),
                        new XElement("PixivPushTimes", TotalCount.PixivPushTimes),
                        new XElement("PixivScanTimes", TotalCount.PixivScanTimes),
                        new XElement("PixivScanError", TotalCount.PixivScanError)
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
