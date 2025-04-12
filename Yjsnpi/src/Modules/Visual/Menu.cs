using System;
using System.Collections;
using UnityEngine;
using Yjsnpi.Core;
using Yjsnpi.Core.Config;
using Yjsnpi.UI;
using Yjsnpi.Utils;

namespace Yjsnpi.Modules.Visual
{
    public class Menu : BaseModule
    {
        private Window _window;
        private Vector2 _scrollPos;
        private Vector2 _moduleSettingsScrollPos;
        private BaseModule _selectedModule;
        private bool _isDetectingKey = false;
        private ConfigProperty _detectingProp = null;

        public Menu() : base("Menu", "Provides configuration menu for all modules", ModuleType.Visual, KeyCode.Insert) 
        {
            _window = new Window(new Rect(0, 0, 300f, Screen.height - 40), "Yjsnpi");
        }

        public override void OnGUI()
        {
            _window.Begin();
            {
                _scrollPos = GUILayout.BeginScrollView(_scrollPos);
                {
                    DrawModules();
                    DrawSelectedModuleConfig();
                    GUILayout.Space(20);
                    DrawConfigSection();
                }
                GUILayout.EndScrollView();
            }
            _window.End();
        }

        private void DrawModules()
        {
            foreach (var module in ModuleManager.GetModules())
            {
                DrawModuleControls(module);
            }
        }

        private void DrawModuleControls(BaseModule module)
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                {
                    bool isSelected = _selectedModule == module;
                    
                    var formattedText = isSelected ? module.Name.Bold() : module.Name;
                    
                    if (GUILayout.Button(formattedText))
                    {
                        _selectedModule = isSelected ? null : module;
                    }
                    
                    string toggleText = module.Enabled ? 
                        "ON".Color(Color.green) : 
                        "OFF".Color(Color.red);

                    if (GUILayout.Button(toggleText, GUILayout.Width(120)))
                    {
                        module.Toggle();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Label(
                    module.Description.Italic().Color(Color.gray)
                );
            }
            GUILayout.EndVertical();
        }

        private void DrawSelectedModuleConfig()
        {
            if (_selectedModule == null) return;

            GUILayout.Space(10);
            
            GUILayout.Label(
                $"{_selectedModule.Name} Settings"
                    .Bold()
                    .Size(14)
            );

            _moduleSettingsScrollPos = GUILayout.BeginScrollView(_moduleSettingsScrollPos, GUILayout.Height(200));
            {
                if (ConfigManager.TryGetConfigProperties(_selectedModule, out var configProps))
                {
                    foreach (var prop in configProps)
                    {
                        if (prop.Attribute.Hidden) continue;
                        
                        DrawConfigProperty(prop);
                    }
                }
                else
                {
                    GUILayout.Label(
                        "No configurable properties found."
                            .Color(Colors.Orange)
                    );
                }
            }
            GUILayout.EndScrollView();
        }

        private void DrawConfigProperty(ConfigProperty prop)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(
                    prop.Attribute.DisplayName
                        .Bold(), 
                    GUILayout.Width(150)
                );
                
                DrawPropertyField(prop);
            }
            GUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(prop.Attribute.Description))
            {
                GUILayout.Label(
                    prop.Attribute.Description
                        .Italic()
                        .Color(Color.gray)
                );
            }
        }

        private void DrawPropertyField(ConfigProperty prop)
        {
            object currentValue = prop.Property.GetValue(_selectedModule);
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
                    if (GUILayout.Button(_isDetectingKey && _detectingProp == prop ? "Press any key...".Color(Color.yellow) : key.ToString()))
                    {
                        if (!_isDetectingKey)
                        {
                            StartDetectKeyPress(prop);
                        }
                    }
                    break;
                
                default:
                    GUILayout.Label($"Unsupported type: {currentValue.GetType().Name}");
                    break;
            }

            if (!Equals(newValue, currentValue) && !(currentValue is KeyCode))
            {
                try
                {
                    prop.Property.SetValue(_selectedModule, newValue);
                    ConfigManager.UpdatePropertyValue(_selectedModule, prop.Property.Name, newValue);
                }
                catch (Exception ex)
                {
                    YjPlugin.Log.LogError($"Failed to set property {prop.Property.Name}: {ex}");
                }
            }
        }
        
        private void StartDetectKeyPress(ConfigProperty prop)
        {
            _detectingProp = prop;
            _isDetectingKey = true;
            CoroutineRunner.StartCoroutine(DetectKeyPressCoroutine(prop));
        }

        private IEnumerator DetectKeyPressCoroutine(ConfigProperty prop)
        {
            while (_isDetectingKey)
            {
                if (Input.anyKeyDown)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        _isDetectingKey = false;
                        _detectingProp = null;
                        yield break;
                    }

                    foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    {
                        if (key == KeyCode.None || key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6) continue;

                        if (Input.GetKeyDown(key))
                        {
                            try
                            {
                                prop.Property.SetValue(_selectedModule, key);
                                ConfigManager.UpdatePropertyValue(_selectedModule, prop.Property.Name, key);
                                _isDetectingKey = false;
                                _detectingProp = null;
                                yield break;
                            }
                            catch (Exception ex)
                            {
                                YjPlugin.Log.LogError($"Failed to set KeyCode property {prop.Property.Name}: {ex}");
                                _isDetectingKey = false;
                                _detectingProp = null;
                                yield break;
                            }
                        }
                    }
                }
                yield return null;
            }
            _detectingProp = null;
        }

        private void DrawConfigSection()
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
}
