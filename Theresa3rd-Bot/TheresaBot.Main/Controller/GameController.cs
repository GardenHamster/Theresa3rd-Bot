using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.DTO;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Model.VO;
using TheresaBot.Main.Services;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : BaseController
    {
        private UCWordService wordService;

        public GameController()
        {
            this.wordService = new UCWordService();
        }

        [HttpGet]
        [Authorize]
        [Route("list/undercover/words")]
        public ApiResult GetUCWordList()
        {
            var words = wordService.GetWords();
            var wordList = words.Select(ToUCWordVo).ToList();
            return ApiResult.Success(wordList);
        }

        [HttpPost]
        [Authorize]
        [Route("add/undercover/words")]
        public ApiResult AddUCWords([FromBody] AddUCWordDto wordDto)
        {
            if (wordDto is null) return ApiResult.ParamError;
            if (string.IsNullOrWhiteSpace(wordDto.Word1)) return ApiResult.ParamError;
            if (string.IsNullOrWhiteSpace(wordDto.Word2)) return ApiResult.ParamError;
            wordService.InsertWords(wordDto.Word1, wordDto.Word2);
            return ApiResult.Success();
        }

        [HttpPost]
        [Authorize]
        [Route("auth/undercover/words")]
        public ApiResult AuthUCWords([FromBody] IdsDto idsDto)
        {
            if (idsDto.Ids is null) return ApiResult.ParamError;
            if (idsDto.Ids.Count == 0) return ApiResult.ParamError;
            wordService.AuthorizeWords(idsDto.Ids);
            return ApiResult.Success();
        }

        [HttpPost]
        [Authorize]
        [Route("delete/undercover/words")]
        public ApiResult DelUCWords([FromBody] IdsDto idsDto)
        {
            if (idsDto.Ids is null) return ApiResult.ParamError;
            if (idsDto.Ids.Count == 0) return ApiResult.ParamError;
            wordService.DeleteWords(idsDto.Ids);
            return ApiResult.Success();
        }

        private UCWordVo ToUCWordVo(UCWordPO word)
        {
            return new UCWordVo
            {
                Id = word.Id,
                Word1 = word.Word1,
                Word2 = word.Word2,
                IsAuthorized = word.IsAuthorized,
                CreateMember = word.CreateMember,
                CreateAt = word.CreateDate.ToTimeStamp()
            };
        }


    }
}
