using System.Numerics;
using System.Text;
using System.Text.Json;

namespace MiniFlexCrmApi.Api.Services;

public static class Base62JsonConverter
{
    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Serializes an object to JSON and encodes it in Base62.
    /// </summary>
    public static string Serialize<T>(T obj)
    {
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(obj);
        return EncodeBase62(jsonBytes);
    }

    /// <summary>
    /// Decodes Base62 back into an object.
    /// </summary>
    public static T Deserialize<T>(string encoded)
    {
        var jsonBytes = DecodeBase62(encoded);
        return JsonSerializer.Deserialize<T>(jsonBytes) ?? throw new InvalidOperationException("Failed to deserialize object.");
    }
	
    public static T DeserializeAnonymous<T>(string encoded, T example) => Deserialize<T>(encoded);

    /// <summary>
    /// Converts byte array to Base62 string.
    /// </summary>
    private static string EncodeBase62(byte[] bytes)
    {
        BigInteger value = new BigInteger(bytes.Concat(new byte[] { 0 }).ToArray()); // Ensure positive BigInteger
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
    private static byte[] DecodeBase62(string base62)
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