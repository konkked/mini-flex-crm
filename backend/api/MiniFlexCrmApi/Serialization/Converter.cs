using System.Collections;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MiniFlexCrmApi.Serialization;

public class AttributesJsonConverter : JsonConverter<dynamic>
{
    public override dynamic? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonValueKind.Undefined.Equals(reader.TokenType) || JsonValueKind.Null.Equals(reader.TokenType))
            return null;

        using var doc = JsonDocument.ParseValue(ref reader);
        return ConverterHelper.ConvertToExpando(doc.RootElement);
    }

    public override void Write(Utf8JsonWriter writer, dynamic value, JsonSerializerOptions options)
        => ConverterHelper.Write(writer, value, options);
}

public static class ConverterHelper
{
    public static dynamic? ConvertToExpando(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var expando = new ExpandoObject();
                var dict = (IDictionary<string, dynamic?>)expando;
                
                foreach (var prop in element.EnumerateObject())
                    dict[prop.Name] = ConvertToExpando(prop.Value);
                
                return expando;

            case JsonValueKind.Array:
                return element.EnumerateArray().Select(ConvertToExpando).ToList();

            case JsonValueKind.String:
                return element.GetString();

            case JsonValueKind.Number:
                return element.TryGetInt64(out var longValue) ? longValue : element.GetDecimal();

            case JsonValueKind.True:
                return true;

            case JsonValueKind.False:
                return false;

            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
            default:
                return null;
        }
    }

    public static void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case Dictionary<string, dynamic[]?> relationships:
            {
                writer.WriteStartObject();
                foreach (var key in relationships.Keys)
                {
                    writer.WritePropertyName(key);
                    writer.WriteStartArray();
                    if(relationships[key] != null && relationships[key]?.Length > 0)
                        foreach (var arrayVal in relationships[key])
                            Write(writer, arrayVal, options);
                    writer.WriteEndArray();
                }
                writer.WriteEndObject();

                break;
            }
            case ExpandoObject expando:
            {
                writer.WriteStartObject();
                foreach (var (key, val) in (IDictionary<string, object>)expando)
                {
                    writer.WritePropertyName(key);
                    JsonSerializer.Serialize(writer, val);
                }
                writer.WriteEndObject();
                break;
            }
            case IEnumerable enumerable when !(value is string):
            {
                writer.WriteStartArray();
                foreach (var item in enumerable)
                {
                    JsonSerializer.Serialize(writer, item);
                }
                writer.WriteEndArray();
                break;
            }
            default:
                JsonSerializer.Serialize(writer, value);
                break;
        }
    }
}