﻿namespace TheresaBot.Main.Model.VO
{
    public record RunningDataVo
    {
        public int RunningSeconds { get; set; }

        public int HandleTimes { get; set; }

        public int PixivPushTimes { get; set; }

        public int PixivScanTimes { get; set; }

        public int PixivScanError { get; set; }

    }
}
