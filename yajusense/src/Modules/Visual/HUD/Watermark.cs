using UnityEngine;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense.Modules.Visual.HUD;

public class Watermark : BaseHUD
{
    private const int FontSize = 60;
    private bool _initialized = false;

    public Watermark() : base("Watermark", "Shows watermark for this client", new Vector2(20f, 20f), 
        new Vector2(0, 0), true) {}

    public override void OnGUI()
    {
        if (!_initialized)
        {
            Size = IMGUIUtils.CalcTextSize("yajusense", FontSize);
            _initialized = true;
        }
        
        Drawer.DrawText("yaju", Position, Color.white, FontSize, true);
        
        float yajuTextSizeX = IMGUIUtils.CalcTextSize("yaju", FontSize).x;
        Drawer.DrawRainbowText("sense", Position + new Vector2(yajuTextSizeX, 0), FontSize, true);
    }
}