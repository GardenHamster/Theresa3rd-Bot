?> Bot配置文件在`Config目录`下，如果你找不到该目录，请检查你下载的是否为 [releases](https://github.com/GardenHamster/Theresa3rd-Bot/releases) 中的`Theresa3rd-Bot.zip`

?> 下列说明以点号`.`来表示子配置项，例：`General.Prefix`表示`General.yml`下的`Prefix`配置

## 路径配置
配置文件中存在一些比如`BotImg/download`或者`[image:BotImg/face/emmm.jpg]`这样的路径

这样的路径表示相对于`TheresaBot.MiraiHttpApi.dll`目录下的文件夹路径，你也可以将它修改为对应系统中的绝对路径

## 图片码
配置文件中存在一些类似`ErrorMsg`或者`Template`，这类`*Msg`和`*Template`的配置项

你可以在这类格式的配置项中插入格式为`[image:BotImg/face/emmm.jpg]`或者`[image:C://Theresa3rd-Bot/BotImg/face/emmm.jpg]`这样的图片码。

其中前者表示相对路径，后者表示绝对路径。

## 本地涩图
这个功能需要你设置一个本地图片路径`Setu.Local.LocalPath`，并且这个文件夹下面需要有包含图片的子文件夹

例如：你的路径值设置为`C:\BotImg\localSetu`，那么你需要在`C:\BotImg\localSetu`目录下添加子文件夹来分类存放你的图片

定时涩图中的配置项`TimingSetu.LocalPath`同理

![image](/img/setting/20230215115916.jpg)

## 其他配置
to be continue...