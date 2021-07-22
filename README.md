# EPlusActivities.API

农工商 E+ 小程序签到抽奖 API

# 如何对接

### Windows 系统

1. [安装 WSL2](https://docs.microsoft.com/zh-cn/windows/wsl/install-win10)
2. [安装 docker](https://www.docker.com/)
3. [PowerShell/CMD 安装 MySql for docker](https://hub.docker.com/_/mysql/)
   1. 获取最新镜像：`docker pull mysql`
   2. 后端开发使用的是 8.0.25 版，这里获取同一版本：`docker pull mysql:8.0.25`
4. 运行 mysql for docker

```sh
docker run -itd --name mysql-test -p 3306:3306 -e MYSQL_ROOT_PASSWORD=123456 mysql
```

5. [安装 .Net 5.0 SDK](https://dotnet.microsoft.com/download)
6. 克隆本项目

```
git clone https://github.com/kamasylvia/EPlusActivities.API.git
```

7. 运行本项目
   - `cd` 到解决方案目录(该目录含有 `.sln` 文件) 执行 `dotnet watch run -p EPlusActivities.API`
   - 或 `cd` 到项目目录（该目录含有 `.csproj` 文件） 执行 `dotnet watch run`
8. 浏览器会自动打开 Swagger，如果没打开或关掉了，请尝试点击[这里](https://localhost:52538/swagger/index.html)打开。

# 测试流程

1. 发送短信至手机号： POST `http://localhost:52537/api/sms`

```json
{
  "PhoneNumber": "11位手机号"
}
```

2. 获得验证码后发送验证请求： POST `http://localhost:52537 /connect/token` with `x-www-form-urlencoded` parameters

| Key           | Value                   |
| ------------- | ----------------------- |
| client_id     | sms.client              |
| client_secret | secret                  |
| grant_type    | sms                     |
| scope         | eplus.test.scope openid |
| phone_number  | 11 位手机号             |
| token         | 验证码                  |
| login_channel | 1                       |
