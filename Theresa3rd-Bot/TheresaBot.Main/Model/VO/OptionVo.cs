namespace TheresaBot.Main.Model.VO
{
    public record OptionVo
    {
        public int Value { get; set; }

        public string Label { get; set; }

        public OptionVo(int value, string label)
        {
            Value = value;
            Label = label;
        }

    }
}
