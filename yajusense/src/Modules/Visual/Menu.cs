using System;
using System.Collections;
using UnityEngine;
using yajusense.Core;
using yajusense.Core.Config;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense.Modules.Visual;

public class Menu : BaseModule
{
    private readonly Window _window;
    private ModuleCategory _selectedCategory = ModuleCategory.Visual;
    
    private const int FontSizeHeader = 14;
    private const int FontSize = 12;
    private readonly Color _descColor = new(0.5f, 0.5f, 0.5f);
    
    private bool _isDetectingKey;
    private ConfigProperty _keyDetectingProp;

    public Menu() : base("Menu", "Menu", ModuleCategory.Visual, KeyCode.Insert)
    {
        _window = new Window(new Rect(0, 0, 600f, 500f), "yajusense");
    }

    public override void OnEnable()
    {
        CursorUnlocker.UpdateCursorControl();
    }

    public override void OnDisable()
    {
        CursorUnlocker.UpdateCursorControl();
    }

    public override void OnGUI()
    {
        _window.Begin();
        {
            DrawTab();
            DrawModules();
            
            if (_selectedCategory == ModuleCategory.ClientSettings)
                DrawClientSettings();
            
            GUILayout.FlexibleSpace();
            DrawConfigControls();
        }
        _window.End();
    }

    private void DrawTab()
    {
        GUILayout.BeginHorizontal();
        {
            foreach (ModuleCategory category in Enum.GetValues(typeof(ModuleCategory)))
            {
                string text = category.ToString().Size(FontSizeHeader);

                if (category == ModuleCategory.ClientSettings)
                    text = "Settings".Size(FontSizeHeader);
                
                if (GUILayout.Button(category == _selectedCategory ? text.Bold() : text))
                {
                    _selectedCategory = category;
                }
            }
        }
        GUILayout.EndHorizontal();
    }

    private void DrawModules()
    {
        foreach (var module in ModuleManager.GetModules())
        {
            if (module.Category != _selectedCategory) 
                continue;
            
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(module.Name.Size(FontSizeHeader));
                    GUILayout.FlexibleSpace();
                    bool enabled = GUILayout.Toggle(module.Enabled, "Enabled");
                    if (enabled != module.Enabled)
                        module.Toggle();
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Label(module.Description.Size(FontSize).Color(_descColor));
                
                GUILayout.Space(10);
                DrawModuleConfig(module);
            }
            GUILayout.EndVertical();
        }
    }
    
    private void DrawModuleConfig(BaseModule module)
    {
        if (ConfigManager.TryGetConfigProperties(module, out var configProps))
        {
            foreach (var prop in configProps)
            {
                if (prop.Attribute.Hidden) 
                    continue;
                
                DrawModuleProperty(module, prop);
            }
        }
    }

    private void DrawModuleProperty(BaseModule module, ConfigProperty prop)
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(prop.Attribute.DisplayName.Size(FontSizeHeader));
                
            DrawPropertyField(module, prop);
        }
        GUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(prop.Attribute.Description))
        {
            GUILayout.Label(prop.Attribute.Description.Color(_descColor).Size(FontSize));
        }
    }
    
    private void DrawPropertyField(BaseModule module, ConfigProperty prop)
    {
        object currentValue = prop.Property.GetValue(module);
        object newValue = currentValue;

        switch (currentValue)
        {
            case bool b:
                newValue = GUILayout.Toggle(b, "");
                break;
                
            case float f:
                GUILayout.BeginHorizontal();
                newValue = GUILayout.HorizontalSlider(f, prop.Attribute.Min, prop.Attribute.Max);
                GUILayout.Label(f.ToString("F2"), GUILayout.Width(50));
                GUILayout.EndHorizontal();
                break;

            case int i:
                GUILayout.BeginHorizontal();
                newValue = (int)GUILayout.HorizontalSlider(i, prop.Attribute.Min, prop.Attribute.Max);
                GUILayout.Label(i.ToString(), GUILayout.Width(50));
                GUILayout.EndHorizontal();
                break;

            case KeyCode key:
                if (GUILayout.Button(_isDetectingKey && _keyDetectingProp == prop ? "Press any key..." : key.ToString()))
                {
                    if (!_isDetectingKey)
                    {
                        StartDetectKeyPress(module, prop);
                    }
                }
                break;
        }

        if (!Equals(newValue, currentValue) && !(currentValue is KeyCode))
        {
            prop.Property.SetValue(module, newValue);
            ConfigManager.UpdatePropertyValue(module, prop.Property.Name, newValue);
        }
    }
    
    private void StartDetectKeyPress(BaseModule module, ConfigProperty prop)
    {
        _keyDetectingProp = prop;
        _isDetectingKey = true;
        CoroutineRunner.StartManagedCoroutine(DetectKeyPressCoroutine(module, prop));
    }

    private IEnumerator DetectKeyPressCoroutine(BaseModule module, ConfigProperty prop)
    {
        while (_isDetectingKey)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _isDetectingKey = false;
                    _keyDetectingProp = null;
                    yield break;
                }

                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (key == KeyCode.None)
                        continue;

                    if (!Input.GetKeyDown(key))
                        continue;

                    prop.Property.SetValue(module, key);
                    ConfigManager.UpdatePropertyValue(module, prop.Property.Name, key);
                    
                    _isDetectingKey = false;
                    _keyDetectingProp = null;
                    yield break;
                }
            }
            yield return null;
        }
        _keyDetectingProp = null;
    }

    private void DrawClientSettings()
    {
        var module = ModuleManager.ClientSettings;
        
        if (ConfigManager.TryGetConfigProperties(module, out var configProps))
        {
            foreach (var prop in configProps)
            {
                if (prop.Attribute.Hidden || prop.Attribute.DisplayName == "Toggle Key") 
                    continue;
                
                DrawModuleProperty(module, prop);
            }
        }
    }
    
    private void DrawConfigControls()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Save Config"))
            {
                ConfigManager.SaveConfig();
            }

            if (GUILayout.Button("Load Config"))
            {
                ConfigManager.LoadConfig();
            }
        }
        GUILayout.EndHorizontal();
    }
}