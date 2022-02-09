# 使用方法

## 色图功能 - pixiv
- 首先需要一个能翻墙的运行环境，本人使用的是[自由鲸](https://www.freewhale.us/auth/register?code=sQAT)(原心阶)，邀请码为sQAT
- 初次使用时需要设置cookie，使用一个平时较少使用的账号登录[pixiv](https://www.pixiv.net)，然后获取cookie，cookie中必须包含PHPSESSID
- pc端登录pixiv后按下F12，在p站中随意搜索一个标签，在网络中找到如下请求，这里以搜索Hololive为例



- 使用 #pixivcookie+获取到的cookie 格式私聊发送给机器人
![image](https://user-images.githubusercontent.com/89188316/150688844-1b5b66a7-fba3-4f30-8d61-f91cf7c688f6.png)

- cookie需要定期更新，在获取色图失败时可以先尝试更新cookie
- 使用 #涩图+自定义标签 可以随机搜索一张该标签的涩图
![image](https://user-images.githubusercontent.com/89188316/150689563-06401175-1beb-48f6-934e-4e8457a4138e.png)

- 使用 #涩图 可以根据配置文件随机一张色图
![image](https://user-images.githubusercontent.com/89188316/150689678-071daacc-1c2c-4f22-938d-6aaf2c7d7c7a.png)

### 订阅画师
- 订阅功能可以定时扫描画师的最新作品，并将最近作品推送到qq群中，r18类的作品将会被忽略
![image](https://user-images.githubusercontent.com/89188316/150690153-0d071711-7c6a-4b5e-8a39-e73d146476aa.png)

- 首先使用 #订阅画师+画师id 订阅一个画师，在pixiv网页版中点开画师头像后，网页地址中 https://www.pixiv.net/users/15034125 的 15034125 为画师id

![image](https://user-images.githubusercontent.com/89188316/150689981-504be048-8a9b-481b-827d-a8cb83676a37.png)
- 也可以使用 #退订画师+画师id 退订一个画师
