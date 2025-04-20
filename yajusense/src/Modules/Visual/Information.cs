using System;
using System.Collections.Generic;
using UnityEngine;
using yajusense.UI;
using yajusense.Utils;
// Add this using directive

// Add this using directive for Func

namespace yajusense.Modules.Visual.HUD;

public class Information : BaseModule
{
    private const int FontSize = 22;

    private readonly List<TextInfo> _textInfos = new();
    private float _deltaTime;

    public Information() : base("Information", "Shows fps counter and position", ModuleCategory.Visual, enabled: true)
    {
        _textInfos.Add(new TextInfo(() => $"FPS: {Mathf.RoundToInt(1.0f / _deltaTime)}"
        ));

        _textInfos.Add(new TextInfo(() => "XYZ: Fetching..."
        ));
    }

    public override void OnGUI()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

        var positionText = "XYZ: N/A";
        if (VRCUtils.IsInWorld())
        {
            var position = VRCUtils.GetLocalVRCPlayerApi().gameObject.transform.position;
            positionText = $"XYZ: {position.x:F0}, {position.y:F0}, {position.z:F0}";
        }

        var currentY = Screen.height - 10f;
        for (var i = 0; i < _textInfos.Count; i++)
        {
            var info = _textInfos[i];

            var text = i == 1 ? positionText : info.GetText();

            var textSize = IMGUIUtils.CalcTextSize(text, FontSize);
            currentY -= textSize.y;
            var textPos = new Vector2(10f, currentY);
            var color = ColorUtils.GetRainbowColor(i * ModuleManager.ClientSettings.RainbowColorStep);

            Drawer.DrawText(text, textPos, color, FontSize, true);
        }
    }

    private record TextInfo(Func<string> GetText);
}