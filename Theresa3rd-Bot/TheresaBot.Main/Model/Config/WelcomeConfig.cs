namespace TheresaBot.Main.Model.Config
{
    public record WelcomeConfig : BasePluginConfig
    {
        public string Template { get; set; }

        public List<WelcomeSpecial> Specials { get; set; } = new();

        public override WelcomeConfig FormatConfig()
        {
            if (Specials is null) Specials = new();
            foreach (var item in Specials) item?.FormatConfig();
            return this;
        }
    }

    public record WelcomeSpecial : BaseConfig
    {
        public List<long> GroupIds { get; set; } = new();

        public string Template { get; set; }

        public override BaseConfig FormatConfig()
        {
            if (GroupIds is null) GroupIds = new();
            return this;
        }
    }




}
