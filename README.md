# Theresa3rd-Bot
[![release](https://img.shields.io/github/v/release/GardenHamster/Theresa3rd-Bot)](https://github.com/GardenHamster/Theresa3rd-Bot/releases) [![download](https://img.shields.io/github/downloads/GardenHamster/Theresa3rd-Bot/total)](https://github.com/GardenHamster/Theresa3rd-Bot/releases)


## 简介
 - 某舰团长利用摸鱼时间写出来的自用的bot，准备移植到 [mirai](https://github.com/mamoe/mirai)
 - 基于 .netcore6.0 和 mirai-api-http，可以在Windows或Linux上运行
 - 开始白嫖前，请在右上角点一下:star:Star
 - [部署文档和使用教程请点击这里](https://www.theresa3rd.cn)

## 声明
 - 本项目为个人自娱自乐写出来的项目，并没有任何收益，并不负责由于使用本项目所产生的任何问题，不得将项目用于任何商业用途
 - 如果有建议问题或者bug请发Issues

## 其他
 - **如果在部署或者使用过程中遇到问题时，请仔细阅读上面的文档后再来提问！！！**
 - **如果在部署或者使用过程中遇到问题时，请仔细阅读上面的文档后再来提问！！！**
 - **如果在部署或者使用过程中遇到问题时，请仔细阅读上面的文档后再来提问！！！**

## 功能和进度
### 基本
- [x] 简易版复读机
- [x] 自定义入群欢迎
- [x] 自定义定时提醒功能

### 色图相关
- [x] 接入 [Lolicon Api](https://api.lolicon.app/#/setu)
- [x] 接入 [Lolisuki Api](https://github.com/GardenHamster/LoliSuki)
- [x] pixiv免代理 
- [x] pixiv标签搜索功能(根据标签获取随机的收藏度较高的色图，可多标签搜索)
- [x] pixiv随机色图功能(随机标签/随机关注画师的作品/随机收藏中的作品/随机订阅中的作品)
- [x] pixivid搜索功能
- [x] pixiv动图转gif功能
- [x] saucenao搜图功能(如果图片来源于pixiv时，尝试下载并返回原图，并且优先返回来源于pixiv中的原图)
- [x] ascii2d搜图功能(当saucenao中找不到来源时，继续使用ascii2d搜索)
- [x] 定时色图功能，包括本地色图，lolicon，lolisuki
- [ ] Pixiv日榜/周榜/月榜/AI榜/男性向日榜 查询和推送功能

### 订阅推送功能
- [x] pixiv画师最新作品推送功能
- [x] pixiv标签最新作品推送功能(收藏数较高且每小时新增一定收藏数的最新作品)
- [x] 米游社用户最新贴子推送功能

### 抽卡
- [x] ~~原神模拟抽卡，在[GenshinGacha](https://github.com/GardenHamster/GenshinGacha)中实现了~~

### Web UI
- [ ] 计划用vue写一个可在外网访问的bot管理页面，用于查询和管理订阅列表和黑名单数据等

### 其他摆烂中的功能，~~反正也已经有dalao写好类似的插件了~~
- [ ] 小游戏 - 狼人杀
- [ ] 关键词自动回复/禁言/撤回功能
- [ ] 词云功能 - 每日/周/月词云
- [ ] Bili用户最新动态推送功能
- [ ] Bili直播推送功能

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
