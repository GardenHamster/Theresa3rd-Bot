namespace TheresaBot.Main.Model.Base
{
    public abstract class BaseModel<T> where T : Enum
    {
        public T Type { get; set; }
        public string Code { get; init; }
        public string Name { get; init; }

        public BaseModel(T type, string code, string name)
        {
            Type = type;
            Code = code;
            Name = name;
        }

    }

}
