namespace TwoFactorDemo;

public interface IUserStore
{
    DemoUser? FindByUsername(string username);
    void Create(DemoUser user);
    void UpdateTotpSecret(string username, string totpSecret);
}

public sealed class InMemoryUserStore : IUserStore
{
    private readonly Dictionary<string, DemoUser> _users = new(StringComparer.OrdinalIgnoreCase);

    public DemoUser? FindByUsername(string username)
    {
        return _users.TryGetValue(username, out var user) ? user : null;
    }

    public void Create(DemoUser user)
    {
        if (_users.ContainsKey(user.Username))
        {
            throw new InvalidOperationException("User already exists.");
        }

        _users[user.Username] = user;
    }

    public void UpdateTotpSecret(string username, string totpSecret)
    {
        if (!_users.TryGetValue(username, out var user))
        {
            throw new InvalidOperationException("User not found.");
        }

        _users[username] = user with { TotpSecret = totpSecret };
    }
}
