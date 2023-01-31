# 部署文档

## 安装数据库
- 数据库为mysql，需要自行安装([小白教程](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/MysqlInstall.md))，或者可以购买云数据库

## 安装mirai-console-loader
- 参照 [mirai-console-loader](https://github.com/iTXTech/mirai-console-loader) 文档安装mcl，并为mcl安装并配置 [mirai-api-http](https://github.com/project-mirai/mirai-api-http) 插件，([小白教程](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/MiraiInstall.md))
- 最后在config/Console/AutoLogin.yml中配置bot账号密码并启动mcl，正常启动结果如下
```bash
2022-02-13 18:09:37 I/main: Auto-login 123456789
2022-02-13 18:09:38 I/Bot.123456789: Loaded account secrets from local cache.
2022-02-13 18:09:38 I/Bot.123456789: Saved account secrets to local cache for fast login.
2022-02-13 18:09:38 I/Bot.123456789: Login successful.
2022-02-13 18:09:39 V/Bot.123456789: Event: BotOnlineEvent(bot=Bot(123456789))
2022-02-13 18:09:39 I/Bot.123456789: Bot login successful.
2022-02-13 18:09:39 I/main: mirai-console started successfully.
```

## 下载并修改配置文件
- [点击这里下载最新版本](https://github.com/GardenHamster/Theresa3rd-Bot/releases)，注：各版本之间的`botsettings.yml`可能会有较大差异，升级版本后请注意对比并修改该文件
- 根据自己的需要修改根目录下的配置文件`botsettings.yml`，修改完成后需要重新启动，**注：参数值为空时需要用一对单引号代替，不能直接删掉什么也不加，不然会报错**
- [botsetting.yml的一些补充说明](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/botsetting.md)，实在没接触过yml语法的小白可以看一下[这里](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/ymlconfig.md)
- 修改根目录下的配置文件appsettings.Production.json，使项目可以连接上mcl
```json5
{
  "Mirai": {                        //mirai-api-http配置在mcl目录config/net.mamoe.mirai-api-http/setting.yml里面
    "host": "127.0.0.1",            //mcl主机ip
    "port": "8100",                 //mirai-api-http配置的port
    "authKey": "theresa3rd",        //mirai-api-http中配置的verifyKey
    "botQQ": "123456789"            //mcl中登录的QQ号
  },
  "Database": {
    "ConnectionString": "Data Source=127.0.0.1;port=3306;Initial Catalog=theresa_bot;uid=root;pwd=123456;CharSet=utf8mb4;SslMode=None;"    //mysql数据库链接，确保能连上数据库以后，然后改成自己的
  }
}

```

## Linux下部署
1、签名密钥并添加 Microsoft 包存储库
```bash
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
```
2、安装ASP.NET Core 6.0 运行时
```bash
sudo yum install aspnetcore-runtime-6.0
```
3、安装libgdiplus
```bash
yum install epel-release
```
```bash
sudo yum install libgdiplus
```
```bash
sudo ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
sudo ln -s /usr/lib64/libgdiplus.so /usr/lib64/gdiplus.dll
```
4、切换到Theresa3rd-Bot.dll所在目录下，运行Theresa3rd-Bot.dll，这里的端口可以随意
```bash
nohup dotnet Theresa3rd-Bot.dll --launch-profile Production --urls http://0.0.0.0:8088
```
5、如果使用#瑟图命令时报错：The remote certificate is invalid because of errors in the certificate chain: NotTimeValid，需要升级一下ca证书
```bash
yum update ca-certificates -y
```

## Windows下部署
- 下载并安装 [ASP.NET Core Runtime 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)，推荐下载页面中的 [Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-6.0.8-windows-hosting-bundle-installer)
- 启动powershell并将路径切换到Theresa3rd-Bot.dll所在目录下，~~或者在目标文件夹中，按住Shift然后右键，在此处打开Powershell窗口~~

- 运行Theresa3rd-Bot.dll，这里的端口可以随意
```bash
dotnet Theresa3rd-Bot.dll --launch-profile Production --urls http://0.0.0.0:8088
```

- 可以在桌面创建一个powershell.ps1脚本方便一键启动
```bash
$host.ui.RawUI.WindowTitle="Theresa3rd-Bot"
cd C:\Theresa3rd-Bot
dotnet Theresa3rd-Bot.dll --launch-profile Production --urls http://0.0.0.0:8088
```

## 正常运行结果如下
```bash
2022-08-28 03:43:52,310 [1] INFO  ConsoleLog - 日志配置完毕...
2022-08-28 03:43:52,441 [1] INFO  ConsoleLog - 配置文件读取完毕...
2022-08-28 03:43:52,442 [1] INFO  ConsoleLog - 开始初始化数据库...
2022-08-28 03:43:53,347 [1] INFO  ConsoleLog - 数据库初始化完毕...
2022-08-28 03:43:53,393 [1] INFO  ConsoleLog - 网站cookie加载完成...
2022-08-28 03:43:53,411 [1] INFO  ConsoleLog - 订阅任务加载完成...
2022-08-28 03:43:53,418 [1] INFO  ConsoleLog - 加载禁止标签完毕
2022-08-28 03:43:53,420 [1] INFO  ConsoleLog - 加载黑名单完毕
2022-08-28 03:43:53,720 [1] INFO  ConsoleLog - 定时器[深渊结算提醒]启动完毕...
2022-08-28 03:43:53,721 [1] INFO  ConsoleLog - pixiv用户订阅任务启动完毕...
2022-08-28 03:43:53,722 [1] INFO  ConsoleLog - pixiv标签订阅任务启动完毕...
2022-08-28 03:43:53,722 [1] INFO  ConsoleLog - 米游社订阅任务启动完毕...
2022-08-28 03:43:53,725 [1] INFO  ConsoleLog - 清理定时器启动完毕...
2022-08-28 03:43:53,727 [1] INFO  ConsoleLog - Cookie检查定时器启动完毕...
2022-08-28 03:43:53,728 [1] INFO  ConsoleLog - Theresa3rd-Bot启动完毕，版本：v0.7.1
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: D:\Theresa3rd-Bot
2022-08-28 03:43:53,882 [8] INFO  ConsoleLog - 已成功连接到mirai-console...
```

## 更新版本的步骤
- 关掉正在运行的powershell脚本
- 替换掉除了以下几个以外的文件
```bash
botsettings.yml
appsettings.json
appsettings.Development.json
appsettings.Production.json
```
- 检查一下botsettings.yml是否有更新，有则对比修改该文件，如果遇到修改比较多的情况可以对照旧文件重新修改一次
- 重新启动脚本

## pixiv图片反向代理
- 如果在qq中打开原图连接时出现感叹号，或者打不开原图链接时，[可以参考这里配置一个自己的反向代理域名](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/pixivproxy.md)，然后修改相关配置

## 一些已知的错误
### 数据库自动建表失败
```bash
SqlSugar.SqlSugarException: 中文提示 :  连接数据库过程中发生错误，检查服务器是否正常连接字符串是否正确，实在找不到原因请先Google错误信息：The given key '0' was not present in the dictionary..
English Message : Connection open error . The given key '0' was not present in the dictionary.
   at SqlSugar.AdoProvider.GetDataReader(String sql, SugarParameter[] parameters)
   at SqlSugar.AdoProvider.SqlQuery[T,T2,T3,T4,T5,T6,T7](String sql, Object parameters)
   at SqlSugar.AdoProvider.SqlQuery[T](String sql, SugarParameter[] parameters)
   at SqlSugar.AdoProvider.SqlQuery[T](String sql, Object parameters)
   at SqlSugar.DbMaintenanceProvider.GetDataBaseList(SqlSugarClient db)
   at SqlSugar.MySqlDbMaintenance.CreateDatabase(String databaseName, String databaseDirectory)
   at SqlSugar.DbMaintenanceProvider.CreateDatabase(String databaseDirectory)
   at Theresa3rd_Bot.Dao.DBClient.CreateDB() in D:\project\Theresa3rd-Bot\Theresa3rd-Bot\Dao\DBClient.cs:line 17
```
```bash
System.Collections.Generic.KeyNotFoundException: The given key '25185' was not present in the dictionary.
   at SqlSugar.AdoProvider.GetDataReader(String sql, SugarParameter[] parameters)
   at SqlSugar.AdoProvider.SqlQuery[T,T2,T3,T4,T5,T6,T7](String sql, Object parameters)
   at SqlSugar.AdoProvider.SqlQuery[T](String sql, SugarParameter[] parameters)
   at SqlSugar.AdoProvider.SqlQuery[T](String sql, Object parameters)
   at SqlSugar.DbMaintenanceProvider.GetDataBaseList(SqlSugarClient db)
   at SqlSugar.MySqlDbMaintenance.CreateDatabase(String databaseName, String databaseDirectory)
   at SqlSugar.DbMaintenanceProvider.CreateDatabase(String databaseDirectory)
   at Theresa3rd_Bot.Dao.DBClient.CreateDB() in D:\project\Theresa3rd-Bot\Theresa3rd-Bot\Dao\DBClient.cs:line 17
```
- 检查appsettings.Production.json中数据库链接字符串完整，完整的链接字符串如下
```bash
Data Source=127.0.0.1;port=3306;Initial Catalog=theresa_bot;uid=root;pwd=123456;CharSet=utf8mb4;SslMode=None;
```

### yml语法错误
- 遇到类似 `YamlDotNet.Core.YamlException` 情况需要重新检查 `botsetting.yml` 文件
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

### linux下报错The remote certificate is invalid because of errors in the certificate chain: NotTimeValid
- 更新一下ca证书
```bash
yum update ca-certificates -y
```

### bot没有回复或者只回复表情
**只有#pixivcookie等更新cookie的指令需要私发给机器人以外，其他指令都要发送到群里面**

**有可能是你发送的不是一个指令，或者这个指令不存在，或者发送的指令前缀不一致，或者你修改了配置文件以后没有重启**

私聊bot但是只会回复表情的情况：
- 需要加bot为好友

群聊bot但是没有回应的情况
- 配置文件中AcceptGroups没有配置群号，或者群号填写错误
- mcl中机器人有回应但是群里没有消息出来，那可能是你的bot账号是刚注册没多久的，或者内容太色了mht不让你发
