namespace TheresaBot.Main.Common
{
    public static class JwtConfig
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public static string SecretKey = "84c421c5-794b-4af7-46e1-92da7f1ed7f8";

        /// <summary>
        /// 颁发者
        /// </summary>
        public const string Issuer = "GardenHamster";

        /// <summary>
        /// 接收者
        /// </summary>
        public const string Audience = "GardenHamster";

        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public const int ExpiredSeconds = 30 * 24 * 60 * 60;

    }
}
