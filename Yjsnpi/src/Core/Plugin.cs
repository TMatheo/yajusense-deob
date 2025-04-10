using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using Yjsnpi.Core.Config;
using Yjsnpi.Modules;

namespace Yjsnpi.Core;

[BepInPlugin("yjsnpi", "Yjsnpi", "1.0.0")]
public class YjPlugin : BasePlugin
{
    public new static ManualLogSource Log;
    
    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo("Initializing Yjsnpi...");

        ConfigManager.Initialize(); 
        ModuleManager.Initialize();
        
        CoroutineRunner.Initialize(AddComponent<CoroutineRunner>());
        AddComponent<YjMonoBehaviour>();
    
        Log.LogInfo("Yjsnpi initialized successfully");
    }
}

public class YjMonoBehaviour : MonoBehaviour
{
    public YjMonoBehaviour(IntPtr handle) : base(handle) {}

    private void Update()
    {
        ModuleManager.UpdateModules();
    }

    private void OnGUI()
    {
        ModuleManager.RenderModules();
    }
}