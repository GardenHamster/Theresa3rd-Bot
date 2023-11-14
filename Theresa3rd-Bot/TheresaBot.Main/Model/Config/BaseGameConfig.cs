namespace TheresaBot.Main.Model.Config
{
    public abstract record BaseGameConfig : BaseConfig
    {
        public bool Enable { get; set; }
    }
}
