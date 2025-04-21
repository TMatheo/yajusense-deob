using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using yajusense.Core;
using yajusense.Extensions;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense.Modules.Visual;

public class ArrayList : BaseModule
{
    private const float MarginX = 5f;
    private const float AnimationSpeed = 10f;
    private const int FontSize = 22;
    private const float LineWidth = 2f;
    private const float OffScreenMargin = 2f;

    private readonly Dictionary<BaseModule, Vector2> _modulePositions = new();

    public ArrayList() : base("ArrayList", "Displays enabled modules", ModuleCategory.Visual, KeyCode.None, true)
    {
    }

    public override void OnGUI()
    {
        var modules = ModuleManager.GetModules()
            .OrderByDescending(m => IMGUIUtils.CalcTextSize(m.Name, FontSize).x)
            .ToList();
        var visibleModuleIndex = 0;

        for (var i = 0; i < modules.Count; i++)
        {
            var module = modules[i];

            var textSize = IMGUIUtils.CalcTextSize(module.Name, FontSize);
            var rectSize = new Vector2(textSize.x + MarginX * 2, textSize.y);

            var targetX = module.Enabled ? Screen.width - rectSize.x : Screen.width;
            var targetY = rectSize.y * visibleModuleIndex;
            var targetPosition = new Vector2(targetX, targetY);

            if (!_modulePositions.TryGetValue(module, out var currentPosition))
            {
                currentPosition = new Vector2(Screen.width, 20f);
                _modulePositions[module] = currentPosition;
            }

            _modulePositions[module] = Vector2.Lerp(currentPosition, targetPosition, AnimationSpeed * Time.deltaTime);

            var isAlmostOffScreen = _modulePositions[module].x >= Screen.width - OffScreenMargin;
            if (!module.Enabled && isAlmostOffScreen) continue;

            var currentRect = new Rect(_modulePositions[module], rectSize);

            var color = ColorUtils.GetRainbowColor(visibleModuleIndex * ModuleManager.ClientSettings.RainbowColorStep);

            var rectColor = color.Darken(0.2f);
            rectColor.a = 0.5f;
            Drawer.DrawFilledRect(currentRect, rectColor);

            Drawer.DrawVLine(currentRect.position, currentRect.height, LineWidth, color);

            var horizontalLineLength = currentRect.width;

            if (module.Enabled)
            {
                var nextEnabledModule = FindNextEnabledModule(modules, i + 1);

                if (nextEnabledModule != null && _modulePositions.TryGetValue(nextEnabledModule, out var nextPosition))
                    horizontalLineLength = nextPosition.x - currentRect.x;
            }

            var hLineStartPos = new Vector2(currentRect.x, currentRect.y + currentRect.height);
            Drawer.DrawHLine(hLineStartPos, horizontalLineLength, LineWidth, color);

            var textPosition = currentRect.position + new Vector2(MarginX, 0);
            Drawer.DrawText(module.Name, textPosition, color, FontSize, true);

            if (module.Enabled) visibleModuleIndex++;
        }
    }

    private BaseModule FindNextEnabledModule(List<BaseModule> modules, int startIndex)
    {
        for (var j = startIndex; j < modules.Count; j++)
            if (modules[j].Enabled)
                return modules[j];

        return null;
    }
}