using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.DTO
{
    public record AddBanTagDto
    {
        public string Keyword { get; set; }

        public TagMatchType TagMatchType { get; set; }

    }
}
