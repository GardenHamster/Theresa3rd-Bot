# bot中用到的yml的一些语法
- 为了方便小白使用这个bot项目，这里针对本项目简单说一下配置文件要怎么改
- 详细的yml教程可以另行百度，或者看一下 [菜鸟教程](https://www.runoob.com/w3cnote/yaml-intro.html)
- 加入你运行以后出现这样`YamlDotNet.Core.YamlException`的错误，说明你的botsetting.yml文件改错了，需要仔细检查一下
```bash
Unhandled exception. YamlDotNet.Core.YamlException: (Line: 18, Col: 30, Idx: 1361) - (Line: 18, Col: 49, Idx: 1380): Exception during deserialization
 ---> System.FormatException: Input string was not in a correct format.
   at System.Number.ThrowOverflowOrFormatException(ParsingStatus status, TypeCode type)
   at YamlDotNet.Serialization.NodeDeserializers.ScalarNodeDeserializer.DeserializeIntegerHelper(TypeCode typeCode, String value)
   at YamlDotNet.Serialization.NodeDeserializers.ScalarNodeDeserializer.YamlDotNet.Serialization.INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func`3 nestedObjectDeserializer, Object& value)
   at YamlDotNet.Serialization.ValueDeserializers.NodeValueDeserializer.DeserializeValue(IParser parser, Type expectedType, SerializerState state, IValueDeserializer nestedObjectDeserializer)
   --- End of inner exception stack trace ---
   at YamlDotNet.Serialization.ValueDeserializers.NodeValueDeserializer.DeserializeValue(IParser parser, Type expectedType, SerializerState state, IValueDeserializer nestedObjectDeserializer)
   at YamlDotNet.Serialization.ValueDeserializers.AliasValueDeserializer.DeserializeValue(IParser parser, Type expectedType, SerializerState state, IValueDeserializer nestedObjectDeserializer)
   at YamlDotNet.Serialization.ValueDeserializers.NodeValueDeserializer.<>c__DisplayClass3_0.<DeserializeValue>b__0(IParser r, Type t)
```

## 文本编辑器
- 首先非常不建议用系统自带的记事本打开yml文件，笔记本没有高亮显示而且容易出现编码上的问题
- 推荐使用 [EverEdit](http://cn.everedit.net/) 或者Nodepad++，(Nodepad++开发者因为反华，官网已经被国内禁止访问了)

## 英符
- 开始修改文件之前，将输入法中的标点符号切换为英符
- yml除了注释和字符串部分以外，其他地方使用中文标点都会报错
- 也可以在输入法里面设置为，在中文模式下使用英文符号
- 中符：![image](https://user-images.githubusercontent.com/89188316/174265117-f9a8526c-02df-434f-a758-d92d779b07d8.png)   英符：![image](https://user-images.githubusercontent.com/89188316/174265192-6448f2de-d2d5-496d-965c-8aa2137f38d0.png)

## 格式
![image](https://user-images.githubusercontent.com/89188316/174249576-e19c9e63-9023-465f-9882-15a252ed3b4e.png)

## 数组
![image](https://user-images.githubusercontent.com/89188316/174250562-06ee8ea0-0166-49eb-9305-70174d347fd0.png)
![image](https://user-images.githubusercontent.com/89188316/174253022-436d06ef-b56e-43f0-a224-cf5b80c50505.png)

## 布尔
![image](https://user-images.githubusercontent.com/89188316/174264053-85c58a87-df08-4c84-982b-070a32fcd29e.png)


## 字符串
![image](https://user-images.githubusercontent.com/89188316/174258141-7bfdc985-4c8c-42ad-96dc-adc27c93ccbe.png)

## 对象列表
![image](https://user-images.githubusercontent.com/89188316/174262589-ab2523b5-1875-46b9-b1af-9af32a110de2.png)
