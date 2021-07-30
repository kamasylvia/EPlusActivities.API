# EPlusActivities.API

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
  - [ ] 抽奖界面背景更换
  - [x] 抽奖用户规则定义（每用户每天可抽奖次数，每用户活动期内可抽奖数据）
  - [ ] 抽奖次数规则
  - [ ] 积分兑换抽奖次数
  - [ ] 参加外部活动兑换抽奖活动
  - [x] 奖品维护最多十个奖品（积分奖品，会员优惠券奖品，实物奖品），每个奖品都有对应的权重及库存维护。
  - [ ] 保存活动并生成活动码

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

# 如何对接

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

7. 运行本项目
   - `cd` 到解决方案目录(该目录含有 `.sln` 文件) 执行 `dotnet watch run -p EPlusActivities.API` 或 `dotnet run -p EPlusActivities.API`.
   - 或 `cd` 到项目目录（该目录含有 `.csproj` 文件） 执行 `dotnet watch run`, 或 `dotnet run`.
8. 项目运行后，浏览器会自动打开 Swagger，如果没打开或关掉了，请手动打开 https://localhost:52538/swagger/index.html

# 测试流程

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
| client_secret | secret                  |
| grant_type    | sms                     |
| scope         | eplus.test.scope openid |
| phone_number  | 11 位手机号             |
| token         | 验证码                  |
| login_channel | 1                       |

3. 在上一步返回到 json 中提取 `access_token` 作为 JWT。
4. 带着 JWT 访问 GET `http://localhost:52537/connect/userinfo` 获取 `sub` 字段，该字段的值就是用户的 `id`。
5. 后续步骤可以参考 webapi 打开的 Swagger 页面。
