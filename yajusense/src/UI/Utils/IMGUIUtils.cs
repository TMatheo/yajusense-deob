using UnityEngine;

namespace yajusense.UI.Utils;

public static class IMGUIUtils
{
	public static Vector2 CalcTextSize(string text, int fontSize)
	{
		var content = new GUIContent(text.Size(fontSize));

		var style = new GUIStyle(GUI.skin.label);

		float width = style.CalcSize(content).x;
		float height = style.CalcHeight(content, width);

		return new Vector2(width, height);
	}
}