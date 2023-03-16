using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Helper
{
    public static class ContentHelper
    {
        public static SetuContent ToResendContent(this SetuContent setuContents, ResendType resendType)
        {
            if (resendType == ResendType.WithoutImg)
            {
                return null;
            }
            if (resendType == ResendType.WithoutImg)
            {
                return setuContents with { SetuImages = new() };
            }
            if (resendType == ResendType.Rotate180)
            {
                return setuContents with { SetuImages = setuContents.SetuImages.Rotate180() };
            }
            if (resendType == ResendType.Blur)
            {
                return setuContents with { SetuImages = setuContents.SetuImages.Blur(5) };
            }
            throw new Exception($"未能将SetuContent转换为{resendType}");
        }

    }
}
