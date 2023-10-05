namespace TheresaBot.Main.Model.Config
{
    public record SetuConfig : BaseConfig
    {
        public int GroupCD { get; set; }

        public int MemberCD { get; set; }

        public string DisableTagsMsg { get; set; }

        public string NotFoundMsg { get; set; }

        public string ProcessingMsg { get; set; }

        public long MaxDaily { get; set; }

        public int RevokeInterval { get; set; }

        public bool SendPrivate { get; set; }

        public SetuPixivConfig Pixiv { get; set; }

        public LoliconConfig Lolicon { get; set; }

        public LolisukiConfig Lolisuki { get; set; }

        public LocalSetuConfig Local { get; set; }

        public PixivUserConfig PixivUser { get; set; }

        public override SetuConfig FormatConfig()
        {
            Pixiv.FormatConfig();
            Lolicon.FormatConfig();
            Lolisuki.FormatConfig();
            Local.FormatConfig();
            return this;
        }
    }


}
