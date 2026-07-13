# 2FAdemo

C# .NET demo for **username/password + TOTP (Microsoft Authenticator)**.

## Run

```bash
dotnet run --project /home/runner/work/2FAdemo/2FAdemo/src/TwoFactorDemo/TwoFactorDemo.csproj
```

Program output includes:
- Demo username/password
- TOTP secret for Microsoft Authenticator (manual add)
- `otpauth://` URI (you can convert it to QR with any local generator)

Then input username, password, and current authenticator code to complete login.

## Test

```bash
dotnet test /home/runner/work/2FAdemo/2FAdemo/TwoFactorDemo.slnx
```
