# 2FAdemo

C# .NET demo for **username/password + TOTP (Microsoft Authenticator)**.

## 标准目录结构（VS2022 可直接打开）

- `/home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo/TwoFactorDemo.sln`
- `/home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo/TwoFactorDemo/TwoFactorDemo.csproj`
- `/home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo/TwoFactorDemo.Tests/TwoFactorDemo.Tests.csproj`

## Run

```bash
dotnet run --project /home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo/TwoFactorDemo/TwoFactorDemo.csproj
```

Program output includes:
- Demo username/password
- TOTP secret for Microsoft Authenticator (manual add)
- `otpauth://` URI (you can convert it to QR with any local generator)

Then input username, password, and current authenticator code to complete login.

## Test

```bash
dotnet test /home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo/TwoFactorDemo.sln
```
