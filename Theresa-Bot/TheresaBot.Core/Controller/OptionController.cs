using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Result;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OptionController : BaseController
    {
        [HttpGet]
        [Authorize]
        [Route("list/imgsize")]
        public ApiResult ListImgSize()
        {
            try
            {
                var sizeList = new string[] { "thumb", "small", "regular", "original" };
                return ApiResult.Success(sizeList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("list/resend")]
        public ApiResult ListResend()
        {
            try
            {
                var optionList = EnumHelper.ResendOptions.ToOptionList();
                return ApiResult.Success(optionList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("list/tag/match")]
        public ApiResult ListTagMatch()
        {
            try
            {
                var optionList = EnumHelper.TagMatchOptions.ToOptionList();
                return ApiResult.Success(optionList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("list/pixiv/random")]
        public ApiResult ListPixivRandom()
        {
            try
            {
                var optionList = EnumHelper.PixivRandomOptions.ToOptionList();
                return ApiResult.Success(optionList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("list/pixiv/user/scan")]
        public ApiResult ListPixivUserScan()
        {
            try
            {
                var optionList = EnumHelper.PixivUserScanOptions.ToOptionList();
                return ApiResult.Success(optionList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("list/timing/setu/source")]
        public ApiResult ListTimingSetuSource()
        {
            try
            {
                var optionList = EnumHelper.TimingSetuSourceOptions.ToOptionList();
                return ApiResult.Success(optionList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("list/pixiv/ranking/sort")]
        public ApiResult ListPixivRankingSort()
        {
            try
            {
                var optionList = EnumHelper.PixivRankingSortOptions.ToOptionList();
                return ApiResult.Success(optionList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("list/dictionary/type")]
        public ApiResult ListDictionaryType()
        {
            try
            {
                var optionList = EnumHelper.DictionaryTypeOptions.ToOptionList();
                optionList.AddSubOptions((int)DictionaryType.WordCloud, EnumHelper.WordcloudTypeOptions);
                return ApiResult.Success(optionList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }


    }
}
