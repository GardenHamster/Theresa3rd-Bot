namespace TheresaBot.Main.Model.Config
{
    public record WelcomeConfig : BasePluginConfig
    {
        public string Template { get; set; }

        public List<WelcomeSpecial> Special { get; set; } = new();

        public override WelcomeConfig FormatConfig()
        {
            if (Special is null) Special = new();
            foreach (var item in Special) item?.FormatConfig();
            return this;
        }
    }

    public record WelcomeSpecial : BaseConfig
    {
        public long GroupId { get; set; }

        public string Template { get; set; }

        public override BaseConfig FormatConfig()
        {
            return this;
        }
    }




}
