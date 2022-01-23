# Theresa3rd-Bot

## 简介
 - 某舰团长利用摸鱼时间写出来的bot，准备移植到mirai。基于 .net core 6.0 和 mirai-api-http
 - 相关功能还在摸鱼开发中，部署方法请参考 [Document.md](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/Document.md)  

## 声明
 - 本项目为个人自娱自乐开发出来的项目，并没有任何收益，并不负责项目所产生的任何问题，不得将项目用于任何商业用途

## 功能
- [x] 入群欢迎
- [x] 普通版复读机
- [x] 定时提醒功能(提醒深渊结算战场开放等...)
- [x] 色图 - pixiv色图搜索功能(根据标签获取随机的收藏度较高的色图)
- [x] 色图 - pixiv画师订阅功能(自动推送关注画师的最新作品)
- [ ] 色图 - pixiv标签订阅功能(自动推送关注标签的收藏度较高的最新作品)
- [ ] 色图 - 以图搜图功能(saucenao)
- [ ] 色图 - 定时随机发送本地涩图功能（没卵用）
- [ ] B站 - 订阅功能(自动推送关注up的最新动态)
- [ ] B站 - 直播推送功能(up开始直播时在群里发送开播提醒)
- [ ] 米游社 - 订阅功能(自动推送关注用户的最新贴子)
- [ ] 米游社 - 崩坏3/原神/米游币自动点到功能(以后再说)
- [ ] 原神模拟抽卡(基于[GardenHamster/GenshinPray](https://github.com/GardenHamster/GenshinPray))
- [ ] 崩坏3 - 模拟抽卡,收种包菜获取水晶,自动获取卡池信息等功能
- [ ] 崩坏3 - 查图鉴以及猜装备功能
- [ ] 小游戏 - 群狼人杀(以后再说)
- [ ] 群管 - 关键词自动回复/禁言/撤回功能
- [ ] 群娱 - 每日/周/月词云功能
- [ ] 群娱 - 漂流瓶功能(跨群聊天,以后再说)

## 其他
- 有想法或问题请发issues，安卓官服的舰长可以加个舰团1069989

## 致谢
- 感谢[mamoe/mirai](https://github.com/mamoe/mirai)提供的支持
- 感谢[Executor-Cheng/mirai-CSharp](https://github.com/Executor-Cheng/mirai-CSharp)提供的框架

## 使用方法

### 随机色图 - pixiv
- 首先需要一个能翻墙的运行环境
- 初次使用时需要设置cookie，使用一个平时较少使用的账号登录pixiv，获取cookie后使用 #pixivcookie+获取到的cookie 格式私聊发送给机器人
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

