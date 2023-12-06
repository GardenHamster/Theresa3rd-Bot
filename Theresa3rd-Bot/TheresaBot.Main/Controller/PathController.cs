using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Services;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PathController : BaseController
    {
        private PathService pathService;

        public PathController()
        {
            this.pathService = new PathService();
        }

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
                var pathList = pathService.LoadFacePath();
                return ApiResult.Success(pathList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("list/mask")]
        public ApiResult ListMask()
        {
            try
            {
                var pathList = pathService.LoadMaskPath();
                return ApiResult.Success(pathList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }


    }
}
