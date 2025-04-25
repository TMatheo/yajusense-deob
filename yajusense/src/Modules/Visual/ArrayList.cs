using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using yajusense.Extensions;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense.Modules.Visual;

public class ArrayList : ModuleBase
{
    private const float MarginX = 5f;
    private const float AnimationSpeed = 10f;
    private const int FontSize = 22;
    private const float LineWidth = 2f;
    private const float OffScreenMargin = 2f;

    private readonly Dictionary<ModuleBase, Vector2> _modulePositions = new();

    public ArrayList() : base("ArrayList", "Displays enabled modules", ModuleCategory.Visual, KeyCode.None, true)
    {
    }

    public override void OnGUI()
    {
        List<ModuleBase> modules = ModuleManager.GetModules()
            .OrderByDescending(m => IMGUIUtils.CalcTextSize(m.Name, FontSize).x)
            .ToList();
        var visibleModuleIndex = 0;

        for (var i = 0; i < modules.Count; i++)
        {
            ModuleBase module = modules[i];

            Vector2 textSize = IMGUIUtils.CalcTextSize(module.Name, FontSize);
            var rectSize = new Vector2(textSize.x + MarginX * 2, textSize.y);

            float targetX = module.Enabled ? Screen.width - rectSize.x : Screen.width;
            float targetY = rectSize.y * visibleModuleIndex;
            var targetPosition = new Vector2(targetX, targetY);

            if (!_modulePositions.TryGetValue(module, out Vector2 currentPosition))
            {
                currentPosition = new Vector2(Screen.width, 20f);
                _modulePositions[module] = currentPosition;
            }

            _modulePositions[module] = Vector2.Lerp(currentPosition, targetPosition, AnimationSpeed * Time.deltaTime);

            bool isAlmostOffScreen = _modulePositions[module].x >= Screen.width - OffScreenMargin;
            if (!module.Enabled && isAlmostOffScreen) continue;

            var currentRect = new Rect(_modulePositions[module], rectSize);

            Color color = ColorUtils.GetRainbowColor(visibleModuleIndex * ModuleManager.ClientSettings.RainbowColorStep);

            Color rectColor = color.Darken(0.2f);
            rectColor.a = 0.5f;
            Drawer.DrawFilledRect(currentRect, rectColor);

            Drawer.DrawVLine(currentRect.position, currentRect.height, LineWidth, color);

            float horizontalLineLength = currentRect.width;

            if (module.Enabled)
            {
                ModuleBase nextEnabledModule = FindNextEnabledModule(modules, i + 1);

                if (nextEnabledModule != null && _modulePositions.TryGetValue(nextEnabledModule, out Vector2 nextPosition))
                    horizontalLineLength = nextPosition.x - currentRect.x;
            }

            var hLineStartPos = new Vector2(currentRect.x, currentRect.y + currentRect.height);
            Drawer.DrawHLine(hLineStartPos, horizontalLineLength, LineWidth, color);

            Vector2 textPosition = currentRect.position + new Vector2(MarginX, 0);
            Drawer.DrawText(module.Name, textPosition, color, FontSize, true);

            if (module.Enabled) visibleModuleIndex++;
        }
    }

    private ModuleBase FindNextEnabledModule(List<ModuleBase> modules, int startIndex)
    {
        for (int j = startIndex; j < modules.Count; j++)
            if (modules[j].Enabled)
                return modules[j];

        return null;
    }
}