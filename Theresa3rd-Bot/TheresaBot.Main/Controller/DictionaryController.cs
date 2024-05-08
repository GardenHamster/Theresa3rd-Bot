using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.DTO;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Model.VO;
using TheresaBot.Main.Services;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class DictionaryController : BaseController
    {
        private DictionaryService dictionaryService;

        public DictionaryController()
        {
            dictionaryService = new DictionaryService();
        }

        [HttpGet]
        [Authorize]
        [Route("list")]
        public ApiResult GetDictionary()
        {
            var datas = dictionaryService.GetDictionary();
            var dataInfos = datas.Select(o => new DictionaryVo
            {
                Id = o.Id,
                Words = o.Words,
                WordType = o.WordType,
                SubType = o.SubType,
                CreateAt = o.CreateDate.ToTimeStamp()
            }).ToList();
            return ApiResult.Success(dataInfos);
        }

        [HttpPost]
        [Authorize]
        [Route("add")]
        public ApiResult AddDictionary([FromBody] AddDictionaryDto datas)
        {
            var words = datas.Words ?? new();
            if (words.Count == 0) return ApiResult.ParamError;
            dictionaryService.InsertDictionary(datas.WordType, words, datas.SubType);
            return ApiResult.Success();
        }

        [HttpPost]
        [Authorize]
        [Route("del")]
        public ApiResult DelDictionary(int[] ids)
        {
            dictionaryService.DelByIds(ids);
            return ApiResult.Success();
        }




    }
}
