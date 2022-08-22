# 使用方法

## 目录:
+ **[通过pixiv搜索色图](Menu.md#pixiv涩图)**
+ **[通过Lolicon搜索色图](Menu.md#Lolicon瑟图)**
+ **[通过Lolisuki搜索色图](Menu.md#Lolicon瑟图)**
+ **[通过saucenao搜索原图](Menu.md#Saucenao搜图)**
+ **[订阅pixiv画师](Menu.md#订阅pixiv画师)**
+ **[订阅pixiv标签](Menu.md#订阅pixiv标签)**
+ **[订阅米游社作者](Menu.md#订阅米游社作者)**
+ **[禁止标签](Menu.md#禁止标签)**
+ **[禁止成员](Menu.md#禁止成员)**

## 通过pixiv搜索色图
### 说明
- 根据配置文件，从 `pixiv` 中搜索一张符合条件的指定色图，下载图片并发送到qq群中，响应速度取决于与pixiv连线的速度

### 环境
- ~~首先需要一个能翻墙的运行环境，本人使用的是[自由鲸](https://www.freewhale.us/auth/register?code=sQAT)(原心阶)，邀请码为sQAT~~
- 从0.4.0版本开始加入了免代理，通过修改SNI的方式访问pixiv，然后通过pixiv.cat代理下载图片，有时候或许使用代理下载图片的速度会更高，可以在botsetting.yml中开启或关闭该功能

### 设置cookie
- pc端登录pixiv后按下F12，在p站中随意搜索一个标签，在网络中找到如下请求，这里以搜索Hololive为例，获取的cookie中必须包含PHPSESSID
![image](https://user-images.githubusercontent.com/89188316/177747559-168c1377-db4a-49f0-869f-78749f80707e.png)

- 然后使用 #pixivcookie [获取到的cookie] 格式私聊发送给机器人，与机器人必须为好友

![image](https://user-images.githubusercontent.com/89188316/177748449-02f59d79-a0bc-4475-80f6-40f0c56e06a6.png)

### 指令
- 发送 `#涩图` 可以根据配置获取随机涩图
- 发送 `#涩图 [自定义标签]` 可以随机搜索一张该标签的涩图
- 发送 `#涩图 [作品id]` 可以根据id获取涩图
- 如果获取到的作品为动图时，会自动转换为gif，可以发送 #涩图+作品id 转换自己喜欢的动图
- 自定义标签允许使用空格和逗号分割不用的标签表示`并且`和`或者`的关系，比如：
- `萝莉 巨乳` 表示同时包含 `萝莉` 和 `巨乳` 这两个标签的涩图
- `白丝,黑丝` 表示只需要存在其中一个标签的涩图
- `萝莉,少女 白丝,黑丝` 表示包含`萝莉`或者`少女`,并且包含`白丝`或者`黑丝` 的涩图
![image](https://user-images.githubusercontent.com/89188316/177752424-78319721-337c-41ff-ac6e-31b64b6a4cb8.png)

## 通过Lolicon搜索色图
### 说明
- 根据配置文件，从 [lolicon api](https://api.lolicon.app) 中获取色图，并发送到qq群中

### 指令
- 发送 `#瑟图` 可以随机获取一张Lolicon图床中的瑟图
- 发送 `#瑟图 [自定义标签]` 可以随机搜索一张该标签的瑟图，多标签可以使用逗号或者空格分割，可以进行多标签搜索
![image](https://user-images.githubusercontent.com/89188316/177755716-0bbbaa13-4b94-4d14-a4d4-a2d93789ff87.png)

## 通过Lolisuki搜索色图
### 说明
- 根据配置文件，从 [lolisuki api]([https://api.lolicon.app](https://lolisuki.cc) 中获取色图，并发送到qq群中

### 指令
- 发送 `#setu` 可以随机获取一张Lolicon图床中的瑟图
- 发送 `#setu [自定义标签]` 可以随机搜索一张该标签的瑟图，多标签可以使用逗号或者空格分割，可以进行多标签搜索
![image](https://user-images.githubusercontent.com/89188316/185850275-c77de5b3-f1be-4008-9ceb-5c3518a305d4.png)

## Saucenao搜图
### 说明
- 通过调用 [saucenao](https://saucenao.com) 尝试搜索原图，如果存在匹配度较高的结果时，尝试获取下载并返回原图以及信息

### Saucenao Cookie
- 在未设置cookie的情况下，Saucenao搜索限制为每个ip每日搜索50次，每30秒搜索3次，在使用频率较高的情况下，建议设置cookie
- pc端打开[https://saucenao.com](https://saucenao.com)，点击右下角的Account然后登录saucenao
- 按下F12，然后在控制台/Console中输入`document.cookie`
![image](https://user-images.githubusercontent.com/89188316/177758500-94720035-c11a-4689-bb91-eca1ac95ce7e.png)

- 然后使用 #saucenaocookie [获取到的cookie] 格式私聊发送给机器人，与机器人必须为好友
![image](https://user-images.githubusercontent.com/89188316/177758915-69de1308-d934-407f-a945-17252124c969.png)

### 指令
- 发送 `#原图` 根据提示进行操作
- 发送 `#原图`+`一张或多张需要搜索图片` 
![image](https://user-images.githubusercontent.com/89188316/177800178-4d6821c6-426c-4e19-8770-67222a3b0339.png)

## 订阅pixiv画师
### 说明
- 使用轮询的方式定时扫描画师的最新作品，并将作品自动推送到qq群，可以在权限中设置是否显示R18或R18图片
![image](https://user-images.githubusercontent.com/89188316/177912501-5166798c-c021-465a-a689-7b0486f04cc3.png)

### 获取画师id
- 在pixiv网页版中点开画师头像后，网页地址中 [https://www.pixiv.net/users/15034125](https://www.pixiv.net/users/15034125) 的 15034125 为画师id

### 指令
- 发送 `#订阅画师` 根据提示分步批量订阅`
- 发送 `#退订画师` 根据提示分步批量退订`
- 发送 `#订阅画师 [画师id] [目标id]` 一次性订阅一个画师
- 发送 `#退订画师 [画师id]` 一次性退订一个画师
- 发送 `#同步画师` 根据提示将pixiv账号中关注的画师批量加入到订阅列表中
![image](https://user-images.githubusercontent.com/89188316/177914681-dc8069e2-accc-4359-9b6e-dfbf8fe7fa31.png)


## 订阅pixiv标签
### 说明
- 使用轮询的方式定时扫描标签的最新作品，并将作品自动推送到qq群，匹配方式为部分一致
![image](https://user-images.githubusercontent.com/89188316/177915391-5356879c-5254-420f-af9f-f660c6e3b15e.png)

### 指令
- 发送 `#订阅标签` 根据提示分步订阅
- 发送 `#退订标签` 根据提示分步退订
- 发送 `#订阅标签 [关键词] [目标id]` 一次性订阅一个标签
- 发送 `#退订标签 [关键词]` 一次性退订一个标签

## 订阅米游社作者
### 说明
- 使用轮询的方式定时扫描米游社作者的最新帖子，并将帖子自动推送到qq群

![image](https://user-images.githubusercontent.com/89188316/174287163-38172cf5-dcd6-454b-82f1-eb7fea980700.png)

### 获取作者id
- 在米游社网页版中点开作者头像后，网页地址中 [https://bbs.mihoyo.com/bh3/accountCenter/postList?id=73565533](https://bbs.mihoyo.com/bh3/accountCenter/postList?id=73565533) 的 73565533 为作者id

### 指令
- 发送 `#订阅版主 [作者id] [目标id]` 一次性订阅一个作者
- 发送 `#退订版主 [作者id]` 一次性退订一个作者
- 发送 `#订阅版`主 根据提示分步订阅
- 发送 `#退订版主` 根据提示分步退订

## 禁止标签
### 说明
- 将一个标签加入到禁止搜索列表中，并且推送中不会再出现包含该标签的作品，可防止群友整活，匹配方式为部分一致，且大小写不敏感
- 当群不存在r18权限时，已默认禁止该群搜索r18和r-18标签

![image](https://user-images.githubusercontent.com/89188316/185852450-b246798b-2a85-4eec-ac01-9f614f79eb50.png)


### 指令
- 发送 `#禁止标签 [关键词]` 禁止一个标签
- 发送 `#解禁标签 [关键词]` 解除一个标签

## 禁止成员
### 说明
- 将一个群友加入黑名单中，忽略其发送的所有指令

![image](https://user-images.githubusercontent.com/89188316/185851078-25151023-1359-405f-af53-c1371b39eb9d.png)

### 指令
- 发送 `#禁止成员 [qq号]` 拉黑一个成员
- 发送 `#解禁成员 [qq号]` 解除一个成员




