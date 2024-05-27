using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.DTO
{
    public record AddDictionaryDto
    {
        public List<string> Words { get; set; }

        public DictionaryType WordType { get; set; }

        public int SubType { get; set; }

    }
}
