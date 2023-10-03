using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.VO;

namespace TheresaBot.Main.Business
{
    internal class PathBusiness
    {

        public List<ImagePathVo> LoadFacePath()
        {
            var botImgPath = FilePath.GetBotImgDirectory();
            var facePath = FilePath.GetFaceDirectory();
            var fileInfos = FileHelper.SearchFiles(facePath);
            var imgPaths = new List<ImagePathVo>();
            foreach (var fileInfo in fileInfos)
            {
                var serverPath = fileInfo.GetRelativePath(botImgPath);
                var httpPath = Path.Combine(FilePath.ImgHttpPath, fileInfo.GetRelativePath(facePath)).Replace(@"\", "/");
                var pathVo = new ImagePathVo(httpPath, serverPath);
                imgPaths.Add(pathVo);
            }
            return imgPaths;
        }

    }
}
