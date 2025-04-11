using UnityEngine;

namespace Yjsnpi.Utilities;

public static class RainbowColorGenerator
{
    private const float HueSpeed = 0.2f;
    private const float Saturation = 1.0f;
    private const float Value = 1.0f;
    
    public static Color GetRainbowColor(float offset = 0f)
    {
        offset = Mathf.Clamp01(offset);
        
        float hue = (Time.time * HueSpeed + offset) % 1.0f;
        
        return Color.HSVToRGB(hue, Saturation, Value);
    }
}