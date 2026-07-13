using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace TwoFactorDemo;

public static class TotpAuthenticator
{
    private const int TimeStepSeconds = 30;
    private const int Digits = 6;

    public static string GenerateSecret(int byteLength = 20)
    {
        var secret = RandomNumberGenerator.GetBytes(byteLength);
        return Base32Encode(secret);
    }

    public static string BuildProvisioningUri(string issuer, string username, string secret)
    {
        var escapedIssuer = Uri.EscapeDataString(issuer);
        var escapedUsername = Uri.EscapeDataString(username);
        return $"otpauth://totp/{escapedIssuer}:{escapedUsername}?secret={secret}&issuer={escapedIssuer}&digits={Digits}&period={TimeStepSeconds}";
    }

    public static string GenerateCode(string secret, DateTimeOffset? timestamp = null)
    {
        var unixTime = (timestamp ?? DateTimeOffset.UtcNow).ToUnixTimeSeconds();
        var counter = unixTime / TimeStepSeconds;
        Span<byte> counterBytes = stackalloc byte[8];
        for (var i = 7; i >= 0; i--)
        {
            counterBytes[i] = (byte)(counter & 0xff);
            counter >>= 8;
        }

        var key = Base32Decode(secret);
        var hmac = HMACSHA1.HashData(key, counterBytes);
        var offset = hmac[^1] & 0x0f;
        var binary = ((hmac[offset] & 0x7f) << 24)
                     | (hmac[offset + 1] << 16)
                     | (hmac[offset + 2] << 8)
                     | hmac[offset + 3];

        var code = binary % (int)Math.Pow(10, Digits);
        return code.ToString(CultureInfo.InvariantCulture).PadLeft(Digits, '0');
    }

    public static bool VerifyCode(string secret, string code, int allowedDriftSteps = 1, DateTimeOffset? timestamp = null)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != Digits || !code.All(char.IsDigit))
        {
            return false;
        }

        var now = timestamp ?? DateTimeOffset.UtcNow;
        for (var i = -allowedDriftSteps; i <= allowedDriftSteps; i++)
        {
            var candidateTime = now.AddSeconds(i * TimeStepSeconds);
            var candidate = GenerateCode(secret, candidateTime);
            if (CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(candidate), Encoding.ASCII.GetBytes(code)))
            {
                return true;
            }
        }

        return false;
    }

    private static string Base32Encode(byte[] data)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var output = new StringBuilder();
        var bitBuffer = 0;
        var bitsInBuffer = 0;

        foreach (var value in data)
        {
            bitBuffer = (bitBuffer << 8) | value;
            bitsInBuffer += 8;
            while (bitsInBuffer >= 5)
            {
                var index = (bitBuffer >> (bitsInBuffer - 5)) & 0x1f;
                bitsInBuffer -= 5;
                output.Append(alphabet[index]);
            }
        }

        if (bitsInBuffer > 0)
        {
            var index = (bitBuffer << (5 - bitsInBuffer)) & 0x1f;
            output.Append(alphabet[index]);
        }

        return output.ToString();
    }

    private static byte[] Base32Decode(string input)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var cleanInput = input.Trim().TrimEnd('=').ToUpperInvariant();
        var output = new List<byte>();
        var bitBuffer = 0;
        var bitsInBuffer = 0;

        foreach (var c in cleanInput)
        {
            var index = alphabet.IndexOf(c);
            if (index < 0)
            {
                throw new FormatException("Invalid Base32 character.");
            }

            bitBuffer = (bitBuffer << 5) | index;
            bitsInBuffer += 5;
            while (bitsInBuffer >= 8)
            {
                output.Add((byte)((bitBuffer >> (bitsInBuffer - 8)) & 0xff));
                bitsInBuffer -= 8;
            }
        }

        return output.ToArray();
    }
}
