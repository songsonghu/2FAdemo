using TwoFactorDemo;

const string username = "demo";
const string password = "P@ssw0rd123!";
const string issuer = "2FAdemo";

var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
          ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var isDevelopment = string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase);

var authService = new AuthenticationService();
var secret = DemoSecretProvider.ResolveSecret(env);
authService.AddUser(username, password, secret);

Console.WriteLine("=== Username + Password + TOTP Demo ===");
Console.WriteLine($"Environment: {env ?? "Production"}");
Console.WriteLine($"Demo username: {username}");
Console.WriteLine($"Demo password: {password}");
Console.WriteLine($"TOTP secret (add to Microsoft Authenticator manually): {secret}");
if (isDevelopment)
{
    Console.WriteLine("Development mode uses a fixed TOTP secret.");
}
Console.WriteLine("Or use this URI to generate a QR code:");
Console.WriteLine(TotpAuthenticator.BuildProvisioningUri(issuer, username, secret));
Console.WriteLine();

Console.Write("Username: ");
var inputUser = Console.ReadLine() ?? string.Empty;
Console.Write("Password: ");
var inputPassword = Console.ReadLine() ?? string.Empty;

if (!authService.ValidatePassword(inputUser, inputPassword))
{
    Console.WriteLine("Primary authentication failed.");
    return;
}

Console.Write("TOTP code from Authenticator: ");
var inputCode = Console.ReadLine() ?? string.Empty;

Console.WriteLine(authService.ValidateTotp(inputUser, inputCode)
    ? "2FA success. Login granted."
    : "TOTP verification failed. Login denied.");
