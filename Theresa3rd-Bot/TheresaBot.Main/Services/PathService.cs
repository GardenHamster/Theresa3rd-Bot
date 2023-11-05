using SkiaSharp;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.VO;

namespace TheresaBot.Main.Services
{
    internal class PathService
    {

        public List<ImagePathVo> LoadFacePath()
        {
            var botImgPath = FilePath.GetBotImgDirectory();
            var facePath = FilePath.GetFaceDirectory();
            return GetFilesPaths(botImgPath, facePath);
        }

        public List<ImagePathVo> LoadMaskPath()
        {
            var botImgPath = FilePath.GetBotImgDirectory();
            var maskPath = FilePath.GetMaskDirectory();
            return GetFilesPaths(botImgPath, maskPath);
        }

        public List<ImagePathVo> GetFilesPaths(string relativeDirPath, string fileDirPath)
        {
            var imgPaths = new List<ImagePathVo>();
            var fileInfos = FileHelper.SearchFiles(fileDirPath);
            foreach (var fileInfo in fileInfos)
            {
                var serverPath = fileInfo.GetRelativePath(relativeDirPath);
                var httpPath = Path.Combine(FilePath.ImgHttpPath, fileInfo.GetRelativePath(fileDirPath)).Replace(@"\", "/");
                var pathVo = new ImagePathVo(httpPath, serverPath);
                imgPaths.Add(pathVo);
            }
            return imgPaths;
        }

    }
}
