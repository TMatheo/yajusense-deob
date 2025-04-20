using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using yajusense.Core;
using yajusense.Core.Config;
using yajusense.Modules.Movement;
using yajusense.Modules.Other;
using yajusense.Modules.Visual;
using yajusense.Modules.Visual.HUD;

namespace yajusense.Modules;

public static class ModuleManager
{
    private static readonly List<BaseModule> Modules = new();
    public static ClientSettings ClientSettings { get; private set; }

    public static IEnumerable<BaseModule> GetModules()
    {
        return Modules.AsReadOnly();
    }

    public static T GetModule<T>() where T : BaseModule
    {
        return Modules.OfType<T>().FirstOrDefault();
    }

    public static void Initialize()
    {
        ClientSettings = new ClientSettings();
        ConfigManager.RegisterModuleConfig(ClientSettings);

        // Movement
        RegisterModule(new Flight());
        RegisterModule(new ThirdPerson());

        // Visual
        RegisterModule(new Watermark());
        RegisterModule(new Information());
        RegisterModule(new UdonInspector());
        RegisterModule(new ArrayList());
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
            if (module.Enabled) module.OnUpdate();
            if (module.ToggleKey != KeyCode.None && Input.GetKeyDown(module.ToggleKey)) module.Toggle();
        }
    }

    public static void RenderModules()
    {
        foreach (var module in Modules)
            if (module.Enabled)
                module.OnGUI();
    }
}