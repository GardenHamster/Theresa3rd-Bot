using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Result;

namespace TheresaBot.Main.Controller
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


    }
}
