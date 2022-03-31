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
- 下载[MiraiAndroid](https://install.appcenter.ms/users/mzdluo123/apps/miraiandroid/distribution_groups/release)并安装到手机中

- 重新打开mcl.cmd，出现如下弹窗，点击Open with TxCaptchaHelper
![image](https://user-images.githubusercontent.com/89188316/161101769-35f7e9d1-202e-4c06-b05d-a7f6caaa8e9f.png)








