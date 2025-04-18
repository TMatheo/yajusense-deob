using UnityEngine;
using yajusense.Utils;

namespace yajusense.Modules.Visual.HUD;

public class ArrayList : BaseHUD
{
    private const float MarginX = 10f;
    private const float MarginY = 5f;
    
    public ArrayList() : base("ArrayList", "", new Vector2(Screen.width - 20f, Screen.height - 20f), new Vector2(0, 0), true) {}

    public override void OnGUI()
    {
        int index = 0;
        foreach (var module in ModuleManager.GetModules())
        {
            Vector2 textSize = IMGUIUtils.CalcTextSize(module.Name, 14);
            
            Vector2 baseRectSize = new Vector2(textSize.x + MarginX * 2, textSize.y + MarginY * 2);
            Vector2 baseRectPos = new Vector2(Screen.width - baseRectSize.x, baseRectSize.y * index);
            Rect baseRect = new Rect(baseRectPos, baseRectSize);
            
            
        }
    }
}