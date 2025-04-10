using UnityEngine;
using Yjsnpi.Core.Config;
using Yjsnpi.UI;

namespace Yjsnpi.Modules.Visual;

public class Menu : BaseModule
{
    private Window _window;
    private Vector2 _scrollPos;

    public Menu() : base("Menu", "Provides configuration menu for all modules", ModuleType.Visual, KeyCode.Insert) 
    {
        _window = new(new(0, 0, 600f, 500f), "Yjsnpi");
    }

    public override void OnRender()
    {
        _window.Begin();
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            {
                DrawModules();
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

        GUILayout.Space(20);
    }

    private void DrawModuleControls(BaseModule module)
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(module.Name);
                if (GUILayout.Button(module.Enabled ? "OFF" : "ON", GUILayout.Width(120)))
                {
                    module.Toggle();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label(module.Description);
        }
        GUILayout.EndVertical();
    }

    private void DrawConfigSection()
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
}
