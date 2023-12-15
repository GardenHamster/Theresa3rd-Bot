using Microsoft.AspNetCore.Authorization;
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
    public class SugarController : BaseController
    {
        private SugarTagService sugarTagService;

        public SugarController()
        {
            sugarTagService = new SugarTagService();
        }

        [HttpGet]
        [Authorize]
        [Route("list/tag")]
        public ApiResult GetTags()
        {
            var sugars = sugarTagService.GetList();
            var sugarInfos = sugars.Select(o => new SugarTagVo
            {
                Id = o.Id,
                Keyword = o.KeyWord,
                BindTags = o.BindTags
            }).ToList();
            return ApiResult.Success(sugarInfos);
        }

        [HttpPost]
        [Authorize]
        [Route("add/tag")]
        public ApiResult AddTags([FromBody] AddSugarTagDto sugar)
        {
            var bingTags = sugar.BindTags;
            var keyWords = sugar.Keyword.SplitParams();
            if (keyWords.Length == 0) return ApiResult.ParamError;
            if (string.IsNullOrWhiteSpace(bingTags)) return ApiResult.ParamError;
            sugarTagService.SetSugarTags(keyWords, bingTags);
            SugarTagDatas.LoadDatas();
            return ApiResult.Success();
        }

        [HttpPost]
        [Authorize]
        [Route("del/tag")]
        public ApiResult DelTag(int[] ids)
        {
            sugarTagService.DelById(ids);
            SugarTagDatas.LoadDatas();
            return ApiResult.Success();
        }

    }
}
