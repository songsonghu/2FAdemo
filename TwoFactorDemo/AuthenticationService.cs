using System.Security.Cryptography;
using System.Text;

namespace TwoFactorDemo;

public sealed class AuthenticationService
{
    private readonly Dictionary<string, DemoUser> _users = new(StringComparer.OrdinalIgnoreCase);

    public void AddUser(string username, string password, string totpSecret)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = HashPassword(password, salt);
        _users[username] = new DemoUser(username, salt, hash, totpSecret);
    }

    public bool ValidatePassword(string username, string password)
    {
        if (!_users.TryGetValue(username, out var user))
        {
            return false;
        }

        var incomingHash = HashPassword(password, user.PasswordSalt);
        return CryptographicOperations.FixedTimeEquals(incomingHash, user.PasswordHash);
    }

    public bool ValidateTotp(string username, string code, DateTimeOffset? timestamp = null)
    {
        if (!_users.TryGetValue(username, out var user))
        {
            return false;
        }

        return TotpAuthenticator.VerifyCode(user.TotpSecret, code, timestamp: timestamp);
    }

    public bool Authenticate(string username, string password, string code, DateTimeOffset? timestamp = null)
    {
        return ValidatePassword(username, password)
            && ValidateTotp(username, code, timestamp);
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32);
    }
}

public sealed record DemoUser(string Username, byte[] PasswordSalt, byte[] PasswordHash, string TotpSecret);
