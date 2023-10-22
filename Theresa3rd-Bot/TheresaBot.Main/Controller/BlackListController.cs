using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.DTO;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Model.VO;
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
            BanMemberDatas.LoadDatas();
            var banList = BanMemberDatas.BanMemberList;
            var banInfos = banList.Select(o => new BanMemberVo
            {
                Id = o.Id,
                MemberId = o.MemberId,
                CreateAt = o.CreateDate.ToTimeStamp(),
                CreateDate = o.CreateDate.ToSimpleString()
            }).ToList();
            return ApiResult.Success(banInfos);
        }

        [HttpPost]
        [Authorize]
        [Route("add/member")]
        public ApiResult AddMember([FromBody] AddBanMemberDto member)
        {
            banMemberService.insertBanMembers(member.MemberId);
            BanMemberDatas.LoadDatas();
            return ApiResult.Success();
        }

        [HttpPost]
        [Authorize]
        [Route("del/member")]
        public ApiResult DelMember(int[] ids)
        {
            banMemberService.DelByIds(ids);
            BanMemberDatas.LoadDatas();
            return ApiResult.Success();
        }

        [HttpGet]
        [Authorize]
        [Route("list/tag")]
        public ApiResult GetTags()
        {
            BanTagDatas.LoadDatas();
            var banList = BanTagDatas.BanTagList;
            var banInfos = banList.Select(o => new BanTagVo
            {
                Id = o.Id,
                KeyWord = o.KeyWord,
                FullMatch = o.FullMatch,
                IsRegular = o.IsRegular,
                CreateAt = o.CreateDate.ToTimeStamp(),
                CreateDate = o.CreateDate.ToSimpleString()
            }).ToList();
            return ApiResult.Success(banInfos);
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
