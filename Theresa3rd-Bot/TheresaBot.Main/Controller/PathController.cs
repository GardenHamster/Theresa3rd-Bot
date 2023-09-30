using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Model.VO;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PathController : BaseController
    {
        [HttpGet]
        [Authorize]
        [Route("list/font")]
        public ApiResult ListFont()
        {
            try
            {
                var fontPath = FilePath.GetFontDirectory();
                var fileInfos = FileHelper.SearchFiles(fontPath);
                var filePath = fileInfos.GetRelativePath(fontPath);
                return ApiResult.Success(filePath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("list/face")]
        public ApiResult ListFace()
        {
            try
            {
                var botImgPath = FilePath.GetBotImgDirectory();
                var facePath = FilePath.GetFaceDirectory();
                var fileInfos = FileHelper.SearchFiles(facePath);
                var filePath = fileInfos.GetRelativePath(botImgPath);
                return ApiResult.Success(filePath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }

    }
}
