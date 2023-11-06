using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Model.VO;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountController : BaseController
    {
        [HttpGet]
        [Authorize]
        [Route("running")]
        public ApiResult GetRunningData()
        {
            RunningDataVo data = new RunningDataVo()
            {
                RunningSeconds = DateTime.Now.SecondDiff(RunningDatas.StartTime),
                TotalHandle = RunningDatas.CountData.TotalHandle,
                TotalPixivPush = RunningDatas.CountData.TotalPixivPush,
                TotalPixivScan = RunningDatas.CountData.TotalPixivScan,
                TotalPixivScanError = RunningDatas.CountData.TotalPixivScanError,
            };
            return ApiResult.Success(data);
        }

    }
}
