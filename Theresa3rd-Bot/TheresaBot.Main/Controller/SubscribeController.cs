using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Business;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Api;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscribeController : BaseController
    {
        private SubscribeBusiness subscribeBusiness;

        public SubscribeController()
        {
            this.subscribeBusiness = new SubscribeBusiness();
        }

        [HttpGet]
        [Authorize]
        [Route("get/pixiv/user")]
        public ApiResult GetPixivSubscribes()
        {
            var subscribeDatas = subscribeBusiness.getSubscribeDatas(SubscribeType.P站画师);
            var subscribeList = subscribeDatas.Select(o => new SubscribeInfoVo
            {
                GroupId = o.GroupId,
                SubscribeId = o.SubscribeId,
                SubscribeCode = o.SubscribeCode,
                SubscribeType = o.SubscribeType,
                SubscribeName = o.SubscribeName
            });
            return ApiResult.Success(subscribeDatas);
        }


    }
}
