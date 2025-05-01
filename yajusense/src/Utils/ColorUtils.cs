using UnityEngine;

namespace yajusense.Utils;

public static class ColorUtils
{
    public static Color GetRainbowColor(float offset = 0f, float hueSpeed = 0.4f)
    {
        float hue = (Time.time * hueSpeed + offset) % 1.0f;

        if (hue < 0)
            hue += 1.0f;

        return Color.HSVToRGB(hue, 1f, 1f);
    }
}