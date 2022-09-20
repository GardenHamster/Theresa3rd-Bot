namespace Theresa3rd_Bot.Model.Config
{
    public class MysUserSubscribeConfig : BaseSubscribeConfig
    {
        public int ShelfLife { get; set; }

        public MysUserSubscribeConfig()
        {
            this.ShelfLife = 12 * 60 * 60;
            this.ScanInterval = 60;
        }

    }
}
