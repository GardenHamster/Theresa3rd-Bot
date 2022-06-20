using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;

namespace Theresa3rd_Bot.Handler
{
    public class SaucenaoHandler
    {
        private PixivBusiness pixivBusiness;

        public SaucenaoHandler()
        {
            pixivBusiness = new PixivBusiness();
        }

        public async Task searchSource(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            await Task.CompletedTask;
        }






    }
}
