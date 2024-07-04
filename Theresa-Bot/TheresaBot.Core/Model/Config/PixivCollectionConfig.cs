namespace TheresaBot.Core.Model.Config
{
    public record PixivCollectionConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; } = new();

        public bool PixivCollect { get; set; }

        public bool LocalCollect { get; set; }

        public string LocalSavePath { get; set; }

        public bool OSSCollect { get; set; }

        public string OSSEndpoint { get; set; }

        public string OSSAccessKeyId { get; set; }

        public string OSSAccessKeySecret { get; set; }

        public string OSSBucketName { get; set; }

        public override PixivCollectionConfig FormatConfig()
        {
            if (Commands is null) Commands = new();
            return this;
        }

    }
}
