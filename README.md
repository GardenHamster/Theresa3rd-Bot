# Theresa3rd-Bot
[![release](https://img.shields.io/github/v/release/GardenHamster/Theresa3rd-Bot)](https://github.com/GardenHamster/Theresa3rd-Bot/releases) [![download](https://img.shields.io/github/downloads/GardenHamster/Theresa3rd-Bot/total)](https://github.com/GardenHamster/Theresa3rd-Bot/releases)


## 简介
 - 某舰团长利用摸鱼时间写出来的自用的bot，准备移植到 [mirai](https://github.com/mamoe/mirai)。基于 .net core 6.0 和 mirai-api-http。
 - 部分功能还在摸鱼开发中，[windows和linux下部署请点击这里](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/Document.md)，[使用示例和食用教程点击这里](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/Menu.md)
 - 开始白嫖前，请在右上角点一下:star:Star
 - 关于bot没有回应或者只会回复ヾ(≧∇≦*)ゝ或٩(๑òωó๑)۶表情的情况[先点一下这里](https://github.com/GardenHamster/Theresa3rd-Bot/blob/main/Document.md#bot没有回复或者只回复表情)
  - ~~不要问bot的名字是什么，因为还没想到好的名字,所以暂时用舰团的名字命名~~

## 声明
 - 本项目为个人自娱自乐写出来的项目，并没有任何收益，并不负责由于使用本项目所产生的任何问题，不得将项目用于任何商业用途
 - 有任何想法问题或者bug请发Issues

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
- [ ] 定时发送本地随机色图功能

### 订阅推送功能
- [x] pixiv画师最新作品推送功能
- [x] pixiv标签最新作品推送功能(收藏数较高且每小时新增一定收藏数的最新作品)
- [x] 米游社用户最新贴子推送功能
- [ ] Bili用户最新动态推送功能
- [ ] Bili直播推送功能

### 抽卡
- [x] ~~原神模拟抽卡，在[GenshinGacha](https://github.com/GardenHamster/GenshinGacha)中实现了~~

### UI
- [ ] 计划用vue写一个可在外网访问的bot管理页面，用于查询和管理订阅列表和黑名单数据等

### 其他摆烂中的功能
- [ ] 小游戏 - 狼人杀
- [ ] 关键词自动回复/禁言/撤回功能
- [ ] 词云功能 - 每日/周/月词云

## 致谢
- 感谢[mamoe/mirai](https://github.com/mamoe/mirai)
- 感谢[Executor-Cheng/mirai-CSharp](https://github.com/Executor-Cheng/mirai-CSharp)

## 效果图
![image](https://user-images.githubusercontent.com/89188316/153139063-7ec31cd9-debe-475f-8ec3-b4660f552d21.png)

![image](https://user-images.githubusercontent.com/89188316/153144525-36b177f2-7ac8-4868-bb4f-223bb6978af9.png)

![image](https://user-images.githubusercontent.com/89188316/153144700-568fb0c8-92c7-4c6e-9868-d4361ab1eb16.png)

![image](https://user-images.githubusercontent.com/89188316/177739246-0002d3e8-3554-4b65-adfc-54aaf440611f.png)

![image](https://user-images.githubusercontent.com/89188316/174220421-1e8bf643-5e2c-4135-94fb-f7980e8be8a3.png)
