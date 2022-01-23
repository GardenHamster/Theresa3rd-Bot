# 部署文档

## 数据库
- 数据库为mysql，使用code first自动建库建表

## mirai-console-loader
- 请参照[iTXTech/mirai-console-loader](https://github.com/iTXTech/mirai-console-loader)安装mcl
- 为mcl安装并配置[mirai-api-http](https://github.com/project-mirai/mirai-api-http)插件
- 启动mcl，并登录机器人账号

## 部署
- 从 [releases](https://github.com/GardenHamster/Theresa3rd-Bot/releases) 处下载最新版本
- 根据自己的需要修改根目录下的配置文件[botsettings.yml](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/Theresa3rd-Bot/botsettings.yml)
- 修改根目录下的配置文件appsettings.Production.json
```json5
{
  "Mirai": {
    "host": "127.0.0.1",            //mcl主机ip
    "port": "8100",                 //mcl中配置的port
    "authKey": "theresa3rd",        //mcl中配置的verifyKey
    "botQQ": "123456789"            //mcl中登录的QQ号
  },
  "Database": {
    "ConnectionString": "Data Source=127.0.0.1;port=3306;Initial Catalog=theresa_bot;uid=root;pwd=123456;"    //mysql数据库链接
  }
}

```

### windows下部署
- 下载并安装 [ASP.NET Core Runtime 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- 启动powershell并将路径切换到Theresa3rd-Bot.dll所在目录下，

- 运行Theresa3rd-Bot.dll，根据自己的需要修改端口和http或https
```bash
dotnet Theresa3rd-Bot.dll --launch-profile Production --urls http://0.0.0.0:8088
```

正常运行结果如下
```bash
2022-01-23 23:41:16,585 [1] INFO  ConsoleLog - 日志配置完毕...
2022-01-23 23:41:16,759 [1] INFO  ConsoleLog - 读取配置文件...
2022-01-23 23:41:16,760 [1] INFO  ConsoleLog - 初始化数据库...
2022-01-23 23:41:21,154 [1] INFO  ConsoleLog - 数据库初始化完毕...
2022-01-23 23:41:21,402 [1] INFO  ConsoleLog - 网站cookie加载完成...
2022-01-23 23:41:21,541 [1] INFO  ConsoleLog - 订阅任务加载完成...
2022-01-23 23:41:21,977 [1] INFO  ConsoleLog - 定时器[深渊结算提醒]启动完毕...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:8088
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Theresa3rd-Bot
```
