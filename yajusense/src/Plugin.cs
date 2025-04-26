using System;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using yajusense.Core;
using yajusense.Core.Config;
using yajusense.Core.Services;
using yajusense.Modules;
using yajusense.Patches;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense;

[BepInPlugin("yajusense", "yajusense", "1.0.0")]
public class Plugin : BasePlugin
{
    public new static ManualLogSource Log;
    public static readonly string ClientDirectory = Path.Combine(Directory.GetCurrentDirectory(), "yajusense");

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo("Initializing yajusense...");

        FileUtils.EnsureDirectoryExists(ClientDirectory);

        HarmonyPatcher.Initialize();

        OpRaiseEvent.ApplyPatch();
        NetworkManagerOnEvent.ApplyPatch();

        AudioService.Initialize();

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
    public YjMonoBehaviour(IntPtr handle) : base(handle) { }

    private void Update()
    {
        ModuleManager.UpdateModules();
        NetworkManagerOnEvent.OnUpdate();
    }

    private void OnGUI()
    {
        ModuleManager.RenderModules();
        NotificationManager.OnGUI();
    }
}