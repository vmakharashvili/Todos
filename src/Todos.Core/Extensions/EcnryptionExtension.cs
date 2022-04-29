using System.Security.Cryptography;
using System.Text;

namespace Todos.Core.Extensions;

public static class EcnryptionExtension
{
    public static string Hash256(this string value, string secretKey)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{value}.{secretKey}"));
        var sb = new StringBuilder();
        foreach (var t in bytes)
        {
            sb.Append(t.ToString("x2"));
        }
        return sb.ToString();
    }
}