using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Util
{
    public static class PixivHelper
    {
        private static string DNS_AND_SNI = "www.pixivision.net";

        private static SocketsHttpHandler handler = new SocketsHttpHandler()
        {
            ConnectCallback = (info, token) =>
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(DNS_AND_SNI, 443);
                var stream = new NetworkStream(socket, true);
                SslStream sslstream = new SslStream(stream, false);
                sslstream.AuthenticateAsClient(new SslClientAuthenticationOptions
                {
                    TargetHost = DNS_AND_SNI,
                    ApplicationProtocols = new List<SslApplicationProtocol>(new SslApplicationProtocol[] { SslApplicationProtocol.Http11 })
                });
                return new ValueTask<Stream>(sslstream);
            }
        };

        public static async Task<string> HttpGetAsync(string url, Dictionary<string, string> headerDic = null, int timeout = 15000)
        {
            HttpClient client = new HttpClient(handler);
            client.DefaultRequestVersion = HttpVersion.Version11;
            return await client.GetStringAsync(url);
        }








    }
}
