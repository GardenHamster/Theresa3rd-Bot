namespace TheresaBot.Main.Model.Config
{
    public record WelcomeConfig : BasePluginConfig
    {
        public string Template { get; private set; }

        public List<WelcomeSpecial> Special { get; private set; }

        public override WelcomeConfig FormatConfig()
        {
            return this;
        }
    }

    public record WelcomeSpecial
    {
        public long GroupId { get; private set; }

        public string Template { get; private set; }
    }




}
