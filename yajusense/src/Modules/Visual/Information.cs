using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using yajusense.Core;
using yajusense.UI;
using yajusense.Utils;
using yajusense.VRC;

namespace yajusense.Modules.Visual;

public class Information : ModuleBase
{
	private const int FontSize = 22;
	private readonly StringBuilder _stringBuilder = new(32);
	private readonly List<TextInfo> _textInfos = new();
	private float _deltaTime;

	public Information() : base("Information", "Shows fps counter and position", ModuleCategory.Visual, enabled: true)
	{
		_textInfos.Add(new TextInfo(() =>
		{
			_stringBuilder.Clear();
			_stringBuilder.Append("FPS: ");
			_stringBuilder.Append(Mathf.RoundToInt(1.0f / _deltaTime));
			return _stringBuilder.ToString();
		}));
		_textInfos.Add(new TextInfo(() =>
		{
			_stringBuilder.Clear();
			if (VRCHelper.IsInWorld())
			{
				Vector3 position = VRCHelper.GetLocalVRCPlayerApi().gameObject.transform.position;
				_stringBuilder.Append($"XYZ: {position.x:F0}, {position.y:F0}, {position.z:F0}");
			}
			else
				_stringBuilder.Append("XYZ: N/A");

			return _stringBuilder.ToString();
		}));
	}

	public override void OnGUI()
	{
		_deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

		float currentY = Screen.height - 10f;
		for (var i = 0; i < _textInfos.Count; i++)
		{
			TextInfo info = _textInfos[i];
			string text = info.GetText();

			Vector2 textSize = IMGUIUtils.CalcTextSize(text, FontSize);
			currentY -= textSize.y;
			var textPos = new Vector2(10f, currentY);
			Color color = ColorUtils.GetRainbowColor(i * ModuleManager.ClientSettings.ColorStep);

			Drawer.DrawText(text, textPos, color, FontSize, true);
		}
	}

	private record TextInfo(Func<string> GetText);
}