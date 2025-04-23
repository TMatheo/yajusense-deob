using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using yajusense.Core.Config;
using yajusense.Modules;
using yajusense.Patches;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense.Core;

[BepInPlugin("yajusense", "yajusense", "1.0.0")]
public class YjPlugin : BasePlugin
{
    public new static ManualLogSource Log;

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo("Initializing yajusense...");

        HarmonyPatcher.Initialize();

        OpRaiseEventPatch.ApplyPatch();
        NetworkManagerOnEventPatch.ApplyPatch();

        ConfigManager.Initialize();

        ModuleManager.Initialize();

        CoroutineRunner.Initialize(AddComponent<CoroutineRunner>());
        AddComponent<YjMonoBehaviour>();

        CursorUnlocker.Init();

        Log.LogInfo("yajusense initialized successfully");
    }
}

public class YjMonoBehaviour : MonoBehaviour
{
    public YjMonoBehaviour(IntPtr handle) : base(handle)
    {
    }

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