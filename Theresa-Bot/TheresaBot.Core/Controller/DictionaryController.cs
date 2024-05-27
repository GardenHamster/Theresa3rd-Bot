using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Core.Datas;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.DTO;
using TheresaBot.Core.Model.Result;
using TheresaBot.Core.Model.VO;
using TheresaBot.Core.Services;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Controller
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
