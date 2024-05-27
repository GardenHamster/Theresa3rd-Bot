using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Core.Datas;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.DTO;
using TheresaBot.Core.Model.Result;
using TheresaBot.Core.Model.VO;
using TheresaBot.Core.Services;

namespace TheresaBot.Core.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlackListController : BaseController
    {
        private BanTagService banTagService;
        private BanMemberService banMemberService;
        private BanPixiverService banPixiverService;

        public BlackListController()
        {
            banTagService = new BanTagService();
            banMemberService = new BanMemberService();
            banPixiverService = new BanPixiverService();
        }

        [HttpGet]
        [Authorize]
        [Route("list/member")]
        public ApiResult GetMembers()
        {
            var banList = banMemberService.GetBanMembers();
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
            banMemberService.InsertBanMember(member.MemberId);
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
            var banList = banTagService.GetBanTags();
            var banInfos = banList.Select(o => new BanTagVo
            {
                Id = o.Id,
                Keyword = o.Keyword,
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
        public ApiResult DelTag(int[] ids)
        {
            banTagService.DelByIds(ids);
            BanTagDatas.LoadDatas();
            return ApiResult.Success();
        }

        [HttpGet]
        [Authorize]
        [Route("list/pixiver")]
        public ApiResult GetPixivers()
        {
            var banList = banPixiverService.GetBanPixivers();
            var banInfos = banList.Select(o => new BanPixiverVo
            {
                Id = o.Id,
                PixiverId = o.PixiverId,
                CreateAt = o.CreateDate.ToTimeStamp(),
                CreateDate = o.CreateDate.ToSimpleString()
            }).ToList();
            return ApiResult.Success(banInfos);
        }

        [HttpPost]
        [Authorize]
        [Route("add/pixiver")]
        public ApiResult AddPixiver([FromBody] AddBanPixiverDto pixiver)
        {
            banPixiverService.InsertBanPixivers(pixiver.PixiverId);
            BanPixiverDatas.LoadDatas();
            return ApiResult.Success();
        }

        [HttpPost]
        [Authorize]
        [Route("del/pixiver")]
        public ApiResult DelPixiver(int[] ids)
        {
            banPixiverService.DelByIds(ids);
            BanPixiverDatas.LoadDatas();
            return ApiResult.Success();
        }


    }
}
