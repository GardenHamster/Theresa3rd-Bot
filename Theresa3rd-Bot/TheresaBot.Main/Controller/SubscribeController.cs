using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Business;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.DTO;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Model.VO;
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
        [Route("list/pixiv/user")]
        public ApiResult GetPixivUserList()
        {
            var subscribeDatas = subscribeBusiness.getSubscribeDatas(SubscribeType.P站画师);
            var subscribeList = subscribeDatas.Select(o => ToSubscribeVo(o));
            return ApiResult.Success(subscribeDatas);
        }

        [HttpGet]
        [Authorize]
        [Route("list/pixiv/tag")]
        public ApiResult GetPixivTagList()
        {
            var subscribeDatas = subscribeBusiness.getSubscribeDatas(SubscribeType.P站标签);
            var subscribeList = subscribeDatas.Select(o => ToSubscribeVo(o));
            return ApiResult.Success(subscribeDatas);
        }

        [HttpPost]
        [Route("cancle")]
        public ApiResult Cancle([FromBody] CancleSubscribeDto cancleDto)
        {
            if (cancleDto.SubscribeIds is null) return ApiResult.ParamError;
            if (cancleDto.SubscribeIds.Count == 0) return ApiResult.ParamError;
            subscribeBusiness.cancleSubscribe(cancleDto.SubscribeIds);
            return ApiResult.Success("退订成功");
        }

        private SubscribeInfoVo ToSubscribeVo(SubscribeInfo subscribeInfo)
        {
            return new SubscribeInfoVo
            {
                GroupId = subscribeInfo.GroupId,
                SubscribeId = subscribeInfo.SubscribeId,
                SubscribeCode = subscribeInfo.SubscribeCode,
                SubscribeType = subscribeInfo.SubscribeType,
                SubscribeName = subscribeInfo.SubscribeName
            };
        }


    }
}
