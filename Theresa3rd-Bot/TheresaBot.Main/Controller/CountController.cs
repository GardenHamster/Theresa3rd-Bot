using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
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

        public CountController()
        {
            subscribeService = new SubscribeService();
        }

        [HttpGet]
        [Authorize]
        [Route("single")]
        public ApiResult GetSingle()
        {
            RunningDataVo data = new RunningDataVo()
            {
                RunningSeconds = DateTime.Now.SecondDiff(RunningDatas.StartTime),
                HandleTimes = RunningDatas.TotalCount.HandleTimes,
                PixivPushTimes = RunningDatas.TotalCount.PixivPushTimes,
                PixivScanTimes = RunningDatas.TotalCount.PixivScanTimes,
                PixivScanError = RunningDatas.TotalCount.PixivScanError,
            };
            return ApiResult.Success(data);
        }

        [HttpGet]
        [Authorize]
        [Route("total")]
        public ApiResult GetTotal()
        {
            RunningDataVo data = new RunningDataVo()
            {
                RunningSeconds = RunningDatas.TotalCount.RunningSeconds,
                HandleTimes = RunningDatas.TotalCount.HandleTimes,
                PixivPushTimes = RunningDatas.TotalCount.PixivPushTimes,
                PixivScanTimes = RunningDatas.TotalCount.PixivScanTimes,
                PixivScanError = RunningDatas.TotalCount.PixivScanError,
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
                PixivUserSubs = subscribeService.countSubscribes(SubscribeType.P站画师),
                PixivTagSubs = subscribeService.countSubscribes(SubscribeType.P站标签),
                MysUserSubs = subscribeService.countSubscribes(SubscribeType.米游社用户),
            };
            return ApiResult.Success(data);
        }



    }
}
