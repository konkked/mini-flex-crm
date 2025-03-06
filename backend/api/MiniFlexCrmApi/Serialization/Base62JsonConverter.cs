using System.Text.Json;
using MiniFlexCrmApi.Util;

namespace MiniFlexCrmApi.Serialization;

public static class Base62JsonConverter
{
    /// <summary>
    /// Serializes an object to JSON and encodes it in Base62.
    /// </summary>
    public static string Serialize<T>(T obj)
    {
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(obj);
        return Base62.Encode(jsonBytes);
    }

    /// <summary>
    /// Decodes Base62 back into an object.
    /// </summary>
    public static T Deserialize<T>(string? encoded)
    {
        var jsonBytes = Base62.Decode(encoded);
        return JsonSerializer.Deserialize<T>(jsonBytes) 
               ?? throw new InvalidOperationException("Failed to deserialize object.");
    }
	
    public static T DeserializeAnonymous<T>(string? encoded, T example) 
        => Deserialize<T>(encoded);

}