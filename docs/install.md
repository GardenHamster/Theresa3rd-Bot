!> 注：本插件在依赖于机器人支持库下运行，所以你必须先运行 [Mirai](https://github.com/mamoe/mirai) 或 [GoCQHttp](https://github.com/Mrs4s/go-cqhttp)，然后再同时运行本插件

?> 修改配置文件时，推荐你使用 [vscode](https://code.visualstudio.com) 或者 [nodepad++](https://github.com/notepad-plus-plus/notepad-plus-plus/releases) 等编辑器代替系统默认记事本，避免出现编码错误或者符号错漏等一系列问题

?> 不推荐你将文件解压到非英文目录和路径下，避免出现不必要的错误和问题

## 安装数据库
数据库为Mysql，需要自行安装，推荐安装 [v8.0.31](https://dev.mysql.com/get/Downloads/MySQLInstaller/mysql-installer-community-8.0.31.0.msi) 及以上，[点击查看新手教程](mysqlInstall.md)

## 安装机器人库
在下面两个机器人库中选择一个安装，你可以两个都安装，但是同一时间只能运行其中一个，**推荐安装Mirai**

- **安装Mirai(二选一)：**参照 [mirai-console-loader](https://github.com/iTXTech/mirai-console-loader) 文档进行安装，然后参考 [mirai-api-http](https://github.com/project-mirai/mirai-api-http) 文档配置`mirai-api-http`插件，然后配置bot账号密码，你可以 [点击这里查看新手教程](miraiInstall.md)

- **安装GoCQ(二选一)：**参照 [go-cqhttp 帮助中心](https://docs.go-cqhttp.org/guide/quick_start.html#%E5%9F%BA%E7%A1%80%E6%95%99%E7%A8%8B) 进行安装，并配置bot账号密码。如果你需要使用私聊相关功能，记得把配置 `allow-temp-session` 设置为true


## 运行机器人库

Mirai正常启动结果如下

```bash
2023-08-05 22:54:25 I/UnidbgFetchQsign: Bot(123456789) initialize complete
2023-08-05 22:54:25 I/Bot.123456789: Saved account secrets to local cache for fast login.
2023-08-05 22:54:26 I/Bot.123456789: Login successful.
2023-08-05 22:54:26 V/Bot.123456789: Event: BotOnlineEvent(bot=Bot(123456789))
2023-08-05 22:54:26 I/Bot.123456789: Bot login successful.
2023-08-05 22:54:26 V/Bot.123456789: Event: net.mamoe.mirai.console.events.AutoLoginEvent$Success@738a39cc
2023-08-05 22:54:26 I/main: mirai-console started successfully.
```

GoCQ正常启动结果如下

```bash
[2023-08-05 22:52:10] [INFO]: 登录成功 欢迎使用: 学园长
[2023-08-05 22:52:10] [INFO]: 开始加载好友列表...
[2023-08-05 22:52:10] [INFO]: 共加载 3 个好友.
[2023-08-05 22:52:10] [INFO]: 开始加载群列表...
[2023-08-05 22:52:10] [INFO]: 收到服务器地址更新通知, 将在下一次重连时应用.
[2023-08-05 22:52:11] [INFO]: 共加载 3 个群.
[2023-08-05 22:52:11] [INFO]: 资源初始化完成, 开始处理信息.
[2023-08-05 22:52:11] [INFO]: アトリは、高性能ですから!
[2023-08-05 22:52:11] [INFO]: CQ WebSocket 服务器已启动: [::]:8100
[2023-08-05 22:52:11] [INFO]: 正在检查更新.
[2023-08-05 22:52:11] [INFO]: 检查更新完成. 当前已运行最新版本.
[2023-08-05 22:52:11] [INFO]: 开始诊断网络情况
[2023-08-05 22:52:13] [INFO]: 网络诊断完成. 未发现问题
```

## 部署签名服务(可选)

如果你在登录bot账号的过程中出现了 `code=45` 等无法登录的问题，可以考虑部署签名服务，但是由于使用签名服务可能会存在账号被 `冻结/封号`的风险，请在考虑清楚后再决定是否使用，**你在使用该服务过程中出现的任何风险和问题都和本人无关，**

你可以参考 [unidbg-fetch-qsign/wiki](https://github.com/fuqiuluo/unidbg-fetch-qsign/wiki) 自行搭建签名服务

Mirai库 参考 [fix-protocol-version](https://github.com/cssxsh/fix-protocol-version#mirai-console-%E4%BD%BF%E7%94%A8%E6%96%B9%E6%B3%95) 文档接入签名服务

Gocq库 参考 [签名服务器相关问题](https://github.com/Mrs4s/go-cqhttp/discussions/2245) 接入签名服务

## 下载插件
从 [Releases](https://github.com/GardenHamster/Theresa3rd-Bot/releases) 中下载最新版本的`Theresa3rd-Bot.zip`压缩包，然后解压到某个英文 目录/路径 下，并不需要放到mcl目录下

## 连接机器人库
修改根目录下的配置文件`appsettings.Production.json`，使本插件可以连接上相应的机器人库
```json
{
  "Mirai": {                        //Mirai相关配置
    "host": "127.0.0.1",            //mcl主机ip
    "port": "8100",                 //mirai-api-http中配置的port
    "authKey": "theresa3rd",        //mirai-api-http中配置的verifyKey
    "botQQ": "123456789"            //mcl中登录的QQ号
  },
  "GoCqHttp": {                     //GoCq相关配置
  "host": "127.0.0.1",              //GoCq主机ip
  "port": "8100"                    //GoCq中配置的port
  },
  "Database": {                     //Mysql数据库链接，确保能连上数据库以后，然后改成自己的
    "ConnectionString": "Data Source=127.0.0.1;port=3306;Initial Catalog=theresa_bot;uid=root;pwd=123456;CharSet=utf8mb4;SslMode=None;"    
  }
}
```

## Linux下部署
1. 安装 ASP.NET Core 8.0 运行环境

?> 这里以 CentOS7 为例，其他Linux版本请参考 [微软官方文档](https://learn.microsoft.com/zh-cn/dotnet/core/install/linux)

```bash
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
```

```bash
sudo yum install aspnetcore-runtime-8.0
```

2. 升级CA证书

```bash
yum update ca-certificates -y
```

3. 切换到`TheresaBot.MiraiHttpApi.dll`所在目录下

```bash
cd TheresaBot.MiraiHttpApi.dll所在目录
```

4. 后台运行dll，这里的端口为前端页面端口，可以随意填，但是不要使用 mirai-http-api 或 go-cqhttp 的端口

- 使用Mirai

```bash
nohup dotnet TheresaBot.MiraiHttpApi.dll --launch-profile Production --urls http://0.0.0.0:8088
```

- 使用GoCQ

```bash
nohup dotnet TheresaBot.GoCqHttp.dll --launch-profile Production --urls http://0.0.0.0:8088
```


## Windows下部署
1. 下载并安装 [ASP.NET Core Runtime 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)，推荐下载页面中的 [Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.0-windows-hosting-bundle-installer)

2. 启动 powershell 并将路径切换到`TheresaBot.MiraiHttpApi.dll`所在目录下

3. 运行dll，这里的端口为前端页面端口，可以随意填，但是不要使用 mirai-http-api 或 go-cqhttp 的端口

- 使用Mirai

```powershell
dotnet TheresaBot.MiraiHttpApi.dll --launch-profile Production --urls http://0.0.0.0:8088
```

- 使用GoCQ

```powershell
dotnet TheresaBot.GoCqHttp.dll --launch-profile Production --urls http://0.0.0.0:8088
```

4. 可以在桌面创建一个 powershell 脚本`Theresa3rdBot.ps1`方便一键启动

- 使用Mirai

```powershell
$host.ui.RawUI.WindowTitle="Theresa-Bot-Mirai-8088"
cd C:\Theresa3rd-Bot            #这里修改为你的解压目录。如果把脚本放在解压目录下，这行可以不写
dotnet TheresaBot.MiraiHttpApi.dll --launch-profile Production --urls http://0.0.0.0:8088
pause
```

- 使用GoCQ

```powershell
$host.ui.RawUI.WindowTitle="Theresa-Bot-GoCQHttp-8088"
cd C:\Theresa3rd-Bot            #这里修改为你的解压目录。如果把脚本放在解压目录下，这行可以不写
dotnet TheresaBot.GoCqHttp.dll --launch-profile Production --urls http://0.0.0.0:8088
pause
```

### 正常运行结果如下
```powershell
2023-12-06 11:04:18,898 INFO  - 日志配置完毕...
2023-12-06 11:04:19,149 INFO  - 配置文件加载完毕...
2023-12-06 11:04:19,239 INFO  - 后台初始化完毕...
2023-12-06 11:04:19,249 INFO  - 尝试连接到mirai-console...
2023-12-06 11:04:19,555 INFO  - 已成功连接到mirai-console...
2023-12-06 11:04:19,708 INFO  - Bot名片获取完毕，QQNumber=123456789，Nickname=略略略
2023-12-06 11:04:19,733 INFO  - 群列表加载完毕，共获取群号 3 个，其中已启用群号 3 个
2023-12-06 11:04:19,735 INFO  - 开始初始化数据库...
2023-12-06 11:04:25,309 INFO  - 数据库初始化完毕...
2023-12-06 11:04:25,604 INFO  - 网站cookie加载完成...
2023-12-06 11:04:25,712 INFO  - 订阅任务加载完成...
2023-12-06 11:04:25,780 INFO  - 加载屏蔽标签列表完毕...
2023-12-06 11:04:25,844 INFO  - 加载屏蔽用户列表完毕...
2023-12-06 11:04:25,911 INFO  - pixiv用户订阅任务启动完毕...
2023-12-06 11:04:25,912 INFO  - pixiv标签订阅任务启动完毕...
2023-12-06 11:04:25,912 INFO  - 米游社订阅任务启动完毕...
2023-12-06 11:04:26,034 INFO  - 定时清理任务启动完毕...
2023-12-06 11:04:26,037 INFO  - Cookie检查定时器启动完毕...
2023-12-06 11:04:26,044 INFO  - 定时提醒任务[深渊结算提醒]启动完毕...
2023-12-06 11:04:26,050 INFO  - 定时涩图任务[下午茶]启动完毕...
2023-12-06 11:04:26,053 INFO  - 定时日榜推送任务[Daily,DailyAI]启动完毕...
2023-12-06 11:04:26,057 INFO  - 词云推送任务[早安词云]启动完毕...
2023-12-06 11:04:26,061 INFO  - Theresa3rd-Bot启动完毕，版本：v0.11.0
----------------------------------------------------------------------------------------
后台密码：123456
你可以在配置文件【Config/Backstage.yml】中修改后台密码(Password)
----------------------------------------------------------------------------------------
访问下列地址配置Bot相关功能：
http://127.0.0.1:8088
你也可以通过公网Ip:端口的方式访问后台
----------------------------------------------------------------------------------------
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:8088
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Theresa3rd-Bot
```

## 配置Bot
访问插件启动后列出的后台地址，比如上面列出的地址：http://127.0.0.1:8088，根据自己的需要修改配置，配置保存后将会立即生效

你也可以手动修改[插件目录/Config]目录下的配置文件，但是这种方式修改后需要手动重启插件

## 开启VPN
Pixiv需要一个可以访问外网的环境，你需要自行购买机场。

建议`Clash`等代理客户端选择`Rule`代理模式，节点选择`JP`节点。

如果你的主机/服务器不在大陆范围内，可以不使用VPN，你也可以使用香港云服务器。

![image](/img/install/2023-05-08-02-31-47-624.jpg)

从v0.4.0版本开始加入了免代理，通过修改SNI的方式访问pixiv，然后通过pixiv.re代理下载图片。

你可以在配置中开启该功能，但不建议在有梯子的情况下启用它。

最后在你运行这个插件的机器上登录[https://www.pixiv.net](https://www.pixiv.net)，确保机器可以正常访问Pixiv

## 配置 cookie
- 如果想要正常使用pixiv相关功能，你需要为插件配置PixivCookie

- 参考 [cookie指令](cookie.md?id=pixiv-cookie) 配置pixiv cookie (主要)

- 参考 [cookie指令](cookie.md?id=saucenao-cookie) 配置saucenao cookie (次要)

## 更新版本的步骤

1. 关掉正在运行的powershell脚本

2. 替换掉除了以下以外的目录/文件
* `Config`
* `appsettings.json`
* `appsettings.Production.json`

3. 重启插件

## pixiv图片代理
* 配置文件中的默认代理`https://i.pixiv.re`被tx标记为危险链接，发送这类红链容易导致Bot被冻结/封号

* 推荐你修改配置文件`Config/Pixiv.yml`中的`OriginUrlProxy`到其他代理地址降低这类风险

* [推荐参考这里配置一个自己的图片代理](imgProxy.md)

![image](/img/install/2023-03-02-18-58-03-758.jpg)