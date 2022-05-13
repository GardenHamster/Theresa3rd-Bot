# 部署文档

## 环境
- 为了确保程序能正常运行，推荐使用以下环境
- Mysql版本：[8.0.28](https://dev.mysql.com/get/Downloads/MySQLInstaller/mysql-installer-community-8.0.28.0.msi)
- mcl版本：[1.2.2](https://github.com/iTXTech/mirai-console-loader/releases/tag/v1.2.2) 或者 mcl-installer版本：[1.0.4](https://github.com/iTXTech/mcl-installer/releases/tag/v1.0.4)
- JDK版本：11

## 数据库
- 数据库为mysql，需要自行安装([小白教程](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/MysqlInstall.md))
- 程序运行后会自动建库建表，请确保账号拥有创建和修改表权限

## mirai-console-loader
- 参照[mirai-console-loader](https://github.com/iTXTech/mirai-console-loader)安装mcl，然后为mcl安装并配置[mirai-api-http](https://github.com/project-mirai/mirai-api-http)插件，最后config/Console/AutoLogin.yml中配置bot账号密码并启动mcl([小白教程](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/MiraiInstall.md))
```bash
2022-02-13 18:09:37 I/WindowHelperJvm: Mirai is using desktop. Captcha will be thrown by window popup. You can add `mirai.no-desktop` to JVM properties (-Dmirai.no-desktop) to disable it.
2022-02-13 18:09:37 I/main: Auto-login 123456789
2022-02-13 18:09:38 I/Bot.123456789: Loaded account secrets from local cache.
2022-02-13 18:09:38 I/Bot.123456789: Saved account secrets to local cache for fast login.
2022-02-13 18:09:38 I/Bot.123456789: Login successful.
2022-02-13 18:09:39 V/Bot.123456789: Event: BotOnlineEvent(bot=Bot(123456789))
2022-02-13 18:09:39 I/Bot.123456789: Bot login successful.
2022-02-13 18:09:39 I/main: mirai-console started successfully.
```

## 部署
- 从 [releases](https://github.com/GardenHamster/Theresa3rd-Bot/releases) 处下载最新版本，注：各版本之间的botsettings.yml可能会有较大差异，升级版本后请注意对比并修改该文件
- 根据自己的需要修改根目录下的配置文件[botsettings.yml](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/Theresa3rd-Bot/botsettings.yml)，修改完成后需要重新启动
- 修改根目录下的配置文件appsettings.Production.json，~~注意是Production不是Development~~
```json5
{
  "Mirai": {                        //mirai-api-http配置在mcl目录config/net.mamoe.mirai-api-http/setting.yml里面
    "host": "127.0.0.1",            //mcl主机ip
    "port": "8100",                 //mirai-api-http配置的port
    "authKey": "theresa3rd",        //mirai-api-http中配置的verifyKey
    "botQQ": "123456789"            //mcl中登录的QQ号
  },
  "Database": {
    "ConnectionString": "Data Source=127.0.0.1;port=3306;Initial Catalog=theresa_bot;uid=root;pwd=123456;CharSet=utf8mb4;"    //mysql数据库链接，确保能连上数据库以后，然后改成自己的
  }
}

```

### windows下部署
- 下载并安装 [ASP.NET Core Runtime 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)，推荐下载页面中的 [Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-6.0.2-windows-hosting-bundle-installer)
- 启动powershell并将路径切换到Theresa3rd-Bot.dll所在目录下，~~或者在目标文件夹中，按住Shift然后右键，在此处打开Powershell窗口~~

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
- 可以在桌面创建一个powershell.ps1脚本方便一键启动，注意将路径和端口改成自己的
```bash
$host.ui.RawUI.WindowTitle="Theresa3rd-Bot"
cd C:\Theresa3rd-Bot
dotnet Theresa3rd-Bot.dll --launch-profile Production --urls http://0.0.0.0:8088
```

## 一些已知的错误
### 数据库自动建表失败[issues#2](https://github.com/GardenHamster/Theresa3rd-Bot/issues/2)
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
- 检查appsettings.Production.json中数据库链接字符串中是否包含CharSet，完整的链接字符串如下
```bash
Data Source=127.0.0.1;port=3306;Initial Catalog=theresa_bot;uid=root;pwd=123456;CharSet=utf8mb4;
```
- 如果还是不行，推荐更换Mysql数据库版本为8.0.28
