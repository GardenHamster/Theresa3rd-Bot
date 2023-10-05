namespace TheresaBot.Main.Model.Config
{
    public record BackstageConfig : BaseConfig
    {
        public string Password { get; set; }

        public string SecretKey { get; set; }

        public override BackstageConfig FormatConfig()
        {
            return this;
        }

    }
}
