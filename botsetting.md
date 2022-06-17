## 关于botsetting.yml中的一些补充说明

### 在消息和模版中插入图片
- 配置文件中 `Template` 和 `****Msg` 以及 `****Img` 中可以使用占位符插入图片，使用格式 `[image:本地图片路径]` 表示一张图片，例：
```yml
DownErrorImg:           '[image:C:\BotImg\face\downError.png]'              #图片下载失败时的替代图片
```
```yml
Template:         |                                                   #群欢迎模版
                  欢迎萌新
                  [image:C:\BotImg\face\welcome.png]
                  请认真阅读群内的相关公告
```

### 模版中的占位符
- `Template` 中 有时候会包含比如类似 `{UserName}` 这样的占位符，类似这样的占位符会在程序执行时被实际获取到的值所代替
- 通常每个 `Template` 都有属于它的固定的占位符，这些占位符可以不出现，但是只有在这个 `Template` 包含一个这样占位符时，这个占位符才会被实际值替换

### Cron表达式
- `Cron` 的值为一个cron表达式，用于表示触发一个定时器执行时机的一个字符串
- 可以自定义或者通过类似 [在线cron表达式生成器](https://www.bejson.com/othertools/cron/) 等工具生成
