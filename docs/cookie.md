!> 注：这部分指令为`私聊指令`，你需要私发给Bot

?> 配置cookie的目的，是为了让Bot能够模仿你登录Pixiv后搜索作品的行为，并且达到自动搜图/推送的目的

!> 本插件并不会向Pixiv以外的网站发送你的cookie，请确保你是在私聊Bot的情况下配置cookie，切勿向其他人泄露你的cookie

### 浏览限制
如果你需要显示R18作品，请确保你的Pixiv账号已允许浏览R18类作品，(默认关闭)

你可以在 `Pixiv`-->`右上角点击你的头像`-->`设置`-->`用户设置`-->`基本设定` 中进行设置

修改完毕后建议退出登录，并且清理缓存后，再进行接下来的操作

![image](/img/cookie/20230325171938.jpg)

## pixiv cookie
pc端登录P站后按下`F12`，然后在P站中随意搜索一个标签，在网络中找到如下请求。

这里以搜索`Hololive`为例(如果搜索中文标签会显示成一串乱码)，获取的cookie中必须包含`PHPSESSID`

然后使用 #pixivcookie [获取到的cookie] 格式私聊发送给机器人，与机器人必须为好友

![image](/img/cookie/177747559-168c1377-db4a-49f0-869f-78749f80707e.png)

![image](/img/cookie/177748449-02f59d79-a0bc-4475-80f6-40f0c56e06a6.png)

## saucenao cookie
在未设置cookie的情况下，Saucenao搜索限制为每个ip每日搜索50次，每30秒搜索3次，在使用频率较高的情况下，建议设置cookie

pc端打开[https://saucenao.com](https://saucenao.com)，点击右下角的Account然后登录Saucenao

按下F12，然后在控制台/Console中输入`document.cookie`

然后使用 #saucenaocookie [获取到的cookie] 格式私聊发送给机器人，与机器人必须为好友

![image](/img/cookie/177758500-94720035-c11a-4689-bb91-eca1ac95ce7e.png)

![image](/img/cookie/177758915-69de1308-d934-407f-a945-17252124c969.png)