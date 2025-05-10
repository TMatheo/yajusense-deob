using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.SDKBase;
using yajusense.Core.Config;
using yajusense.Modules.Movement;
using yajusense.Modules.Other;
using yajusense.Modules.Photon;
using yajusense.Modules.Player;
using yajusense.Modules.Visual;
using yajusense.Modules.Visual.HUD;

namespace yajusense.Modules;

public static class ModuleManager
{
	private static readonly List<ModuleBase> Modules = new();
	public static ClientSettings ClientSettings { get; private set; }

	public static IEnumerable<ModuleBase> GetModules()
	{
		return Modules.AsReadOnly();
	}

	public static T GetModule<T>() where T : ModuleBase
	{
		return Modules.OfType<T>().FirstOrDefault();
	}

	public static void Init()
	{
		ClientSettings = new ClientSettings();
		ConfigManager.RegisterModuleConfig(ClientSettings);

		// Movement
		RegisterModule(new Flight());
		RegisterModule(new Speed());
		RegisterModule(new LagSwitch());
		RegisterModule(new ClickTP());

		// Visual
		RegisterModule(new Watermark());
		RegisterModule(new Information());
		RegisterModule(new ThirdPerson());
		RegisterModule(new UdonInspector());
		RegisterModule(new ArrayList());
		RegisterModule(new Menu());
		RegisterModule(new PlayerNotification());

		// Player
		RegisterModule(new HideSelf());
		
		// Photon
		RegisterModule(new Earrape());
	}

	private static void RegisterModule(ModuleBase module)
	{
		ConfigManager.RegisterModuleConfig(module);

		Modules.Add(module);
		Plugin.Log.LogInfo($"Module registered: {module.Name}");
	}

	public static void UpdateModules()
	{
		foreach (ModuleBase module in Modules)
		{
			if (module.Enabled)
				module.OnUpdate();

			if (module.ToggleKey != KeyCode.None && Input.GetKeyDown(module.ToggleKey))
				module.Toggle();
		}
	}

	public static void RenderModules()
	{
		foreach (ModuleBase module in Modules)
		{
			if (module.Enabled)
				module.OnGUI();
		}
	}

	public static void NotifyPlayerChanges(List<VRCPlayerApi> joinedPlayers, List<VRCPlayerApi> leftPlayers)
	{
		foreach (ModuleBase module in Modules)
		{
			if (!module.Enabled)
				continue;

			foreach (VRCPlayerApi player in joinedPlayers)
			{
				module.OnPlayerJoined(player);
			}

			foreach (VRCPlayerApi player in leftPlayers)
			{
				module.OnPlayerLeft(player);
			}
		}
	}

	public static void NotifyOpRaiseEvent(byte eventCode, ref Il2CppSystem.Object eventData)
	{
		foreach (ModuleBase module in Modules)
		{
			if (!module.Enabled)
				continue;

			module.OnOpRaiseEvent(eventCode, ref eventData);
		}
	}
}