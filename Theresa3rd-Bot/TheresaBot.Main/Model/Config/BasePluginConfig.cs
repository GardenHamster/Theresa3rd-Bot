namespace TheresaBot.Main.Model.Config
{
    public abstract record BasePluginConfig : BaseConfig
    {
        public bool Enable { get; set; }
    }
}
