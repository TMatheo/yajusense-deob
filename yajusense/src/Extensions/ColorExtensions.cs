using System;
using System.Globalization;
using UnityEngine;
using yajusense.Core;

namespace yajusense.Extensions;

public static class ColorExtensions
{
    public static string ToHexRGBA(this Color color)
    {
        Color32 color32 = color;
        return $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}";
    }

    public static Color Darken(this Color color, float factor)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        v *= factor;
        return Color.HSVToRGB(h, s, v);
    }

    public static Color ToColor(this string hex)
    {
        if (string.IsNullOrEmpty(hex) || hex.Length != 7 || hex[0] != '#')
        {
            YjPlugin.Log.LogError($"Invalid hex string: {hex}. Expected format is #RRGGBB.");
            return Color.white;
        }

        try
        {
            byte r = byte.Parse(hex.Substring(1, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(3, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(5, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
        catch (FormatException e)
        {
            YjPlugin.Log.LogError($"Error parsing hex string: {hex}. {e.Message}");
            return Color.white;
        }
    }
}