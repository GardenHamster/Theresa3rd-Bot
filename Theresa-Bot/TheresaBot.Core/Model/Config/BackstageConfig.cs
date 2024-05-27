using TheresaBot.Core.Helper;

namespace TheresaBot.Core.Model.Config
{
    public record BackstageConfig : BaseConfig
    {
        public string Password { get; set; }

        public string SecretKey { get; set; }

        public override BackstageConfig FormatConfig()
        {
            if (string.IsNullOrWhiteSpace(SecretKey))
            {
                SecretKey = StringHelper.RandomUUID32();
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                Password = StringHelper.RandomUUID8();
            }
            return this;
        }

    }
}
