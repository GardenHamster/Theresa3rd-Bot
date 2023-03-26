# Theresa3rd-Bot
[![release](https://img.shields.io/github/v/release/GardenHamster/Theresa3rd-Bot)](https://github.com/GardenHamster/Theresa3rd-Bot/releases) [![download](https://img.shields.io/github/downloads/GardenHamster/Theresa3rd-Bot/total)](https://github.com/GardenHamster/Theresa3rd-Bot/releases)

## 简介
 - 某舰团长利用~~摸鱼时间~~业余时间写出来的自用的Bot，基于Mirai平台(通过[Mirai-CSharp](https://github.com/Executor-Cheng/mirai-CSharp)与之交互)
 - 基于 .net core 6.0 和 mirai-api-http，可以在Windows或Linux上运行
 - 开始白嫖前，请在右上角点一下:star:Star
 - [部署文档和使用教程请点击这里](https://www.theresa3rd.cn)，如果链接失效可以访问 [GitHub Pages](https://gardenhamster.github.io/TheresaBotDoc)
 
## 声明
 - **本项目为个人自娱自乐写出来的项目，并没有任何收益，并不对使用本项目产生的任何问题负责，不得将项目用于任何商业用途**

## 提问
 - **如果在部署或者使用过程中遇到问题时，请仔细阅读上面的文档后再来提问！！！**
 - **如果在部署或者使用过程中遇到问题时，请仔细阅读上面的文档后再来提问！！！**
 - **如果在部署或者使用过程中遇到问题时，请仔细阅读上面的文档后再来提问！！！**
 - Issuse请贴出相关的报错信息，mirai-http-api和本插件版本，botsettings.yml相关配置，必要的聊天记录截图
 - 只发一句`没有反应`，`无法运行`之类的Issuse不予解答

## 功能和进度
### 基本
- [x] 简易版复读机
- [x] 自定义入群欢迎
- [x] 自定义定时提醒功能

### 色图相关
- [x] [Lolicon](https://api.lolicon.app)随机涩图
- [x] [Lolisuki](https://lolisuki.cc)随机涩图
- [x] pixiv免代理 
- [x] pixivid搜索
- [x] pixiv标签搜索(根据标签获取随机的收藏度较高的色图，可多标签搜索)
- [x] pixiv随机色图(随机标签/随机关注画师的作品/随机收藏中的作品/随机订阅中的作品)
- [x] pixiv动图转gif功能
- [x] saucenao搜图功能(如果图片来源于pixiv时，尝试下载并返回原图，并且优先返回来源于pixiv中的原图)
- [x] ascii2d搜图功能(当saucenao中找不到来源时，继续使用ascii2d搜索)
- [x] 定时色图功能，包括本地色图，lolicon，lolisuki
- [x] Pixiv日榜/周榜/月榜/AI榜/男性向日榜

### 订阅推送功能
- [x] pixiv画师最新作品推送功能
- [x] pixiv标签最新作品推送功能(收藏数较高且每小时新增一定收藏数的最新作品)
- [x] 米游社用户最新贴子推送功能
- [x] Pixiv日榜/周榜/月榜/AI榜/男性向榜推送功能

### 抽卡
- [x] ~~原神模拟抽卡，在[GenshinGacha](https://github.com/GardenHamster/GenshinGacha)中实现了~~

### TODO
- [ ] 写一个可在外网访问的Web UI，用于查询和管理订阅列表和黑名单数据等
- [ ] 做一个涩图收藏功能(包括添加到Pixiv收藏，记录到数据库，保存到本地文件夹)
- [ ] 做一个画师作品一览图功能

### 其他摆烂中的功能
- [ ] 小游戏 - 狼人杀
- [ ] 词云功能 - 每日/周/月词云
- [ ] ~~反正也已经有dalao写好类似的插件了~~

## 致谢
- [mamoe/mirai](https://github.com/mamoe/mirai)
- [Executor-Cheng/mirai-CSharp](https://github.com/Executor-Cheng/mirai-CSharp)
- [Lolicon Api](https://api.lolicon.app)
- [Lolisuki Api](https://lolisuki.cc)
- [saucenao.com](https://saucenao.com)
- [ascii2d.net](https://ascii2d.net)

## 一些效果图
![image](https://user-images.githubusercontent.com/89188316/153139063-7ec31cd9-debe-475f-8ec3-b4660f552d21.png)

![image](https://user-images.githubusercontent.com/89188316/153144525-36b177f2-7ac8-4868-bb4f-223bb6978af9.png)

![image](https://user-images.githubusercontent.com/89188316/153144700-568fb0c8-92c7-4c6e-9868-d4361ab1eb16.png)

![image](https://user-images.githubusercontent.com/89188316/177739246-0002d3e8-3554-4b65-adfc-54aaf440611f.png)

![image](https://user-images.githubusercontent.com/89188316/197740246-9850327e-3cd1-4dd5-9ade-402ce613bf7d.png)

![image](https://user-images.githubusercontent.com/89188316/222678926-469e27f6-1efe-426d-9646-9d2821e9904d.png)

