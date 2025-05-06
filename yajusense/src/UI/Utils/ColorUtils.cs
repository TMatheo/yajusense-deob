using System;
using UnityEngine;

namespace yajusense.UI.Utils;

public static class ColorUtils
{
	public static Color GetRainbowColor(float offset = 0f, float hueSpeed = 0.4f)
	{
		float hue = (Time.time * hueSpeed + offset) % 1.0f;

		if (hue < 0)
			hue += 1.0f;

		return Color.HSVToRGB(hue, 1f, 1f);
	}

	public static Color FromHex(string hex)
	{
		if (string.IsNullOrEmpty(hex))
			throw new ArgumentException("Hex string cannot be null or empty");

		if (!hex.StartsWith("#") || (hex.Length != 7 && hex.Length != 9))
			throw new FormatException("Invalid color format. Expected #RRGGBB or #RRGGBBAA");

		try
		{
			var r = Convert.ToByte(hex.Substring(1, 2), 16);
			var g = Convert.ToByte(hex.Substring(3, 2), 16);
			var b = Convert.ToByte(hex.Substring(5, 2), 16);
			byte a = hex.Length == 9 ? Convert.ToByte(hex.Substring(7, 2), 16) : (byte)255;

			return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
		}
		catch (Exception ex)
		{
			throw new FormatException($"Failed to parse color from hex: {hex}", ex);
		}
	}

	public static string ToHex(Color color, bool includeAlpha = false)
	{
		var r = (byte)(Mathf.Clamp01(color.r) * 255);
		var g = (byte)(Mathf.Clamp01(color.g) * 255);
		var b = (byte)(Mathf.Clamp01(color.b) * 255);

		if (includeAlpha || color.a < 1.0f)
		{
			var a = (byte)(Mathf.Clamp01(color.a) * 255);
			return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
		}

		return $"#{r:X2}{g:X2}{b:X2}";
	}
}