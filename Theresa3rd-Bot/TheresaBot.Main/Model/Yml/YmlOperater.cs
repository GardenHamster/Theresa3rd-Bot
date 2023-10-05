using System.Text;
using TheresaBot.Main.Model.Config;
using YamlDotNet.Serialization;

namespace TheresaBot.Main.Model.Yml
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
            using TextReader reader = new StreamReader(fileStream, Encoding.GetEncoding("gb2312"));
            Deserializer deserializer = new Deserializer();
            return deserializer.Deserialize<T>(reader);
        }

        public void SaveConfig(T data)
        {
            var serializer = new SerializerBuilder().Build();
            var yamlContent = serializer.Serialize(data);
            using StreamWriter stream = new StreamWriter(YmlPath, false, Encoding.GetEncoding("gb2312"));
            stream.Write(yamlContent);
            stream.Flush();
            stream.Close();
        }


    }
}
