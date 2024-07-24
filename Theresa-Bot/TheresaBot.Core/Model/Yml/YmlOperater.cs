using System.Text;
using TheresaBot.Core.Model.Config;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace TheresaBot.Core.Model.Yml
{
    public class YmlOperater<T> where T : BaseConfig
    {
        public string YmlPath { get; init; }

        public YmlOperater(string ymlPath)
        {
            this.YmlPath = ymlPath;
        }

        public T LoadConfig()
        {
            if (File.Exists(YmlPath) == false) return null;
            using FileStream fileStream = new FileStream(YmlPath, FileMode.Open, FileAccess.Read);
            using TextReader reader = new StreamReader(fileStream, Encoding.UTF8);
            Deserializer deserializer = new Deserializer();
            var config = deserializer.Deserialize<T>(reader);
            config.FormatConfig();
            return config;
        }

        public void SaveConfig(T data)
        {
            data.FormatConfig();
            var enumConverter = new EnumConverter();
            var serializer = new SerializerBuilder().WithTypeConverter(enumConverter).Build();
            var yamlContent = serializer.Serialize(data);
            using StreamWriter stream = new StreamWriter(YmlPath, false, Encoding.UTF8);
            stream.Write(yamlContent);
            stream.Flush();
            stream.Close();
        }
    }

    internal class EnumConverter : IYamlTypeConverter
    {
        public bool Accepts(System.Type type)
        {
            return type.IsEnum;
        }

        public object ReadYaml(IParser parser, System.Type type)
        {
            int enumValue = 0;
            var value = parser.Consume<Scalar>().Value;
            if (int.TryParse(value, out enumValue))
            {
                return enumValue;
            }
            return Enum.Parse(type, value);
        }

        public void WriteYaml(IEmitter emitter, object value, System.Type type)
        {
            var enumValue = (int)value;
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, enumValue.ToString(), ScalarStyle.Any, true, false));
        }
    }


}
