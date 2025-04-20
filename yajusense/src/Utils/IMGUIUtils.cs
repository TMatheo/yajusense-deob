using UnityEngine;

namespace yajusense.Utils;

public static class IMGUIUtils
{
    public static Vector2 CalcTextSize(string text, int fontSize)
    {
        var content = new GUIContent(text.Size(fontSize));

        var style = new GUIStyle(GUI.skin.label);

        var width = style.CalcSize(content).x;
        var height = style.CalcHeight(content, width);

        return new Vector2(width, height);
    }
}