using System.Security.Cryptography;
using System.Text;

namespace KanjiReader.Domain.Common;

public static class Utilities
{
    public static byte[] Hash(string code)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(code));
    }
    
    public static string GenerateCode()
    {
        return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
    }
}