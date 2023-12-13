using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Model.VO;
using TheresaBot.Main.Services;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountController : BaseController
    {
        private SubscribeService subscribeService;
        private SubscribeGroupService subscribeGroupService;

        public CountController()
        {
            subscribeService = new SubscribeService();
            subscribeGroupService = new SubscribeGroupService();
        }

        [HttpGet]
        [Authorize]
        [Route("single")]
        public ApiResult GetSingle()
        {
            CountDataVo data = new CountDataVo()
            {
                RunningSeconds = RunningDatas.RunningSeconds,
                HandleTimes = CountDatas.TotalCount.HandleTimes,
                PixivPushTimes = CountDatas.TotalCount.PixivPushTimes,
                PixivScanTimes = CountDatas.TotalCount.PixivScanTimes,
                PixivScanError = CountDatas.TotalCount.PixivScanError,
            };
            return ApiResult.Success(data);
        }

        [HttpGet]
        [Authorize]
        [Route("total")]
        public ApiResult GetTotal()
        {
            CountDataVo data = new CountDataVo()
            {
                RunningSeconds = RunningDatas.TotalSeconds,
                HandleTimes = CountDatas.TotalCount.HandleTimes,
                PixivPushTimes = CountDatas.TotalCount.PixivPushTimes,
                PixivScanTimes = CountDatas.TotalCount.PixivScanTimes,
                PixivScanError = CountDatas.TotalCount.PixivScanError,
            };
            return ApiResult.Success(data);
        }


        [HttpGet]
        [Authorize]
        [Route("subscribe")]
        public ApiResult GetSubscribe()
        {
            SubscribeDataVo data = new SubscribeDataVo()
            {
                PixivUserSubs = subscribeGroupService.CountSubscribes(SubscribeType.P站画师),
                PixivTagSubs = subscribeGroupService.CountSubscribes(SubscribeType.P站标签),
                MysUserSubs = subscribeGroupService.CountSubscribes(SubscribeType.米游社用户),
            };
            return ApiResult.Success(data);
        }



    }
}
