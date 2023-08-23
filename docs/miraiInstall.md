?> mirai 的相关问题，请自行到 [mirai issues](https://github.com/mamoe/mirai/issues) 或者 [mirai社区](https://mirai.mamoe.net) 中查找解决办法，这里只演示Windows下基本的安装流程，并不帮忙解决相关问题

?> 账号无法登录登录的问题，你可以参考 [无法登录的临时处理方案](https://mirai.mamoe.net/topic/223/%E6%97%A0%E6%B3%95%E7%99%BB%E5%BD%95%E7%9A%84%E4%B8%B4%E6%97%B6%E5%A4%84%E7%90%86%E6%96%B9%E6%A1%88) 自行解决

### 安装JDK17
确保你已经安装好JDK17或者更高的版本，并且已经配置好了[环境变量](https://www.baidu.com/s?wd=jdk%E7%8E%AF%E5%A2%83%E5%8F%98%E9%87%8F%E9%85%8D%E7%BD%AE)，具体安装方法请自行[百度](https://www.baidu.com/s?wd=jdk17%E5%AE%89%E8%A3%85%E5%92%8C%E7%8E%AF%E5%A2%83%E9%85%8D%E7%BD%AE)

![image](/img/mirai/20230214183859.jpg)

### 下载MCL
从[https://github.com/iTXTech/mirai-console-loader/releases](https://github.com/iTXTech/mirai-console-loader/releases) 处下载最新的mcl压缩包，然后解压到某个英文路径下

### 运行mcl
运行解压目录中的`mcl.cmd`，等待加载完毕，然后关闭

![image](/img/mirai/20230214185810.jpg)

### 配置bot账号
修改 `mcl目录/config/Console/AutoLogin.yml`，这里的账号和密码建议填自己的小号

![image](/img/mirai/20230214190430.jpg)

### 安装mirai-api-http插件
在`mcl目录`空白位置处按住`Shift`点击右键，在此处打开Powershell窗口，然后执行命令

如果没有这个选项，需要手动打开PowerShell，然后将路径切换到mcl目录下再执行命令

```shell
./mcl --update-package net.mamoe:mirai-api-http --channel stable-v2 --type plugin
```

![image](/img/mirai/20230214191133.jpg)

![image](/img/mirai/20230214191428.jpg)

### 配置mirai-api-http插件
在`mcl目录/config/net.mamoe.mirai-api-http`路径下，创建`setting.yml`文件，然后将下面的配置复制到里面保存

!> 注意：这一个步骤很重要，有不少人因为缺少这一部分内容，导致插件连接不上MCL
  
``` yaml
## 配置文件中的值，全为默认值
## 启用的 adapter, 内置有 http, ws, reverse-ws, webhook
adapters:
  - http
  - ws

## 是否开启认证流程, 若为 true 则建立连接时需要验证 verifyKey
## 建议公网连接时开启
enableVerify: true
verifyKey: theresa3rd

## 开启一些调式信息
debug: false

## 是否开启单 session 模式, 若为 true，则自动创建 session 绑定 console 中登录的 bot
## 开启后，接口中任何 sessionKey 不需要传递参数
## 若 console 中有多个 bot 登录，则行为未定义
## 确保 console 中只有一个 bot 登陆时启用
singleMode: false

## 历史消息的缓存大小
## 同时，也是 http adapter 的消息队列容量
cacheSize: 4096

## adapter 的单独配置，键名与 adapters 项配置相同
adapterSettings:
  ## 详情看 http adapter 使用说明 配置
  http:
    host: localhost
    port: 8100
    cors: ["*"]
  
  ## 详情看 websocket adapter 使用说明 配置
  ws:
    host: localhost
    port: 8100
    reservedSyncId: -1
```

确保`verifyKey`和`port`两个值与插件中`appsettings.Production.json`的值保持一致

![image](/img/mirai/20230214220550.jpg)

### 验证并登录mcl
重新启动`mcl.cmd`，等待加载完毕后出现如下界面，将下面的网址用鼠标点击选中，然后点击右键复制出来，放到浏览器中打开

![image](/img/mirai/20230214221934.jpg)

浏览器打开页面后，先按下`F12`打开控制台，切换到网络选项卡，接着选中`XHR`，最后拖动滑块完成验证

![image](/img/mirai/20230214224403.jpg)

点击`Preview`选项卡，然后将ticket的值全部复制出来，注意不要复制双引号

![image](/img/mirai/20230214230703.jpg)

鼠标右键将复制出来的ticket值粘贴到刚才的MCL控制台中，回车提交

![image](/img/mirai/20230214231215.jpg)

接下来需要验证码，先输入yes，然后再输入手机接收到的验证码，回车提交，最后看到`Bot login successful` 表示登陆成功了

![image](/img/mirai/20230214231901.jpg)


### 修改配置文件中bot账号
打开`appsettings.Production.json`

将`botQQ`的值，改为刚才MCL控制台中登录的QQ号，然后保存

![image](/img/mirai/20230214233740.jpg)
