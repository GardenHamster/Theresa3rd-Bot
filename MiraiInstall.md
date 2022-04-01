# MCL安装

### 下载
- 下载最新版本的[mcl-installer](https://github.com/iTXTech/mcl-installer/releases)


### 安装mcl
- 打开下载好的安装文件，然后一直回车就行，如果是32位系统就在Binary Architecture处输入x32
![image](https://user-images.githubusercontent.com/89188316/161088266-461de87d-0e75-414b-aca3-fa3b7eb97e85.png)

### 运行mcl
- 打开mcl.cmd，等待插件更新完毕后关闭控制台
![image](https://user-images.githubusercontent.com/89188316/161089016-81ea8a98-1f74-4f11-96d8-679c5a613868.png)

### 为mcl安装mirai-api-http插件
- 在mcl安装目录空白处按住shift然后点击右键，在此处打开Powershell窗口，然后执行命令
```shell
./mcl --update-package net.mamoe:mirai-api-http --channel stable-v2 --type plugin
```
- 在mcl安装目录的config/net.mamoe.mirai-api-http路径下创建setting.yml文件，然后将下面的配置复制到里面保存
```yaml
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

### 设置bot账号密码
- 修改mcl安装目录下，config/Console目录中的AutoLogin.yml文件
![image](https://user-images.githubusercontent.com/89188316/161096535-77340b3a-862b-426b-9c26-53c26f92832d.png)

### 验证并登录mcl
- 下载[滑动验证助手](https://github.com/mzdluo123/TxCaptchaHelper/releases)并安装到手机中

- 重新打开mcl.cmd，出现如下弹窗，点击Open with TxCaptchaHelper
![image](https://user-images.githubusercontent.com/89188316/161181330-1e1bf5a6-833d-465a-ab80-419c2cdaaa52.png)

- 将弹出来的验证码填入刚下载的滑动验证助手中，然后下一步，完成滑块验证
- ![image](https://user-images.githubusercontent.com/89188316/161178513-bf58f2a1-3c03-4f5b-aa0e-6c4c79119b96.png)
- ![image](https://user-images.githubusercontent.com/89188316/161178979-0655d719-36f1-46a7-93d2-ba9fbff51306.png)

### 设备授权
- 将URL复制出来私聊发送给bot，然后用手机登录bot后点开这个URL
- ![image](https://user-images.githubusercontent.com/89188316/161181132-b4c76afa-637a-4f27-9b2c-2a4c90226209.png)
- 点QQ扫码验证
- ![image](https://user-images.githubusercontent.com/89188316/161180368-66d0717b-f76e-4e88-bdaa-7f818e07fc33.png)
- 将二维码截图保存后，用登录的bot扫描
- ![image](https://user-images.githubusercontent.com/89188316/161180431-eff57a70-17e4-4c11-9195-805b57d8dd93.png)
- 然后允许登录
- ![image](https://user-images.githubusercontent.com/89188316/161180461-2a273c16-425b-46e6-8f97-52260ff30543.png)

### 最后
- 关闭刚才需要复制url的弹窗，可以看到bot登录成功了
- ![image](https://user-images.githubusercontent.com/89188316/161180912-388225c4-a67c-4722-95bc-0fc8902e6d50.png)








