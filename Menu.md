# 使用方法

## 色图功能 - pixiv
### 环境
- 首先需要一个能翻墙的运行环境，本人使用的是[自由鲸](https://www.freewhale.us/auth/register?code=sQAT)(原心阶)，邀请码为sQAT

### 设置cookie
- 初次使用时需要设置cookie，使用一个平时较少使用的账号登录[pixiv](https://www.pixiv.net)，然后获取cookie，cookie中必须包含PHPSESSID
- pc端登录pixiv后按下F12，在p站中随意搜索一个标签，在网络中找到如下请求，这里以搜索Hololive为例
![image](https://user-images.githubusercontent.com/89188316/153154862-8785396e-414a-4f2d-bba3-f7ca8c34f144.png)
- 使用 #pixivcookie+获取到的cookie 格式私聊发送给机器人
![image](https://user-images.githubusercontent.com/89188316/153157373-047aa094-483f-4051-9833-ca6af15698ff.png)
- cookie需要定期更新，在获取色图失败时可以先尝试更新cookie

### 指令
- 使用 #涩图+作品id 可以根据id获取涩图
- 使用 #涩图+自定义标签 可以随机搜索一张该标签的涩图
- 使用 #涩图 可以根据配置获取随机涩图
- 如果获取到的作品为动图时，会转换为gif，可以使用 #涩图+作品id 转换自己喜欢的动图

### 订阅画师
- 订阅功能可以定时扫描画师的最新作品，并将最近作品推送到qq群中，r18类的作品将会被忽略
![image](https://user-images.githubusercontent.com/89188316/150690153-0d071711-7c6a-4b5e-8a39-e73d146476aa.png)

- 首先使用 #订阅画师+画师id 订阅一个画师，在pixiv网页版中点开画师头像后，网页地址中 https://www.pixiv.net/users/15034125 的 15034125 为画师id

![image](https://user-images.githubusercontent.com/89188316/150689981-504be048-8a9b-481b-827d-a8cb83676a37.png)
- 也可以使用 #退订画师+画师id 退订一个画师
