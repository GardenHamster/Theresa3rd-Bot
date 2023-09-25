using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Model.VO;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : BaseController
    {
        [HttpGet]
        [Authorize]
        [Route("list")]
        public ApiResult GetGroup()
        {
            var groupInfos = BotConfig.GroupInfos.Select(o => new GroupInfoVo()
            {
                GroupId = o.GroupId,
                GroupName = o.GroupName
            });
            return ApiResult.Success(groupInfos);
        }

    }
}
