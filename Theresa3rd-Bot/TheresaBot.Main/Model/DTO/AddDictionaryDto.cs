using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.DTO
{
    public record AddDictionaryDto
    {
        public List<string> Words { get; set; }

        public DictionaryType WordType { get; set; }

        public int SubType { get; set; }

    }
}
