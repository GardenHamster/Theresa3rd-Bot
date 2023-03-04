namespace TheresaBot.Main.Model.Config
{
    public class WelcomeConfig : BasePluginConfig
    {
        public string Template { get; private set; }

        public List<WelcomeSpecial> Special { get; private set; }
    }

    public class WelcomeSpecial
    {
        public long GroupId { get; private set; }

        public string Template { get; private set; }
    }




}
