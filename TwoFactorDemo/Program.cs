using TwoFactorDemo;

const string issuer = "2FAdemo";

var connectionString = Environment.GetEnvironmentVariable("TWOFA_DEMO_SQLSERVER_CONNECTION");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Please set TWOFA_DEMO_SQLSERVER_CONNECTION to a SQL Server connection string.");
    return;
}

var userStore = new SqlServerUserStore(connectionString);
userStore.EnsureSchema();
var authService = new AuthenticationService(userStore);

Console.WriteLine("=== Username + Password + TOTP Demo (SQL Server) ===");
Console.Write("Username: ");
var username = (Console.ReadLine() ?? string.Empty).Trim();
Console.Write("Password: ");
var password = Console.ReadLine() ?? string.Empty;

if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
{
    Console.WriteLine("Username and password are required.");
    return;
}

var registrationResult = authService.RegisterUser(username, password);
if (registrationResult.IsNewUser)
{
    var secret = registrationResult.Secret!;
    Console.WriteLine("User registered successfully.");
    Console.WriteLine($"TOTP secret (add to Microsoft Authenticator): {secret}");
    Console.WriteLine("Provisioning URI:");
    Console.WriteLine(TotpAuthenticator.BuildProvisioningUri(issuer, username, secret));
    Console.WriteLine("请先在 Authenticator 完成绑定，然后再登录。");
    return;
}

if (!authService.ValidatePassword(username, password))
{
    Console.WriteLine("Primary authentication failed.");
    return;
}

Console.Write("Reset authenticator now? (y/N): ");
var resetChoice = Console.ReadLine() ?? string.Empty;
if (string.Equals(resetChoice, "y", StringComparison.OrdinalIgnoreCase))
{
    var newSecret = authService.ResetTotpSecret(username);
    Console.WriteLine("Authenticator reset completed.");
    Console.WriteLine($"New TOTP secret: {newSecret}");
    Console.WriteLine("Provisioning URI:");
    Console.WriteLine(TotpAuthenticator.BuildProvisioningUri(issuer, username, newSecret));
    return;
}

Console.Write("TOTP code from Authenticator: ");
var inputCode = Console.ReadLine() ?? string.Empty;

Console.WriteLine(authService.ValidateTotp(username, inputCode)
    ? "2FA success. Login granted."
    : "TOTP verification failed. Login denied.");
