using TheresaBot.Core.Helper;

namespace TheresaBot.Core.Model.VO
{
    public record OptionVo
    {
        public int Value { get; set; }

        public string Label { get; set; }

        public List<OptionVo> SubOptions { get; set; } = new();

        public OptionVo(int value, string label)
        {
            Value = value;
            Label = label;
        }

        public void AddSubOptions(Dictionary<int, string> options)
        {
            SubOptions.AddRange(options.ToOptionList());
        }




    }
}
