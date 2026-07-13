using TwoFactorDemo;

const string username = "demo";
const string password = "P@ssw0rd123!";
const string issuer = "2FAdemo";

var authService = new AuthenticationService();
var secret = TotpAuthenticator.GenerateSecret();
authService.AddUser(username, password, secret);

Console.WriteLine("=== Username + Password + TOTP Demo ===");
Console.WriteLine($"Demo username: {username}");
Console.WriteLine($"Demo password: {password}");
Console.WriteLine($"TOTP secret (add to Microsoft Authenticator manually): {secret}");
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
