using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Model.DTO;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Services;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlackListController : BaseController
    {
        private BanTagService banTagService;
        private BanMemberService banMemberService;

        public BlackListController()
        {
            banTagService = new BanTagService();
            banMemberService = new BanMemberService();
        }

        [HttpGet]
        [Authorize]
        [Route("list/member")]
        public ApiResult GetMembers()
        {
            return ApiResult.Success(BanMemberDatas.BanMemberList);
        }

        [HttpPost]
        [Authorize]
        [Route("add/member")]
        public ApiResult AddMember(long memberId)
        {
            banMemberService.insertBanMembers(memberId);
            BanMemberDatas.LoadDatas();
            return ApiResult.Success();
        }

        [HttpPost]
        [Authorize]
        [Route("del/member")]
        public ApiResult DelMember(int id)
        {
            banMemberService.DelById(id);
            BanMemberDatas.LoadDatas();
            return ApiResult.Success();
        }

        [HttpGet]
        [Authorize]
        [Route("list/tag")]
        public ApiResult GetTags()
        {
            return ApiResult.Success(BanTagDatas.BanTagList);
        }

        [HttpPost]
        [Authorize]
        [Route("add/tag")]
        public ApiResult AddTag([FromBody] AddBanTagDto tag)
        {
            banTagService.InsertBanTag(tag.Keyword, tag.TagMatchType);
            BanTagDatas.LoadDatas();
            return ApiResult.Success();
        }

        [HttpPost]
        [Authorize]
        [Route("del/tag")]
        public ApiResult DelTag(int id)
        {
            banTagService.DelById(id);
            BanTagDatas.LoadDatas();
            return ApiResult.Success();
        }


    }
}
