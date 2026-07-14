# 2FAdemo

C# .NET demo for **username/password + TOTP (Microsoft Authenticator)**.

## 标准目录结构（VS2022 可直接打开）

- `/home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo.sln`
- `/home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo/TwoFactorDemo.csproj`
- `/home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo.Tests/TwoFactorDemo.Tests.csproj`

## Run

```bash
TWOFA_DEMO_SQLSERVER_CONNECTION="Server=localhost;Database=TwoFactorDemo;Trusted_Connection=True;TrustServerCertificate=True" \
dotnet run --project /home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo/TwoFactorDemo.csproj
```

## SQL Server 表设计

数据库脚本位于：

```bash
/home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo/Database/SqlServerSchema.sql
```

核心表：

- `dbo.Users`
  - `UserId`：主键
  - `Username`：唯一用户名（唯一索引）
  - `PasswordSalt` / `PasswordHash`：密码哈希材料
  - `TotpSecret`：每个用户独立 TOTP secret
  - `CreatedAtUtc` / `UpdatedAtUtc` / `TotpSecretUpdatedAtUtc`：审计时间

## 业务规则

- 每个用户注册时生成自己的 `TotpSecret`，不同用户 secret 不同。
- 同一个用户重复登录/再次运行程序时，secret 保持不变。
- 只有用户选择“重置认证器”时，才会更新该用户的 `TotpSecret`。

## Test

```bash
dotnet test /home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo.sln
```
