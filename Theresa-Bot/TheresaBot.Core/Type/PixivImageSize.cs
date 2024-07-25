namespace TheresaBot.Core.Type
{
    public static class PixivImageSize
    {
        public const string Thumb = "thumb";
        public const string Small = "small";
        public const string Regular = "regular";
        public const string Original = "original";

        public static List<string> GetAllSizes() => new List<string> { Thumb, Small, Regular, Original };

    }
}
