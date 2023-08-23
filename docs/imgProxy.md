# cloudflare免费搭建p站图片代理

### 购买域名
* 首先准备一个自己的域名，如果没有的话可以去阿里云购买一个.xyz之类的比较便宜的域名

* 从阿里云注册域名 https://wanwang.aliyun.com/domain/searchresult

### 注册cloudflare
* 注册然后登录 https://dash.cloudflare.com

* 然后在右上角将 English (US) 切换到 简体中文

### 创建网站
![image](/img/proxy/196925024-36ae2507-d93e-4b41-b866-4d488f8562f2.png)

![image](/img/proxy/196925319-d8444f63-4366-42d4-b4ff-fc8596241b27.png)

![image](/img/proxy/196925535-e16a82cb-3434-4483-b693-068754b3115b.png)

![image](/img/proxy/196927461-08de7d53-5457-468c-9434-9f36fb238529.png)

![image](/img/proxy/196928234-9e264520-2098-4103-acbb-290f711c5f61.png)

![image](/img/proxy/196928397-6f60081a-1c39-42b0-8e4b-5e093e05fa5d.png)

### 修改域名的DNS
![image](/img/proxy/196930014-ef283178-13a9-409c-9376-6ba8cd537c03.png)

![image](/img/proxy/196930240-3f826c25-e093-4640-ad7c-11557ad5f84a.png)

![image](/img/proxy/196931439-52751076-74ff-47b0-ac3d-6795482850c0.png)

![image](/img/proxy/196931830-7c576758-120d-4fe5-bfda-20107d988f0e.png)

![image](/img/proxy/196932086-ecaa0e50-a510-4a37-b164-b3fdfeb00152.png)

![image](/img/proxy/196932566-e0b31824-fa37-444c-87bc-f31ef565667f.png)

### 选择https
![image](/img/proxy/196932907-4e8d0fa2-923c-4eef-8d65-7d88966c2104.png)

### 创建Worker
![image](/img/proxy/197002724-b6e47bab-7419-4504-bcee-e5f258503e4e.png)

![image](/img/proxy/197003418-4a1decc3-cf8b-48c6-8263-3303753b7d46.png)

![image](/img/proxy/197003827-80f401c7-18ee-40c7-858c-2dd232828745.png)

![image](/img/proxy/197004127-33500081-c969-4461-9df0-39c700cda81f.png)

![image](/img/proxy/197004638-bdf7b4fa-41f1-4b6d-ba02-8a758accd4ad.png)

```js
addEventListener('fetch', event => {
    let url = new URL(event.request.url);
    url.hostname = 'i.pximg.net';   
    let request = new Request(url, event.request);
    event.respondWith(
        fetch(request, {
            headers:{
                'Referer': 'https://www.pixiv.net', //需要代理访问的网站
                'User-Agent':'Cloudflare Workers'   //代理服务器
            }
        })
    );
});
```

### 绑定域名
![image](/img/proxy/197005332-6ee4dd74-c044-40a7-9d37-57fa79a9be9c.png)

![image](/img/proxy/197005511-8409ff0a-b07d-4fab-aa4b-a2f13389b105.png)

![image](/img/proxy/197006789-2de916db-6155-4e43-bc27-b479177915a0.png)

![image](/img/proxy/197007874-ea036065-a833-4f97-b98b-29b8784132a7.png)


### 测试
* 这是pixiv上的一条原图链接https://i.pximg.net/img-original/img/2019/03/06/00/40/39/73532572_p0.jpg

* 使用刚才配置的代理域名替换掉i.pximg.net得到链接https://pixiv.gardencavy.site/img-original/img/2019/03/06/00/40/39/73532572_p0.jpg

* 关掉梯子然后打开链接测试图片是否能正常打开，并且发送到手机后再测试一下，如果不行就等一段时间后再测试，生效需要一段时间，可能需要几个小时

![image](/img/proxy/197010554-f110f621-a20e-4955-a439-370efacc8edc.png)

### 修改配置文件
将配置好的代理域名替换掉`Pixiv.OriginUrlProxy`中的值

![image](/img/proxy/197007874-96edc6e3-4860-1d4a-f1ef-3e48ca1cd2f5.jpg)