using System.Text.Json;
using System.Text.Json.Serialization;

namespace MiniFlexCrmApi.Api.Serialization;

public class RelationshipsJsonConverter : JsonConverter<Dictionary<string, dynamic?[]>>
{
    public override Dictionary<string, dynamic?[]>? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (JsonValueKind.Undefined.Equals(reader.TokenType) || JsonValueKind.Null.Equals(reader.TokenType))
            return null;
        
        using var doc = JsonDocument.ParseValue(ref reader);
        var element = doc.RootElement;
        return element.EnumerateObject().Where(p=>p.Value.ValueKind == JsonValueKind.Array)
            .ToDictionary(p => p.Name,  
                p =>  p.Value.EnumerateArray().Select(ConverterHelper.ConvertToExpando).ToArray());
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, dynamic?[]> value, JsonSerializerOptions options)
        => ConverterHelper.Write(writer, value, options);
}