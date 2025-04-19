using UnityEngine;
using yajusense.UI;
using yajusense.Utils;
using System.Collections.Generic; // Add this using directive
using System; // Add this using directive for Func

namespace yajusense.Modules.Visual.HUD;

public class Information : BaseModule
{
    private float _deltaTime;
    private const int FontSize = 22;
    
    private record TextInfo(Func<string> GetText);

    private readonly List<TextInfo> _textInfos = new();

    public Information() : base("Information", "Shows fps counter and position", ModuleCategory.Visual, enabled: true)
    {
        _textInfos.Add(new TextInfo(
            () => $"FPS: {Mathf.RoundToInt(1.0f / _deltaTime)}"
        ));
        
        _textInfos.Add(new TextInfo(
            () => "XYZ: Fetching..."
        ));
    }

    public override void OnGUI()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        
        string positionText = "XYZ: N/A";
        VRCUtils.SafeExecuteInWorld(() =>
        {
            Vector3 position = VRCUtils.GetLocalVRCPlayerApi().gameObject.transform.position;
            positionText = $"XYZ: {position.x:F0}, {position.y:F0}, {position.z:F0}";
        });

        float currentY = Screen.height - 10f;
        for (int i = 0; i < _textInfos.Count; i++)
        {
            TextInfo info = _textInfos[i];

            string text = i == 1 ? positionText : info.GetText();

            Vector2 textSize = IMGUIUtils.CalcTextSize(text, FontSize);
            currentY -= textSize.y;
            Vector2 textPos = new Vector2(10f, currentY);
            Color color = ColorUtils.GetRainbowColor(i * ModuleManager.ClientSettings.RainbowColorStep);

            Drawer.DrawText(text, textPos, color, FontSize, true);
        }
    }
}