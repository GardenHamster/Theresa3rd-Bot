﻿namespace TheresaBot.Core.Model.Pixiv
{
    public class PixivResult<T>
    {
        public bool error { get; set; }
        public string message { get; set; }
        public T body { get; set; }
    }
}