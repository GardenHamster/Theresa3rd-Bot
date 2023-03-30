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
                if (contentList.Count > 0)
                {
                    contentList.Add(new PlainContent(string.Empty, true));
                    contentList.Add(new PlainContent(string.Empty, true));
                }
                contentList.AddRange(setuContent.SetuInfos);
                contentList.AddRange(setuContent.SetuImages.ToLocalImageContent());
            }
            return contentList;
        }

        public static List<LocalImageContent> ToLocalImageContent(this List<FileInfo> imgLists)
        {
            return imgLists.Select(o => new LocalImageContent(o)).ToList();
        }

    }
}
