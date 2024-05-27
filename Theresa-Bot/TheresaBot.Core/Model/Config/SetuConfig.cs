namespace TheresaBot.Core.Model.Config
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
            if (GroupCD < 0) GroupCD = 0;
            if (MemberCD < 0) MemberCD = 0;
            if (MaxDaily < 0) MaxDaily = 0;
            if (RevokeInterval < 0) RevokeInterval = 0;
            if (Pixiv is not null) Pixiv.FormatConfig();
            if (Lolicon is not null) Lolicon.FormatConfig();
            if (Lolisuki is not null) Lolisuki.FormatConfig();
            if (Local is not null) Local.FormatConfig();
            if (PixivUser is not null) PixivUser.FormatConfig();
            return this;
        }

    }
}
