using UnityEngine;
using yajusense.Core.Config;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense.Modules.Visual;

public class MLGMode : ModuleBase
{
    private const float LineHeight = 5f;

    public MLGMode() : base("MLGMode", "Enables MLG mode", ModuleCategory.Visual, enabled: true)
    {
    }

    [Config("Line Count", "Line Count", false, 1, 20)]
    public int LineCount { get; set; } = 10;

    public override void OnGUI()
    {
        float segmentWidth = Screen.width / (float)LineCount;
        float colorStep = ModuleManager.ClientSettings.RainbowColorStep;

        for (var i = 0; i < LineCount; i++)
        {
            float startX = i * segmentWidth;
            float colorOffset = i * colorStep;

            Color startColor = ColorUtils.GetRainbowColor(colorOffset);
            Color endColor = ColorUtils.GetRainbowColor(colorOffset + colorStep);

            Drawer.DrawGradientHLine(
                new Vector2(startX, 0),
                segmentWidth,
                LineHeight,
                startColor,
                endColor
            );
        }
    }
}