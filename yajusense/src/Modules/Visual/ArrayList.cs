using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using yajusense.Extensions;
using yajusense.UI;
using yajusense.UI.Utils;

namespace yajusense.Modules.Visual;

public class ArrayList : ModuleBase
{
	private const float MarginX = 5f;
	private const float AnimationSpeed = 10f;
	private const int FontSize = 22;
	private const float LineWidth = 2f;
	private const float OffScreenMargin = 2f;
	private readonly Dictionary<ModuleBase, Vector2> _modulePositions = new();
	private ulong _lastEnabledStatesHash;
	private int _lastModuleCount;

	private List<ModuleBase> _sortedModules = new();

	public ArrayList() : base("ArrayList", "Displays enabled modules", ModuleCategory.Visual, KeyCode.None, true) { }

	private void UpdateSortedModulesIfNeeded()
	{
		List<ModuleBase> currentModules = ModuleManager.GetModules().ToList();
		ulong currentStateHash = 0;
		foreach (ModuleBase mod in currentModules)
		{
			currentStateHash = (currentStateHash << 1) | (mod.Enabled ? 1UL : 0UL);
		}

		if (currentModules.Count != _lastModuleCount || currentStateHash != _lastEnabledStatesHash)
		{
			_sortedModules = currentModules.OrderByDescending(m => IMGUIUtils.CalcTextSize(m.Name, FontSize).x).ToList();
			_lastModuleCount = _sortedModules.Count;
			_lastEnabledStatesHash = currentStateHash;

			var currentModuleSet = new HashSet<ModuleBase>(_sortedModules);
			List<ModuleBase> modulesToRemove = _modulePositions.Keys.Where(m => !currentModuleSet.Contains(m)).ToList();
			foreach (ModuleBase modToRemove in modulesToRemove)
			{
				_modulePositions.Remove(modToRemove);
			}
		}
	}

	public override void OnGUI()
	{
		UpdateSortedModulesIfNeeded();

		var visibleModuleIndex = 0;

		for (var i = 0; i < _sortedModules.Count; i++)
		{
			ModuleBase module = _sortedModules[i];
			Vector2 textSize = IMGUIUtils.CalcTextSize(module.Name, FontSize);
			var rectSize = new Vector2(textSize.x + MarginX * 2, textSize.y);

			float targetX = module.Enabled ? Screen.width - rectSize.x : Screen.width;
			if (!_modulePositions.TryGetValue(module, out Vector2 currentPosition))
			{
				currentPosition = new Vector2(Screen.width, 20f + i * rectSize.y);
				_modulePositions[module] = currentPosition;
			}

			var targetPosition = new Vector2(targetX, currentPosition.y);

			_modulePositions[module] = Vector2.Lerp(currentPosition, targetPosition, AnimationSpeed * Time.deltaTime);


			if (module.Enabled)
			{
				float correctTargetY = rectSize.y * visibleModuleIndex;
				Vector2 currentPos = _modulePositions[module];

				currentPos.y = Mathf.Lerp(currentPos.y, correctTargetY, AnimationSpeed * Time.deltaTime);
				_modulePositions[module] = currentPos;

				visibleModuleIndex++;
			}

			bool isAlmostOffScreen = _modulePositions[module].x >= Screen.width - OffScreenMargin;
			if (!module.Enabled && isAlmostOffScreen)
				continue;

			var currentRect = new Rect(_modulePositions[module], rectSize);
			Color color = ColorUtils.GetRainbowColor(visibleModuleIndex * ModuleManager.ClientSettings.ColorStep);

			Color rectColor = color.Darken(0.2f);
			rectColor.a = 0.5f;
			Drawer.DrawFilledRect(currentRect, rectColor);

			Drawer.DrawVLine(currentRect.position, currentRect.height, LineWidth, color);
			float horizontalLineLength = currentRect.width;

			if (module.Enabled)
			{
				ModuleBase nextEnabledModule = FindNextEnabledModule(_sortedModules, i + 1);
				if (nextEnabledModule != null && _modulePositions.TryGetValue(nextEnabledModule, out Vector2 nextPosition))
				{
					horizontalLineLength = nextPosition.x - currentRect.x;
					if (horizontalLineLength < 0)
						horizontalLineLength = 0;
				}

				var hLineStartPos = new Vector2(currentRect.x, currentRect.y + currentRect.height);
				Drawer.DrawHLine(hLineStartPos, horizontalLineLength, LineWidth, color);
			}

			Vector2 textPosition = currentRect.position + new Vector2(MarginX, 0);
			Drawer.DrawText(module.Name, textPosition, color, FontSize, true);
		}
	}

	private ModuleBase FindNextEnabledModule(List<ModuleBase> modules, int startIndex)
	{
		for (int j = startIndex; j < modules.Count; j++)
		{
			if (modules[j].Enabled)
				return modules[j];
		}

		return null;
	}
}