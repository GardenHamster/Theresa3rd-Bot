using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Helper
{
    public static class ContentHelper
    {
        public static SetuContent ToResendContent(this SetuContent setuContent, ResendType resendType)
        {
            if (resendType == ResendType.None)
            {
                return null;
            }
            if (resendType == ResendType.WithoutImg)
            {
                return setuContent with { SetuImages = new() };
            }
            if (resendType == ResendType.Rotate180)
            {
                return setuContent with { SetuImages = setuContent.SetuImages.Rotate180() };
            }
            if (resendType == ResendType.Blur)
            {
                return setuContent with { SetuImages = setuContent.SetuImages.Blur(5) };
            }
            throw new Exception($"未能将SetuContent转换为{resendType}");
        }

        public static List<SetuContent> ToResendContent(this List<SetuContent> setuContents, ResendType resendType)
        {
            return setuContents.Select(o => o.ToResendContent(resendType)).ToList();
        }

        public static List<BaseContent> ToBaseContent(this List<SetuContent> setuContents)
        {
            var contentList = new List<BaseContent>();
            foreach (SetuContent setuContent in setuContents)
            {
                contentList.AddRange(setuContent.SetuInfos);
                contentList.AddRange(setuContent.SetuImages.ToLocalImageContent());
            }
            return contentList;
        }

        public static List<BaseContent>[] ToBaseContents(this List<SetuContent> setuContents)
        {
            var contentLists = new List<List<BaseContent>>();
            foreach (SetuContent setuContent in setuContents)
            {
                contentLists.Add(setuContent.SetuInfos.Concat(setuContent.SetuImages.ToLocalImageContent()).ToList());
            }
            return contentLists.ToArray();
        }

        public static List<BaseContent> ToBaseContent(this List<FileInfo> imgList)
        {
            return imgList.ToLocalImageContent().Cast<BaseContent>().ToList();
        }

        public static List<LocalImageContent> ToLocalImageContent(this List<FileInfo> imgList)
        {
            return imgList.Select(o => new LocalImageContent(o)).ToList();
        }

        public static List<BaseContent>[] SetDefaultImage(this List<BaseContent>[] contentList)
        {
            return contentList.Select(o => o.SetDefaultImage()).ToArray();
        }

        public static List<BaseContent> SetDefaultImage(this List<BaseContent> contentList)
        {
            FileInfo errorImg = FilePath.GetDownErrorImg();
            for (int i = 0; i < contentList.Count; i++)
            {
                if (contentList[i] is not LocalImageContent imageContent) continue;
                if (imageContent.FileInfo is not null) continue;
                if (errorImg is not null) imageContent.FileInfo = errorImg;
            }
            return contentList;
        }

        

    }
}
