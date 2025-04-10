using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace Yjsnpi.Core.Config.JsonConverters;

public class KeyCodeJsonConverter : JsonConverter<KeyCode>
{
    public override KeyCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var intValue))
        {
            return (KeyCode)intValue;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var strValue = reader.GetString();
            if (Enum.TryParse<KeyCode>(strValue, true, out var result))
            {
                return result;
            }
        }

        return KeyCode.None;
    }

    public override void Write(Utf8JsonWriter writer, KeyCode value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}