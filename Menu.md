# 使用方法

## 目录:
+ **[通过pixiv搜索色图](Menu.md#pixiv涩图)**
+ **[通过Lolicon搜索色图](Menu.md#Lolicon瑟图)**
+ **[通过saucenao搜索原图](Menu.md#Saucenao搜图)**
+ **[禁止标签](Menu.md#禁止标签)**
+ **[订阅pixiv画师](Menu.md#订阅pixiv画师)**
+ **[订阅pixiv标签](Menu.md#订阅pixiv标签)**
+ **[订阅米游社作者](Menu.md#订阅米游社作者)**

## 通过pixiv搜索色图
### 说明
- 根据配置文件，从 `pixiv` 中搜索一张符合条件的指定色图，下载图片并发送到qq群中，响应速度取决于与pixiv连线的速度

### 环境
- ~~首先需要一个能翻墙的运行环境，本人使用的是[自由鲸](https://www.freewhale.us/auth/register?code=sQAT)(原心阶)，邀请码为sQAT~~
- 从0.4.0版本开始加入了免代理，通过修改SNI的方式访问pixiv，然后通过pixiv.cat代理下载图片，有时候或许使用代理下载图片的速度会更高，可以在botsetting.yml中开启或关闭该功能

### 设置cookie
- 初次使用时需要设置cookie，使用一个平时较少使用的账号登录[pixiv](https://www.pixiv.net)，然后获取cookie，cookie中必须包含PHPSESSID
- pc端登录pixiv后按下F12，在p站中随意搜索一个标签，在网络中找到如下请求，这里以搜索Hololive为例
- 然后使用 #pixivcookie [获取到的cookie] 格式私聊发送给机器人，与机器人必须为好友

![image](https://user-images.githubusercontent.com/89188316/153154862-8785396e-414a-4f2d-bba3-f7ca8c34f144.png)

![image](https://user-images.githubusercontent.com/89188316/153157373-047aa094-483f-4051-9833-ca6af15698ff.png)
- cookie需要定期更新，在获取色图失败时可以先尝试更新cookie

### 指令
- 发送 `#涩图` 可以根据配置获取随机涩图
- 发送 `#涩图 [自定义标签]` 可以随机搜索一张该标签的涩图
- 发送 `#涩图 [作品id]` 可以根据id获取涩图
- 如果获取到的作品为动图时，会自动转换为gif，可以发送 #涩图+作品id 转换自己喜欢的动图
![image](https://user-images.githubusercontent.com/89188316/153163179-cab64f76-8b5b-47b5-a59b-099168d8a995.png)
![image](https://user-images.githubusercontent.com/89188316/153164054-604ad40e-d272-4652-923b-88fd45d911d8.png)
![image](https://user-images.githubusercontent.com/89188316/153159925-d0dff1cd-0e26-4be1-9870-c16d57ea01b5.png)

## 通过Lolicon搜索色图
### 说明
- 根据配置文件，从 [lolicon api](https://api.lolicon.app) 中获取色图，并发送到qq群中

### 指令
- 发送 `#瑟图` 可以根据随机获取一张Lolicon图床中的瑟图
- 发送 `#瑟图 [自定义标签]` 可以随机搜索一张该标签的瑟图，多标签可以使用逗号或者空格分割，可以进行多标签搜索
![image](https://user-images.githubusercontent.com/89188316/153169798-ce49c3be-154c-48fd-9a99-e991430c682a.png)

## Saucenao搜图
### 说明
- 通过调用 [saucenao](https://saucenao.com) 尝试搜索原图，如果存在匹配度较高的结果时，尝试获取下载并返回原图以及信息
![image](https://user-images.githubusercontent.com/89188316/175912401-1d3a6b08-8130-48c8-b864-8c160a8ef065.png)

### 指令
- 发送 `#原图` 根据提示进行操作
- 发送 `#原图`+`一张或多张需要搜索图片` 

## 禁止标签
### 说明
- 将一个标签加入到禁止搜索列表中，防止群友整活，匹配方式为完全一致
![image](https://user-images.githubusercontent.com/89188316/153175892-80e31abe-cbf7-4485-bfb1-bc7370f8c06d.png)

### 指令
- 发送 `#禁止标签 [关键词]` 禁止一个标签
- 发送 `#解禁标签 [关键词]` 解除一个标签

## 订阅pixiv画师
### 说明
- 使用轮询的方式定时扫描画师的最新作品，并将作品自动推送到qq群，可以在权限中设置是否显示R18或R18图片

![image](https://user-images.githubusercontent.com/89188316/153171928-b9e90263-5351-41a4-824f-6a999feca886.png)

### 获取画师id
- 在pixiv网页版中点开画师头像后，网页地址中 [https://www.pixiv.net/users/15034125](https://www.pixiv.net/users/15034125) 的 15034125 为画师id

### 指令
- 发送 `#订阅画师` 根据提示分步批量订阅`
- 发送 `#退订画师` 根据提示分步批量退订`
- 发送 `#订阅画师 [画师id] [目标id]` 一次性订阅一个画师
- 发送 `#退订画师 [画师id]` 一次性退订一个画师
- 发送 `#同步画师` 根据提示将pixiv账号中关注的画师批量加入到订阅列表中

![image](https://user-images.githubusercontent.com/89188316/174473049-5f46b11a-3fda-4298-bebf-747adda9a5d5.png)
![image](https://user-images.githubusercontent.com/89188316/174473145-97e16062-17a1-443f-9ec0-92b8bcb0003e.png)


## 订阅pixiv标签
### 说明
- 使用轮询的方式定时扫描标签的最新作品，并将作品自动推送到qq群，匹配方式为部分一致

![image](https://user-images.githubusercontent.com/89188316/153169722-389c2058-a54f-46e6-9004-c9073498f0b9.png)

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






