using UnityEngine;
using yajusense.Core.Config;
using yajusense.UI;

namespace yajusense.Modules.Visual.HUD;

public class HUDEditor : BaseModule
{
    private const float GridSize = 20f;
    private const float GridLineThickness = 1f;
    private const float GridLineAlpha = 0.2f;
    private const float SnapDistance = 10f;
    
    private BaseHUD _selectedHUD;
    private bool _isDragging;
    private Vector2 _dragOffset;
    
    public HUDEditor() : base("HUD Editor", "Edit HUD positions visually", ModuleType.Visual) {}

    public override void OnDisable()
    {
        _selectedHUD = null;
        _isDragging = false;
    }

    public override void OnGUI()
    {
        if (!Enabled) return;

        // DrawGrid();
        HandleInputEvents();
        DrawHUDOutlines();
        DrawSelectedHUDInfo();
    }

    private void DrawGrid()
    {
        Color gridColor = new Color(1f, 1f, 1f, GridLineAlpha);
        
        for (float x = 0; x < Screen.width; x += GridSize)
        {
            Drawer.DrawVLine(new Vector2(x, 0), Screen.height, GridLineThickness, gridColor);
        }
        
        for (float y = 0; y < Screen.height; y += GridSize)
        {
            Drawer.DrawHLine(new Vector2(0, y), Screen.width, GridLineThickness, gridColor);
        }
    }

    private void HandleInputEvents()
    {
        Event current = Event.current;
        Vector2 mousePos = current.mousePosition;

        switch (current.type)
        {
            case EventType.MouseDown when current.button == 0:
                HandleMouseDown(mousePos);
                break;
            
            case EventType.MouseUp when current.button == 0:
                _isDragging = false;
                if (_selectedHUD != null)
                {
                    ConfigManager.UpdatePropertyValue(_selectedHUD, nameof(BaseHUD.Position), _selectedHUD.Position);
                }
                break;
            
            case EventType.MouseDrag when current.button == 0 && _isDragging:
                HandleMouseDrag(mousePos);
                break;
            
            case EventType.KeyDown when current.keyCode == KeyCode.Escape:
                _selectedHUD = null;
                _isDragging = false;
                break;
        }
    }

    private void HandleMouseDown(Vector2 mousePos)
    {
        _selectedHUD = null;
        
        foreach (var module in ModuleManager.GetModules())
        {
            if (module is BaseHUD hud && hud.Enabled)
            {
                var hudRect = GetHUDRect(hud);
                if (hudRect.Contains(mousePos))
                {
                    _selectedHUD = hud;
                    _isDragging = true;
                    _dragOffset = mousePos - hud.Position;
                    break;
                }
            }
        }
    }

    private void HandleMouseDrag(Vector2 mousePos)
    {
        if (_selectedHUD == null || !_isDragging) return;
        
        Vector2 newPosition = mousePos - _dragOffset;
        
        float snappedX = Mathf.Round(newPosition.x / GridSize) * GridSize;
        if (Mathf.Abs(newPosition.x - snappedX) < SnapDistance)
        {
            newPosition.x = snappedX;
        }
    
        float snappedY = Mathf.Round(newPosition.y / GridSize) * GridSize;
        if (Mathf.Abs(newPosition.y - snappedY) < SnapDistance)
        {
            newPosition.y = snappedY;
        }
        
        newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width - _selectedHUD.Size.x);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height - _selectedHUD.Size.y);
    
        _selectedHUD.Position = newPosition;
    }

    private void DrawHUDOutlines()
    {
        foreach (var module in ModuleManager.GetModules())
        {
            if (module is BaseHUD hud && hud.Enabled)
            {
                var hudRect = GetHUDRect(hud);
                bool isSelected = hud == _selectedHUD;
                
                Color outlineColor = isSelected ? Color.green : Color.yellow;
                outlineColor.a = isSelected ? 0.8f : 0.5f;
                
                Drawer.DrawRect(hudRect, 2f, outlineColor);
                
                if (isSelected)
                {
                    Drawer.DrawText(
                        hud.Name,
                        hud.Position + new Vector2(0, -20),
                        Color.white,
                        12,
                        true
                    );
                }
            }
        }
    }

    private void DrawSelectedHUDInfo()
    {
        if (_selectedHUD == null) return;

        string infoText = $"{_selectedHUD.Name}\n" +
                         $"Position: {_selectedHUD.Position.x:F0}, {_selectedHUD.Position.y:F0}";
        
        Vector2 infoPos = new Vector2(20, Screen.height - 100);
        
        Drawer.DrawFilledRect(
            new Rect(infoPos.x - 10, infoPos.y - 10, 200, 60),
            new Color(0f, 0f, 0f, 0.5f)
        );
        
        Drawer.DrawText(
            infoText,
            infoPos,
            Color.white,
            14,
            true
        );
    }

    private Rect GetHUDRect(BaseHUD hud)
    {
        return new Rect(hud.Position, hud.Size);
    }
}