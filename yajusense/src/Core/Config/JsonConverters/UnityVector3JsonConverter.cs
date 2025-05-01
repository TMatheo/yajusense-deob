using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vector3 = UnityEngine.Vector3;

namespace yajusense.Core.Config.JsonConverters;

public class UnityVector3JsonConverter : JsonConverter<Vector3>
{
    public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;
        return new Vector3(root.GetProperty("x").GetSingle(), root.GetProperty("y").GetSingle(), root.GetProperty("z").GetSingle());
    }

    public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("x", value.x);
        writer.WriteNumber("y", value.y);
        writer.WriteNumber("z", value.z);
        writer.WriteEndObject();
    }
}