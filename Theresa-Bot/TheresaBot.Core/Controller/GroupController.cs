using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Core.Common;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Result;
using TheresaBot.Core.Model.VO;
using TheresaBot.Core.Session;

namespace TheresaBot.Core.Controller
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
            try
            {
                await BotHelper.LoadGroupInfosAsync(Session);
                return ApiResult.Success(BotConfig.GroupInfos);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群列表加载失败");
                return ApiResult.Fail("群列表加载失败");
            }
        }




    }
}
