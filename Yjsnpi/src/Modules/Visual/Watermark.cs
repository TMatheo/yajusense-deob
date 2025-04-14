using UnityEngine;
using Yjsnpi.UI;
using Yjsnpi.Utils;

namespace Yjsnpi.Modules.Visual;

public class Watermark : BaseModule
{
    private const int FontSize = 60;
    private readonly Vector2 _pos = new Vector2(20f, 20f);
    
    public Watermark() : base("Watermark", "Shows watermark for this client", ModuleType.Visual, enabled: true) {}

    public override void OnGUI()
    {
        Drawer.DrawText("yaju", _pos, Color.white, FontSize, true);
        
        float textSize = IMGUIUtils.CalcTextSize("yaju", FontSize).x;
        Drawer.DrawRainbowText("sense", _pos + new Vector2(textSize, 0), FontSize, true);
    }
}