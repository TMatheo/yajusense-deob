using UnityEngine;
using yajusense.UI.Utils;

namespace yajusense.UI;

public class Window
{
	private const float ResizeHandleSize = 20f;
	private readonly string _title;

	private Vector2 _dragOffset;
	private bool _isDragging;
	private bool _isResizing;
	private Rect _position;
	private Rect _resizeHandleRect;

	public Window(Rect position, string title = "")
	{
		_position = position;
		_title = title;
	}

	public void Begin()
	{
		HandleInputEvents();
		GUILayout.BeginArea(_position, GUI.skin.window);
		{
			if (!string.IsNullOrEmpty(_title))
			{
				GUILayout.Label(_title.Bold(), new GUIStyle(GUI.skin.label)
				{
					alignment = TextAnchor.MiddleCenter,
					fontSize = 18,
				});
			}

			GUILayout.BeginHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginVertical();
			GUILayout.Space(5);
		}
	}

	public void End()
	{
		GUILayout.EndVertical();
		GUILayout.Space(5);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void HandleInputEvents()
	{
		Event currentEvent = Event.current;
		Vector2 mousePos = currentEvent.mousePosition;

		_resizeHandleRect = new Rect(_position.x + _position.width - ResizeHandleSize, _position.y + _position.height - ResizeHandleSize, ResizeHandleSize, ResizeHandleSize);

		var titleBarRect = new Rect(_position.x, _position.y, _position.width, 20f);

		switch (currentEvent.type)
		{
			case EventType.MouseDown:
				if (GUIUtility.hotControl != 0)
					break;

				if (_resizeHandleRect.Contains(mousePos))
				{
					_isResizing = true;
					currentEvent.Use();
				}
				else if (titleBarRect.Contains(mousePos))
				{
					_isDragging = true;
					_dragOffset = mousePos - _position.position;
					currentEvent.Use();
				}

				break;

			case EventType.MouseUp:
				_isDragging = false;
				_isResizing = false;
				break;

			case EventType.MouseDrag:
				if (_isDragging)
				{
					_position.position = mousePos - _dragOffset;
					currentEvent.Use();
				}
				else if (_isResizing)
				{
					_position.size = new Vector2(Mathf.Max(mousePos.x - _position.x + ResizeHandleSize, 100f), Mathf.Max(mousePos.y - _position.y + ResizeHandleSize, 100f));
					currentEvent.Use();
				}

				break;
		}
	}
}