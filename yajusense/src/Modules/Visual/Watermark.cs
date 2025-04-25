using UnityEngine;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense.Modules.Visual.HUD;

public class Watermark : ModuleBase
{
    private const int FontSize = 60;
    private readonly Vector2 _pos = new(20f, 20f);

    public Watermark() : base("Watermark", "Shows watermark for this client", ModuleCategory.Visual, enabled: true)
    {
    }

    public override void OnGUI()
    {
        Drawer.DrawText("yaju", _pos, Color.white, FontSize, true);

        float yajuTextSizeX = IMGUIUtils.CalcTextSize("yaju", FontSize).x;
        Drawer.DrawRainbowText("sense", _pos + new Vector2(yajuTextSizeX, 0), FontSize, true);
    }
}