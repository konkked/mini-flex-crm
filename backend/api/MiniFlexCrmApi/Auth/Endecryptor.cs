using System.Security.Cryptography;

namespace MiniFlexCrmApi.Auth;

public interface IEndecryptor
{
    string Encrypt(string unencrypted, int radix);
    string Decrypt(string encrypted, int radix);
}

public class Endecryptor : IEndecryptor
{
    private readonly byte[] _key;
    private const string RADIX_CHARS = "01ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+/"; // 64 chars

    public Endecryptor(IEncryptionSecretProvider secretProvider)
    {
        // Decode Base64 secret and derive 32-byte key with HMAC-SHA256
        var base64Key = secretProvider.GetSecret();
        var secretBytes = Convert.FromBase64String(base64Key);
        using var hmac = new HMACSHA256(secretBytes);
        _key = hmac.ComputeHash("MurphyIsSmurfyAlyIsBoobali#1127"u8.ToArray());
        Array.Resize(ref _key, 32); // AES-256 key
    }

    public string Encrypt(string unencrypted, int radix)
    {
        if (string.IsNullOrEmpty(unencrypted)) throw new ArgumentNullException(nameof(unencrypted));
        if (radix < 2 || radix > 64) throw new ArgumentOutOfRangeException(nameof(radix), "Radix must be between 2 and 64");

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();
        var iv = aes.IV;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        ms.Write(iv, 0, iv.Length); // Prepend IV
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var writer = new StreamWriter(cs);
        writer.Write(unencrypted);
        var encryptedBytes = ms.ToArray();
        return BytesToRadixString(encryptedBytes, radix); // Convert to specified radix
    }

    public string Decrypt(string encrypted, int radix)
    {
        if (string.IsNullOrEmpty(encrypted)) 
            throw new ArgumentNullException(nameof(encrypted));
        if (radix < 2 || radix > 64) 
            throw new ArgumentOutOfRangeException(nameof(radix), "Radix must be between 2 and 64");

        var encryptedBytes = RadixStringToBytes(encrypted, radix);

        using var aes = Aes.Create();
        aes.Key = _key;
        var iv = new byte[16];
        Array.Copy(encryptedBytes, 0, iv, 0, iv.Length); // Extract IV
        aes.IV = iv;

        var cipherText = new byte[encryptedBytes.Length - iv.Length];
        Array.Copy(encryptedBytes, iv.Length, cipherText, 0, cipherText.Length);

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(cipherText);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);
        return reader.ReadToEnd();
    }

    private string BytesToRadixString(byte[] bytes, int radix)
    {
        // Convert bytes to a BigInteger, then to radix string
        var result = "";
        System.Numerics.BigInteger num = 0;
        foreach (var b in bytes)
        {
            num = (num << 8) + b; // Build big integer from bytes
        }

        if (num == 0) return RADIX_CHARS[0].ToString();

        while (num > 0)
        {
            var digit = (int)(num % radix);
            result = RADIX_CHARS[digit] + result;
            num /= radix;
        }
        return result;
    }

    private byte[] RadixStringToBytes(string radixStr, int radix)
    {
        // Convert radix string back to BigInteger, then to bytes
        System.Numerics.BigInteger num = 0;
        foreach (var c in radixStr)
        {
            var digit = RADIX_CHARS.IndexOf(c);
            if (digit < 0 || digit >= radix) throw new FormatException("Invalid character for specified radix");
            num = num * radix + digit;
        }

        // Convert BigInteger to byte array
        var bytes = num.ToByteArray();
        // BigInteger may add a leading zero byte for sign, remove if present and not needed
        if (bytes.Length > 1 && bytes[bytes.Length - 1] == 0)
        {
            Array.Resize(ref bytes, bytes.Length - 1);
        }
        Array.Reverse(bytes); // Little-endian to big-endian
        return bytes;
    }
}