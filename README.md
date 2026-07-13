# 2FAdemo

C# .NET demo for **username/password + TOTP (Microsoft Authenticator)**.

## 标准目录结构（VS2022 可直接打开）

- `/home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo.sln`
- `/home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo/TwoFactorDemo.csproj`
- `/home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo.Tests/TwoFactorDemo.Tests.csproj`

## Run

```bash
dotnet run --project /home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo/TwoFactorDemo.csproj
```

### 固定 secret（仅开发模式）

设置开发环境变量后，程序会使用固定 TOTP secret，避免每次运行都要重新绑定 Authenticator：

```bash
DOTNET_ENVIRONMENT=Development dotnet run --project /home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo/TwoFactorDemo.csproj
```

## Test

```bash
dotnet test /home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo.sln
```
