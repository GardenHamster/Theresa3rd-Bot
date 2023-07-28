namespace TheresaBot.Main.Model.Result
{
    public record ModifyResult
    {
        public int CreateCount { get; set; }

        public int UpdateCount { get; set; }

        public int DeleteCount { get; set; }

        public int SkipCount { get; set; }


    }
}
