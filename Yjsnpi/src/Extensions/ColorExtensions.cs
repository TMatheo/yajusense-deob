using UnityEngine;

namespace Yjsnpi.Extensions;

public static class ColorExtensions
{
    public static string ToHexRGB(this Color color)
    {
        Color32 color32 = color;
        return $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}";
    }

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
}