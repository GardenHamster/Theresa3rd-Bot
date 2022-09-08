# Mysql安装

## 下载
- 从 [mysql官网](https://dev.mysql.com/downloads/installer) 中下载最新版本，或者点击直接下载 [MySQL Installer 8.0.30](https://cdn.mysql.com//Downloads/MySQLInstaller/mysql-installer-community-8.0.30.0.msi)
## 安装

### 打开刚下载的安装包
![image](https://user-images.githubusercontent.com/89188316/161034492-b420439a-f5fb-4bfd-bc3a-d43e63ac9bb5.png)

### 选择Full(完全安装)，然后一直Next
![image](https://user-images.githubusercontent.com/89188316/161034853-96cc6e51-3a71-4621-8b22-1e066ba3b4c1.png)

### 选择Yes
![image](https://user-images.githubusercontent.com/89188316/161035483-5c07bc2a-2b0c-4839-97a0-77872064cb50.png)

### 接着Execute等待执行完毕，然后保持默认一直Next
![image](https://user-images.githubusercontent.com/89188316/161036291-05d4f84d-7d61-470c-b12d-be219bd8e85c.png)

### 这里选择第二项
![image](https://user-images.githubusercontent.com/89188316/161036600-0c7eb97c-5c65-4c9f-880f-09f8a8283869.png)

### 这里设置一下数据库密码
![image](https://user-images.githubusercontent.com/89188316/161037041-b2891423-b1ec-4705-9deb-c94785645760.png)

### 接着一路Next、Execute、Finish
![image](https://user-images.githubusercontent.com/89188316/161037448-f0ff8f57-68a0-4a56-9d40-aef87be72f02.png)

### 将刚才设置的数据库密码填入Password中，然后点check，显示Connection successded后点一路Next、Execute、Finish
![image](https://user-images.githubusercontent.com/89188316/161037965-01a850d5-cc5f-484f-b60f-f8e4cb07a7ed.png)

### 完成后会打开一个Mysql Workbench，点一下下面这个连接
![image](https://user-images.githubusercontent.com/89188316/161039046-bbf3b5de-0d7a-44df-a178-8a4ac53396e7.png)

### 输入数据库密码
![image](https://user-images.githubusercontent.com/89188316/161039376-873eac51-2a37-45a2-ad68-91418f1914b2.png)

### 能进来表示安装成功了
![image](https://user-images.githubusercontent.com/89188316/161039540-72f1b007-4266-40e8-8ab0-8e0df30ef04f.png)

### 如果打开失败，检查一下mysql服务有没有打开
- 搜索 服务 或者 services.msc
![image](https://user-images.githubusercontent.com/89188316/161040329-7fde87a3-4268-47dd-92e4-88059add0170.png)

- 找到Mysql，如果没有正在运行的话，点击右键启动
![image](https://user-images.githubusercontent.com/89188316/161040800-bc413e1d-02e2-4b69-9e78-b823d349b75e.png)

## 将数据库密码写入到bot配置文件中
### 推荐安装一个NodePad++编辑器
### 用NodePad++打开appsettings.Production.json
### 将配置文件中pwd的值(这里是123456)，改为刚才设置的数据库密码，然后保存
![image](https://user-images.githubusercontent.com/89188316/161043245-510c6c00-a2f1-4ed1-864c-f420c4795635.png)


