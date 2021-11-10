# EPlusActivities.API

[![Build Status](https://dev.azure.com/kamasylvia/NGS%20Lottery/_apis/build/status/kamasylvia.EPlusActivities.API?branchName=dev)](https://dev.azure.com/kamasylvia/NGS%20Lottery/_build/latest?definitionId=1&branchName=dev)

农工商 E+ 小程序签到抽奖 API

# 项目进度

## 用户端

- 用户登录：
  - [x] 用户通过手机号+短信验证码方式，登录平台
  - [x] 首次登录创建新用户，生成用户 ID.
- [x] 用户与会员绑定：已登录成功用户通过，以手机号为参数向会员平台发送会员绑定请求，
  - [x] 如果该用户为会员时返回会员 ID
  - [x] 如果用户不是会员，会员平台将新建一个会员并返回会员 ID.
- [x] 用户签到：用户端根据管理端维护的签到活动进行展示及签到操作（区分会员渠道）。
- [x] 用户抽奖：用户端根据管理端维护的抽奖活动进行展示及抽奖操作（区分会员渠道）。
- 用户抽奖和中奖记录：
  - [x] 用户端展示用户中奖记录，
  - 奖品发放情况：
    - [x] 虚拟奖品显示发放结果
    - [x] 实物奖品需要用户确认物流地址后，显示发放结果。
- [x] 用户物流地址维护：用户可以填写多个物流地址（最高 5 个），可设置默认地址。

## 管理端

- 签到活动维护：管理端可以配置签到活动

  - [x] 渠道分类
  - [x] 定义签到周期
  - [x] 每日签到奖品
  - [x] 连续签到奖品
  - 奖品分类：
    - 积分奖品，优惠券奖品，指定抽奖活动抽奖次数奖品。

- 用户抽奖维护：管理端可以配置抽奖活动

  - [x] 抽奖活动周期
  - [x] 展示类型（转盘，挖宝，刮刮卡）
  - [x] 抽奖界面背景更换
  - [x] 抽奖用户规则定义（每用户每天可抽奖次数，每用户活动期内可抽奖数据）
  - [x] 抽奖次数规则
  - [x] 积分兑换抽奖次数
  - [x] 参加外部活动兑换抽奖活动
  - [x] 奖品维护最多十个奖品（积分奖品，会员优惠券奖品，实物奖品），每个奖品都有对应的权重及库存维护。
  - [x] 保存活动并生成活动码

  - 用户管理：
    - [x] 用户列表
    - [x] 绑定会员情况
    - [x] 用户地址查询
    - [x] 激活的渠道

- 用户中奖记录查询

  - [x] 查询用户抽奖记录
  - [x] 更新中奖记录（实物查询）

- 会员渠道维护
  - [x] 会员渠道维护
  - [x] 会员渠道维护、会员奖励积分接口
  - [x] 会员奖励积分接口

# 如何对接联调

### Windows 系统

1. [安装 WSL2](https://docs.microsoft.com/zh-cn/windows/wsl/install-win10)
2. [安装 docker](https://www.docker.com/)
3. [PowerShell/CMD 安装 MySql for docker](https://hub.docker.com/_/mysql/)
   1. 后端开发使用的是 8.0.25 版，这里获取同一版本：`docker pull mysql:8.0.25`
   2. 或者获取最新镜像：`docker pull mysql`
4. 运行 mysql for docker

```sh
docker run -itd --name mysql-test -p 3306:3306 -e MYSQL_ROOT_PASSWORD=123456 mysql
```

5. [安装 .Net 5.0 SDK](https://dotnet.microsoft.com/download)
6. 克隆本项目

```
git clone https://github.com/kamasylvia/EPlusActivities.API.git
```
7. 安装 [Dapr](https://docs.microsoft.com/zh-cn/dotnet/architecture/dapr-for-net-developers/getting-started)
8. 初始化 Dapr
```sh
dapr init
```

# 运行本项目

## .NET SDK（MySQL 用 docker 版）

- `cd` 到解决方案目录(该目录含有 `.sln` 文件) 执行
```sh
$ dapr run --app-id EPlusActivities --app-port 52537 -- dotnet run -p EPlusActivities.API
$ dapr run --app-id FileService --app-port 52500 --app-protocol grpc -- dotnet run -p FileService
```
- 开发环境下，项目运行后，浏览器会自动打开 Swagger，如果没打开或关掉了，请手动打开 http://localhost:52537/swagger/index.html
- 生产环境使用 docker-compose

## docker-compose

- `cd` 到解决方案目录(该目录含有 `.sln` 和 `docker-compose.yml` 文件) 执行
  - `docker-compose up -d`

### 注意

- 因为有反向代理，比如 nginx，容器内的项目是没有开 https 的。
- dapr grpc server 暂时关闭 HTTP/2.0 的 mTLS 验证。

# 测试流程

## 用户验证

1. 发送短信至手机号： POST `http://localhost:52537/api/sms`

```json
{
  "PhoneNumber": "11位手机号"
}
```

2. 获得验证码后发送验证请求： POST `http://localhost:52537/connect/token` with `x-www-form-urlencoded` parameters

| Key           | Value                   |
| ------------- | ----------------------- |
| client_id     | sms.client              |
| grant_type    | sms                     |
| scope         | eplus.test.scope openid |
| login_channel | 1                       |
| phone_number  | 11 位手机号             |
| token         | 验证码                  |

## 管理员验证

POST `http://localhost:52537/connect/token` with `x-www-form-urlencoded` parameters

| Key        | Value                   |
| ---------- | ----------------------- |
| client_id  | password                |
| grant_type | password                |
| scope      | eplus.test.scope openid |
| username   | admin                   |
| password   | Pa\$\$w0rd              |

## 验证通过之后进行登陆

1. 在上一步返回到 json 中提取 `access_token` 作为 JWT。
2. 带着 JWT 访问 GET `http://localhost:52537/connect/userinfo` 获取 `sub` 字段，该字段的值就是用户的 `id`。
3. 访问 Swagger 页面 https://localhost:52538/swagger/index.html。
4. 获取用户信息：GET api/user
5. 创建活动：POST api/activity

# 如何部署

生产环境下使用 docker-compose 部署：

0. 有压缩包的情况下直接解压到新文件夹，跳过 1、2 步。
1. 新建空目录
2. 复制 `docker-compose.yml` 文件，`Settings` 文件夹, `Nginx` 文件夹 和`html` 文件夹到新目录。
   - 如果前端有更新，删除 `html` 文件夹下所有文件并复制新的前端文件到 `html` 下，请确保 `html/index.html` 可访问。
3. 在新目录下运行指令 `docker-compose up -d`
4. 如果 `eplusactivities-api` 服务运行失败，先重启 `eplusactivities-db` 再重启 `eplusactivities-api`。
5. 如果 `fileservice` 服务运行失败，先重启 `fileservice-db` 再重启 `fileservice`。
6. 每次修改 `Nginx` 目录下的文件后，需要重启 `nginx` 服务。
