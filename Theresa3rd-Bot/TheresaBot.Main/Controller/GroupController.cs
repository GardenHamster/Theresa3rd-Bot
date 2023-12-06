using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Model.VO;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : BaseController
    {
        private BaseSession Session;

        public GroupController(BaseSession session)
        {
            Session = session;
        }

        [HttpGet]
        [Authorize]
        [Route("list")]
        public ApiResult GetGroups()
        {
            var groupInfos = BotConfig.GroupInfos.Select(o => new GroupInfoVo()
            {
                GroupId = o.GroupId,
                GroupName = o.GroupName
            });
            return ApiResult.Success(groupInfos);
        }

        [HttpGet]
        [Authorize]
        [Route("load")]
        public async Task<ApiResult> LoadGroups()
        {
            var groupInfos = await Session.LoadGroupInfosAsync();
            if (groupInfos is null) return ApiResult.Fail("加载群列表失败");
            return ApiResult.Success(groupInfos);
        }




    }
}
