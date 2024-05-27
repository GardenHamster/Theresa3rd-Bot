using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.Base
{
    public record BaseSourceItem
    {
        public SetuSourceType SourceType { get; set; }

        public string SourceUrl { get; set; }

        public string SourceId { get; set; }
    }
}
