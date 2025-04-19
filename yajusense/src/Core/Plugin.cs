using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using yajusense.Core.Config;
using yajusense.Modules;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense.Core;

[BepInPlugin("yjsnpi", "yajusense", "1.0.0")]
public class YjPlugin : BasePlugin
{
    public new static ManualLogSource Log;
    
    private static Harmony _harmonyInstance;
    
    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo("Initializing yajusense...");
        
        _harmonyInstance = new Harmony("yajusense");
        Log.LogInfo("Harmony instance created.");

        ConfigManager.Initialize(); 
        Log.LogInfo("Config Manager initialized.");
        
        ModuleManager.Initialize();
        Log.LogInfo("Module Manager initialized.");
        
        CoroutineRunner.Initialize(AddComponent<CoroutineRunner>());
        AddComponent<YjMonoBehaviour>();
        
        CursorUnlocker.Init(_harmonyInstance);
    
        Log.LogInfo("yajusense initialized successfully");
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
        NotificationManager.OnGUI();
    }
}