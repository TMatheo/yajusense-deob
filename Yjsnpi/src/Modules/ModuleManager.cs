using System.Collections.Generic;
using UnityEngine;
using Yjsnpi.Core;
using Yjsnpi.Core.Config;
using Yjsnpi.Modules.Visual;

namespace Yjsnpi.Modules;

public static class ModuleManager
{
    private static readonly List<BaseModule> Modules = new();
    
    public static IEnumerable<BaseModule> GetModules() => Modules.AsReadOnly();
    
    public static void Initialize()
    {
        RegisterModule(new Flight());
        
        RegisterModule(new UdonInspector());
        RegisterModule(new Menu());
    }
    
    private static void RegisterModule(BaseModule module)
    {
        if (module == null)
        {
            YjPlugin.Log.LogWarning("Attempted to register a null module");
            return;
        }
        
        ConfigManager.RegisterModuleConfig(module);
        
        Modules.Add(module);
        YjPlugin.Log.LogDebug($"Module registered: {module.Name}");
    }
    
    public static void UpdateModules()
    {
        foreach (var module in Modules)
        {
            if (module.Enabled)
            {
                module.OnUpdate();
            }
            if (Input.GetKeyDown(module.ToggleKey))
            {
                module.Toggle();
            }
        }
    }

    public static void RenderModules()
    {
        foreach (var module in Modules)
        {
            if (module.Enabled)
            {
                module.OnGUI();
            }
        }
    }
}