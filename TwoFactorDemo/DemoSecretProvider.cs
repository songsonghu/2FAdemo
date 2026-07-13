namespace TwoFactorDemo;

public static class DemoSecretProvider
{
    public const string DevelopmentFixedSecret = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ";

    public static string ResolveSecret(string? environmentName)
    {
        return string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase)
            ? DevelopmentFixedSecret
            : TotpAuthenticator.GenerateSecret();
    }
}
