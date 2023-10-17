# Theresa3rd-Bot
[![release](https://img.shields.io/github/v/release/GardenHamster/Theresa3rd-Bot)](https://github.com/GardenHamster/Theresa3rd-Bot/releases) [![download](https://img.shields.io/github/downloads/GardenHamster/Theresa3rd-Bot/total)](https://github.com/GardenHamster/Theresa3rd-Bot/releases)

## 简介
 - 某舰团长利用 ~~摸鱼时间~~ 写出来的自用的Bot
 - 基于 .net core 6.0，可以在 Windows 或 Linux 上运行
 - 对接了 [mirai-api-http](https://github.com/project-mirai/mirai-api-http) (使用SDK [Mirai-CSharp](https://github.com/Executor-Cheng/mirai-CSharp)) 
 - 对接了 [go-cqhttp](https://github.com/Mrs4s/go-cqhttp) (使用SDK [EleCho.GoCqHttpSdk](https://github.com/OrgEleCho/EleCho.GoCqHttpSdk))
 - [部署文档和使用教程请点击这里](https://www.theresa3rd.cn)，如果链接失效可以访问 [github-pages](https://gardenhamster.github.io/Theresa3rd-Bot)
 - 开始白嫖前，请在右上角点一下:star:Star
 
## 声明
 - **本项目为个人自娱自乐写出来的项目，并没有任何收益，并不对使用本项目产生的任何问题负责，不得将项目用于任何商业用途**

## 关于提问
 - **如果在部署或者使用过程中遇到问题时，请仔细阅读上面的文档后再来提问！！！**
 - **如果在部署或者使用过程中遇到问题时，请仔细阅读上面的文档后再来提问！！！**
 - **如果在部署或者使用过程中遇到问题时，请仔细阅读上面的文档后再来提问！！！**
 - Issuse请贴出相关的报错信息，mirai-http-api或go-cqhttp版本，本插件版本，botsettings.yml相关内容，必要的聊天记录截图
 - 只发一句`没有反应`，`无法运行`之类的Issuse不予解答

## 功能和进度
### 消息提醒
- [x] 简易版复读机
- [x] 自定义入群欢迎
- [x] 自定义定时提醒功能

### 色图相关
- [x] [Lolicon](https://api.lolicon.app) 随机涩图
- [x] [Lolisuki](https://lolisuki.cc) 随机涩图
- [x] 本地随机涩图 
- [x] pixiv免代理 
- [x] pixivid查询
- [x] pixiv动图转gif
- [x] pixiv标签搜索(根据标签获取随机的收藏度较高的色图，可多标签搜索)
- [x] pixiv随机搜索(随机标签/随机关注画师的作品/随机收藏中的作品/随机订阅中的作品)
- [x] Pixiv日榜/周榜/月榜/AI榜/男性向日榜预览图
- [x] pixiv画师作品预览图
- [x] saucenao搜图(如果图片来源于pixiv时，尝试下载并返回原图，并且优先返回来源于pixiv中的原图)
- [x] ascii2d搜图(当saucenao中找不到来源时，继续使用ascii2d搜索)
- [x] 每日/周/月/年词云，昨日/上周/上月/去年词云，词云蒙版自定义

### 订阅推送功能
- [x] pixiv画师最新作品推送功能
- [x] pixiv标签最新作品推送功能(收藏数较高且每小时新增一定收藏数的最新作品)
- [x] 米游社用户最新贴子推送功能
- [x] Pixiv日榜/周榜/月榜/AI榜/男性向榜推送功能
- [x] 定时涩图推送，包括本地色图，lolicon，lolisuki

### 模拟抽卡
- [x] ~~原神模拟抽卡，在[GenshinGacha](https://github.com/GardenHamster/GenshinGacha)中实现了~~

### TODO
- [ ] 写一个可在外网访问的 [Web UI](https://github.com/GardenHamster/Theresa3rd-Bot-Web)，用于查询和管理订阅列表和黑名单数据等
- [ ] 做一个涩图收藏功能(包括添加到Pixiv收藏，记录到数据库，保存到本地文件夹)

### 其他画饼中的功能
- [ ] 小游戏 - 狼人杀
- [ ] 小游戏 - 谁是卧底

## 致谢
- [mamoe/mirai](https://github.com/mamoe/mirai)
- [Executor-Cheng/mirai-CSharp](https://github.com/Executor-Cheng/mirai-CSharp)
- [Mrs4s/go-cqhttp](https://github.com/Mrs4s/go-cqhttp)
- [EleCho.GoCqHttpSdk](https://github.com/OrgEleCho/EleCho.GoCqHttpSdk)
- [Lolicon Api](https://api.lolicon.app)
- [Lolisuki Api](https://lolisuki.cc)
- [saucenao.com](https://saucenao.com)
- [ascii2d.net](https://ascii2d.net)
- [WordCloud.NetCore](https://github.com/GardenHamster/WordCloud.NetCore)

## 一些效果图

![Screenshot_2023-08-14-20-08-40-270_com tencent mo](https://github.com/GardenHamster/Theresa3rd-Bot/assets/89188316/bbd6ccda-0b27-49de-9f73-aa8670fc7966)

![Screenshot_2023-08-15-00-05-58-765_com tencent mo](https://github.com/GardenHamster/Theresa3rd-Bot/assets/89188316/fbdecc9d-9267-4c12-9b1f-b7ec6544657c)

![Screenshot_2023-08-17-14-51-05-588_com tencent mo](https://github.com/GardenHamster/Theresa3rd-Bot/assets/89188316/b61f320d-349e-46ee-acbc-d2d2f1fa03df)

![Screenshot_2023-08-15-00-28-27-643_com tencent mo](https://github.com/GardenHamster/Theresa3rd-Bot/assets/89188316/e5e15984-bf82-4fb4-b826-2f1d3635505b)

![Screenshot_2023-08-15-00-22-13-259_com tencent mo](https://github.com/GardenHamster/Theresa3rd-Bot/assets/89188316/d758dde8-9c69-4e3e-9ffa-8bd94de3bc5d)

![Screenshot_2023-08-17-15-15-27-951_com tencent mo](https://github.com/GardenHamster/Theresa3rd-Bot/assets/89188316/68c4db47-08f1-4276-bdaf-f66b0640640c)

![Screenshot_2023-08-17-15-22-14-318_com tencent mo](https://github.com/GardenHamster/Theresa3rd-Bot/assets/89188316/dbffb85b-e328-4237-8c35-c4af8f50d63b)
