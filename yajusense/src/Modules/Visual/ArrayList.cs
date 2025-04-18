using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using yajusense.Extensions;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense.Modules.Visual;

public class ArrayList : BaseModule
{
    private const float MarginX = 5f;
    private const float AnimationSpeed = 8f;
    private const int FontSize = 22;
    private const float LineWidth = 2f;
    private const float RainbowColorStep = 0.03f;
    private const float OffScreenMargin = 2f;
    
    public ArrayList() : base("ArrayList", "Displays enabled modules", ModuleType.Visual, KeyCode.None, true) {}
    
    public override void OnGUI()
    {
        List<BaseModule> modules = ModuleManager.GetModules()
            .OrderByDescending(m => IMGUIUtils.CalcTextSize(m.Name, FontSize).x)
            .ToList();
        int visibleModuleIndex = 0;
        
        for (int i = 0; i < modules.Count; i++)
        {
            BaseModule module = modules[i];
            
            Vector2 textSize = IMGUIUtils.CalcTextSize(module.Name, FontSize);
            Vector2 rectSize = new Vector2(textSize.x + MarginX * 2, textSize.y);
            
            float targetX = module.Enabled ? Screen.width - rectSize.x : Screen.width;
            float targetY = rectSize.y * visibleModuleIndex;
            Vector2 targetPosition = new Vector2(targetX, targetY);
            
            
            module.ArrayListPosition = Vector2.Lerp(module.ArrayListPosition, targetPosition, AnimationSpeed * Time.deltaTime);
            
            bool isAlmostOffScreen = module.ArrayListPosition.x >= Screen.width - OffScreenMargin;
            if (!module.Enabled && isAlmostOffScreen)
            {
                continue;
            }
            
            Rect currentRect = new Rect(module.ArrayListPosition, rectSize);
            
            Color color = ColorUtils.GetRainbowColor(visibleModuleIndex * RainbowColorStep);
            
            Color rectColor = color.Darken(0.2f);
            rectColor.a = 0.5f;
            Drawer.DrawFilledRect(currentRect, rectColor);
            
            Drawer.DrawVLine(currentRect.position, currentRect.height, LineWidth, color);
            
            float horizontalLineLength = currentRect.width;

            if (module.Enabled)
            {
                BaseModule nextEnabledModule = FindNextEnabledModule(modules, i + 1);

                if (nextEnabledModule != null)
                {
                    horizontalLineLength = nextEnabledModule.ArrayListPosition.x - currentRect.x;
                }
            }
            
            Vector2 hLineStartPos = new Vector2(currentRect.x, currentRect.y + currentRect.height);
            Drawer.DrawHLine(hLineStartPos, horizontalLineLength, LineWidth, color);
            
            Vector2 textPosition = currentRect.position + new Vector2(MarginX, 0);
            Drawer.DrawText(module.Name, textPosition, color, FontSize, true);
            
            if (module.Enabled)
            {
                visibleModuleIndex++;
            }
        }
    }
    
    private BaseModule FindNextEnabledModule(List<BaseModule> modules, int startIndex)
    {
        for (int j = startIndex; j < modules.Count; j++)
        {
            if (modules[j].Enabled)
            {
                return modules[j];
            }
        }
        return null;
    }
}