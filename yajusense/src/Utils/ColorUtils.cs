using System;
using System.Globalization;
using UnityEngine;
using yajusense.Modules;
using yajusense.Modules.Visual;

namespace yajusense.Utils;

public static class ColorUtils
{
    private static readonly Color Color1 = HexToColor("#6E48AA");
    private static readonly Color Color2 = HexToColor("#D4C7D9");
    
    public static Color GetClientColor(float offset = 0f, float speed = 0.4f)
    {
        if (ModuleManager.ClientSettings.MLGMode)
        {
            GetRainbowColor(offset, speed);
        }
        
        float t = Mathf.Sin(Time.time * speed + offset) * 0.5f + 0.5f;
        
        return Color.Lerp(Color1, Color2, t);
    }

    private static Color HexToColor(string hex)
    {
        if (string.IsNullOrEmpty(hex) || hex.Length != 7 || hex[0] != '#')
        {
            Plugin.Log.LogError($"Invalid hex string: {hex}. Expected format is #RRGGBB.");
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
            Plugin.Log.LogError($"Error parsing hex string: {hex}. {e.Message}");
            return Color.white;
        }
    }
    
    private static Color GetRainbowColor(float offset = 0f, float hueSpeed = 0.4f)
    {
        offset = Mathf.Clamp01(offset);

        float hue = (Time.time * hueSpeed + offset) % 1.0f;

        return Color.HSVToRGB(hue, 1f, 1f);
    }
}