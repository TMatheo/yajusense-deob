using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace yajusense.Core.Config.JsonConverters;

public class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string value for Color");

        string hex = reader.GetString();
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            return color;

        throw new JsonException($"Invalid color format: {hex}");
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue($"#{ColorUtility.ToHtmlStringRGBA(value)}");
    }
}