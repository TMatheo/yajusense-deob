using System.IO;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using yajusense.Core;
using yajusense.Core.Config;
using yajusense.Core.Utils;
using yajusense.Modules;
using yajusense.Platform.Unity.Utils;
using yajusense.Services;

namespace yajusense;

[BepInPlugin("yajusense", "yajusense", "1.0.0")]
public class Plugin : BasePlugin
{
	public new static ManualLogSource Log;
	public static readonly string ClientDirectory = Path.Combine(Directory.GetCurrentDirectory(), "yajusense");
	private Harmony _harmony;

	public override void Load()
	{
		Log = base.Log;
		Log.LogInfo("Initializing yajusense...");

		FileUtils.EnsureDirectoryExists(ClientDirectory);

		CursorUnlocker.Init();

		AudioService.Init();

		ConfigManager.Init();

		ModuleManager.Init();

		_harmony = new Harmony("yajusense");
		Log.LogInfo("Harmony initialized successfully");

		_harmony.PatchAll();
		Log.LogInfo("Patches applied successfully");

		CoroutineRunner.Initialize(AddComponent<CoroutineRunner>());
		AddComponent<MainMonoBehaviour>();

		Log.LogInfo("yajusense initialized successfully");
	}
}