using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TheresaBot.Main.Common;

namespace TheresaBot.Main.Helper
{
    public static class IPAddressHelper
    {
        /// <summary>
        /// 获取本机局域网IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .Where(p => p.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(p.Address))
                .FirstOrDefault()?.Address.ToString();
        }

        public static List<string> GetLocalBackstageUrls()
        {
            var localIp = GetLocalIP();
            var urls = new List<string>();
            urls.AddRange(BotConfig.ServerAddress.Select(o => o.Replace("0.0.0.0", "127.0.0.1")));
            urls.AddRange(BotConfig.ServerAddress.Select(o => o.Replace("0.0.0.0", localIp)));
            return urls;
        }

    }
}
