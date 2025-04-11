using System;
using System.Collections;
using UnityEngine;
using Yjsnpi.Core;
using Yjsnpi.Core.Config;
using Yjsnpi.UI;
using Yjsnpi.Utilities;

namespace Yjsnpi.Modules.Visual
{
    public class Menu : BaseModule
    {
        private Window _window;
        private Vector2 _scrollPos;
        private Vector2 _moduleSettingsScrollPos;
        private BaseModule _selectedModule;

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
                        "ON".Color(RichTextUtility.Colors.Green) : 
                        "OFF".Color(RichTextUtility.Colors.Red);

                    if (GUILayout.Button(toggleText, GUILayout.Width(120)))
                    {
                        module.Toggle();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Label(
                    module.Description.Italic().Color(RichTextUtility.Colors.Gray)
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
                        DrawConfigProperty(prop);
                    }
                }
                else
                {
                    GUILayout.Label(
                        "No configurable properties found."
                            .Color(RichTextUtility.Colors.Orange)
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
                        .Color(RichTextUtility.Colors.Gray)
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
                    newValue = GUILayout.HorizontalSlider(f, prop.Attribute.Min, prop.Attribute.Max);
                    break;

                case int i:
                    newValue = (int)GUILayout.HorizontalSlider(i, prop.Attribute.Min, prop.Attribute.Max);
                    break;

                case string s:
                    newValue = GUILayout.TextField(s);
                    break;

                case KeyCode key:
                    if (GUILayout.Button(key.ToString()))
                    {
                        CoroutineRunner.StartCoroutine(DetectKeyPress(prop));
                    }
                    break;
            }

            if (!Equals(newValue, currentValue))
            {
                prop.Property.SetValue(_selectedModule, newValue);
                ConfigManager.SaveConfig();
            }
        }
        
        private IEnumerator DetectKeyPress(ConfigProperty prop)
        {
            GUILayout.Button("<color=green>Press any key...</color>");

            while (!Input.anyKeyDown)
            {
                yield return null;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    prop.Property.SetValue(_selectedModule, key);
                    break;
                }
            }
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
