using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Core.Model.Pixiv
{
    public class PixivCollectionParam
    {
        public int PixivId { get; set; }

        public int Level { get; set; }

        public List<string> Tags { get; set; } = new();

        public PixivCollectionParam(int pixivId, int level, List<string> tags)
        {
            PixivId = pixivId;
            Level = level;
            Tags = tags;
        }
    }
}
