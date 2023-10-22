using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.DTO
{
    public record AddBanTagDto
    {
        public string Keyword { get; set; }

        public TagMatchType TagMatchType { get; set; }

    }
}
