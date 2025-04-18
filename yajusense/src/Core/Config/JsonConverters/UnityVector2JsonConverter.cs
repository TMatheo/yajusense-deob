using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vector2 = UnityEngine.Vector2;

namespace yajusense.Core.Config.JsonConverters;

public class UnityVector2JsonConverter : JsonConverter<Vector2>
{
    public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        return new(
            root.GetProperty("x").GetSingle(),
            root.GetProperty("y").GetSingle()
        );
    }

    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("x", value.x);
        writer.WriteNumber("y", value.y);
        writer.WriteEndObject();
    }
}