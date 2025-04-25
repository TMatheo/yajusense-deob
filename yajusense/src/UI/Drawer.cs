using UnityEngine;
using yajusense.Extensions;
using yajusense.Utils;

namespace yajusense.UI;

public static class Drawer
{
    private static Texture2D _whiteTexture;

    private static Texture2D WhiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    public static void DrawHLine(Vector2 start, float length, float thickness, Color color)
    {
        var rect = new Rect(start.x, start.y - thickness * 0.5f, length, thickness);
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawVLine(Vector2 start, float length, float thickness, Color color)
    {
        var rect = new Rect(start.x - thickness * 0.5f, start.y, thickness, length);
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawLine(Vector2 start, Vector2 end, float thickness, Color color)
    {
        Color oldColor = GUI.color;
        GUI.color = color;

        Vector2 dir = end - start;
        float length = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Matrix4x4 matrixBackup = GUI.matrix;
        GUIUtility.RotateAroundPivot(angle, start);

        var lineRect = new Rect(
            start.x,
            start.y - thickness * 0.5f,
            length,
            thickness
        );

        GUI.DrawTexture(lineRect, WhiteTexture);
        GUI.matrix = matrixBackup;

        GUI.color = oldColor;
    }

    public static void DrawRect(Rect rect, float thickness, Color color)
    {
        Color oldColor = GUI.color;
        GUI.color = color;

        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), WhiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), WhiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), WhiteTexture);
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), WhiteTexture);

        GUI.color = oldColor;
    }

    public static void DrawFilledRect(Rect rect, Color color)
    {
        Color oldColor = GUI.color;
        GUI.color = color;

        GUI.DrawTexture(rect, WhiteTexture);

        GUI.color = oldColor;
    }

    public static void DrawFilledGradientRect(Rect rect, Color color1, Color color2, bool horizontal = true)
    {
        Texture2D gradientTex = new(2, 2)
        {
            wrapMode = TextureWrapMode.Clamp
        };

        gradientTex.SetPixels(horizontal
            ? new[] { color1, color2, color1, color2 }
            : new[] { color1, color1, color2, color2 }
        );

        gradientTex.Apply();

        GUI.DrawTexture(rect, gradientTex);

        Object.DestroyImmediate(gradientTex);
    }

    public static void DrawText(string text, Vector2 position, Color color, int fontSize = 12, bool shadow = false, bool outline = false, bool bold = false)
    {
        if (string.IsNullOrEmpty(text)) return;

        text = text.Color(color);
        text = text.Size(fontSize);
        if (bold)
            text = text.Bold();

        Vector2 dummy = new(1000f, 1000f);

        if (shadow)
        {
            var offset = new Vector2(1f, 1f);

            string shadowText = text.Color(color.Darken(0.1f));

            GUI.Label(new Rect(position + offset, dummy), shadowText);
        }

        if (outline)
        {
            string outlineText = text.Color(Color.black);
            var offset = 2f;
            GUI.Label(new Rect(position + new Vector2(offset, offset), dummy), outlineText);
            GUI.Label(new Rect(position + new Vector2(-offset, offset), dummy), outlineText);
            GUI.Label(new Rect(position + new Vector2(-offset, -offset), dummy), outlineText);
            GUI.Label(new Rect(position + new Vector2(offset, -offset), dummy), outlineText);
        }

        GUI.Label(new Rect(position, dummy), text);
    }

    public static void DrawRainbowText(string text, Vector2 position, int fontSize = 12, bool shadow = false, float colorOffset = 0f)
    {
        if (string.IsNullOrEmpty(text)) return;

        float rainbowOffset = 1f / text.Length * 0.2f;

        var xOffset = 0.0f;
        var index = 0;
        foreach (char c in text)
        {
            var str = c.ToString();
            Color color = ColorUtils.GetRainbowColor((index + 1) * rainbowOffset + colorOffset);

            DrawText(str, position + new Vector2(xOffset, 0), color, fontSize, shadow);

            float xSize = IMGUIUtils.CalcTextSize(str, fontSize).x;
            xOffset += xSize;

            index++;
        }
    }
}