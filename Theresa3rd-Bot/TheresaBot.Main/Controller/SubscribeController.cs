using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.DTO;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Model.VO;
using TheresaBot.Main.Services;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscribeController : BaseController
    {
        private SubscribeService subscribeService;

        public SubscribeController()
        {
            this.subscribeService = new SubscribeService();
        }

        [HttpGet]
        [Authorize]
        [Route("list/pixiv/user")]
        public ApiResult GetPixivUserList()
        {
            var subscribeDatas = subscribeService.getSubscribeDatas(SubscribeType.P站画师);
            var subscribeList = subscribeDatas.Select(ToSubscribeVo).ToList();
            return ApiResult.Success(subscribeList);
        }

        [HttpGet]
        [Authorize]
        [Route("list/pixiv/tag")]
        public ApiResult GetPixivTagList()
        {
            var subscribeDatas = subscribeService.getSubscribeDatas(SubscribeType.P站标签);
            var subscribeList = subscribeDatas.Select(ToSubscribeVo).ToList();
            return ApiResult.Success(subscribeList);
        }

        [HttpGet]
        [Authorize]
        [Route("list/miyoushe/user")]
        public ApiResult GetMiyousheUserList()
        {
            var subscribeDatas = subscribeService.getSubscribeDatas(SubscribeType.米游社用户);
            var subscribeList = subscribeDatas.Select(ToSubscribeVo).ToList();
            return ApiResult.Success(subscribeList);
        }

        [HttpPost]
        [Authorize]
        [Route("delete")]
        public ApiResult Delete([FromBody] IdsDto idsDto)
        {
            if (idsDto.Ids is null) return ApiResult.ParamError;
            if (idsDto.Ids.Count == 0) return ApiResult.ParamError;
            subscribeService.deleteSubscribeGroup(idsDto.Ids);
            return ApiResult.Success("退订成功");
        }

        private SubscribeInfoVo ToSubscribeVo(SubscribeInfo subscribeInfo)
        {
            return new SubscribeInfoVo
            {
                Id = subscribeInfo.Id,
                GroupId = subscribeInfo.GroupId,
                SubscribeId = subscribeInfo.SubscribeId,
                SubscribeCode = subscribeInfo.SubscribeCode,
                SubscribeType = subscribeInfo.SubscribeType,
                SubscribeName = subscribeInfo.SubscribeName,
                SubscribeAt = subscribeInfo.SubscribeDate.ToTimeStamp(),
                SubscribeDate = subscribeInfo.SubscribeDate.ToSimpleString()
            };
        }


    }
}
