namespace TheresaBot.Main.Model.Config
{
    public record WelcomeConfig : BasePluginConfig
    {
        public string Template { get; set; }

        public List<WelcomeSpecial> Special { get; set; }

        public override WelcomeConfig FormatConfig()
        {
            return this;
        }
    }

    public record WelcomeSpecial
    {
        public long GroupId { get; set; }

        public string Template { get; set; }
    }




}
