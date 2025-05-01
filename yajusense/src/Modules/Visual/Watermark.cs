using UnityEngine;
using yajusense.Core.Config;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense.Modules.Visual.HUD;

public class Watermark : ModuleBase
{
    private const int FontSize = 60;
    private const float LineHeight = 5f;
    private readonly Vector2 _pos = new(20f, 20f);

    public Watermark() : base("Watermark", "Shows watermark for this client", ModuleCategory.Visual, enabled: true) { }

    [Config("Line Count", "Line Count", false, 1, 20)]
    public int LineCount { get; set; } = 10;

    public override void OnGUI()
    {
        Drawer.DrawText("yaju", _pos, Color.white, FontSize, true);

        float yajuTextSizeX = IMGUIUtils.CalcTextSize("yaju", FontSize).x;
        Drawer.DrawGradientText("sense", _pos + new Vector2(yajuTextSizeX, 0), FontSize, true);

        DrawLine();
    }

    private void DrawLine()
    {
        float segmentWidth = Screen.width / (float)LineCount;
        float colorStep = ModuleManager.ClientSettings.ColorStep;

        for (var i = 0; i < LineCount; i++)
        {
            float startX = i * segmentWidth;
            float colorOffset = i * colorStep;

            Color startColor = ColorUtils.GetRainbowColor(colorOffset);
            Color endColor = ColorUtils.GetRainbowColor(colorOffset + colorStep);

            Drawer.DrawGradientHLine(new Vector2(startX, 0), segmentWidth, LineHeight, startColor, endColor);
        }
    }
}