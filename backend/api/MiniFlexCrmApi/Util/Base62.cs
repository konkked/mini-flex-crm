using System.Numerics;
using System.Text;

namespace MiniFlexCrmApi.Util;

public static class Base62
{
    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    
    public static string EncodeString(string str)=>Encode(Encoding.Default.GetBytes(str));
    
    /// <summary>
    /// Converts byte array to Base62 string.
    /// </summary>
    public static string Encode(byte[] bytes)
    {
        var value = new BigInteger(bytes.Concat(new byte[] { 0 }).ToArray()); // Ensure positive BigInteger
        var result = new StringBuilder();

        while (value > 0)
        {
            value = BigInteger.DivRem(value, 62, out var remainder);
            result.Insert(0, Base62Chars[(int)remainder]);
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts Base62 string back to byte array.
    /// </summary>
    public static byte[] Decode(string? base62)
    {
        BigInteger value = 0;

        foreach (var c in base62)
        {
            value = value * 62 + Base62Chars.IndexOf(c);
        }

        var bytes = value.ToByteArray();
        return bytes[^1] == 0 ? bytes[..^1] : bytes; // Remove extra leading zero if exists
    }
}