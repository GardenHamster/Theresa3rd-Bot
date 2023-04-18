namespace TheresaBot.Main.Model.Config
{
    public class SetuConfig
    {
        public int GroupCD { get; private set; }

        public int MemberCD { get; private set; }

        public string DisableTagsMsg { get; private set; }

        public string NotFoundMsg { get; private set; }

        public string ProcessingMsg { get; private set; }

        public long MaxDaily { get; private set; }

        public int RevokeInterval { get; private set; }

        public bool SendPrivate { get; private set; }

        public SetuPixivConfig Pixiv { get; private set; }

        public LoliconConfig Lolicon { get; private set; }

        public LolisukiConfig Lolisuki { get; private set; }

        public LocalSetuConfig Local { get; private set; }

        public PixivUserConfig PixivUser { get; private set; }
    }


}
