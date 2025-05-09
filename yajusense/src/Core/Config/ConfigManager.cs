using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using yajusense.Core.Config.JsonConverters;
using yajusense.Modules;

namespace yajusense.Core.Config;

public static class ConfigManager
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		WriteIndented = true,
		Converters =
		{
			new ColorJsonConverter(),
			new KeyCodeJsonConverter(),
			new UnityVector2JsonConverter(),
			new UnityVector3JsonConverter(),
		},
	};

	private static JsonObject _configData = new();
	private static readonly IConfigStorage ConfigStorage = new FileConfigStorage(JsonOptions);
	private static readonly ConfigPropertyManager PropertyManager = new(JsonOptions);

	public static void Init()
	{
		LoadConfig();
	}

	public static void LoadConfig()
	{
		try
		{
			_configData = ConfigStorage.Load();
			Plugin.Log.LogInfo("Config loaded successfully.");
		}
		catch (JsonException jsonEx)
		{
			Plugin.Log.LogError($"Config load error (JSON Parsing): {jsonEx.Message}. Resetting config.");
			ResetConfigAndBackup("CorruptedJson");
		}
		catch (Exception ex)
		{
			Plugin.Log.LogError($"Config load error (General): {ex}. Resetting config.");
			ResetConfigAndBackup("LoadError");
		}
	}

	public static void SaveConfig()
	{
		try
		{
			PropertyManager.UpdateConfigDataFromModules(_configData);
			ConfigStorage.Save(_configData);
			Plugin.Log.LogInfo("Config saved successfully.");
		}
		catch (Exception ex)
		{
			Plugin.Log.LogError($"Config save error: {ex}");
		}
	}

	public static void RegisterModuleConfig(ModuleBase module)
	{
		PropertyManager.RegisterModuleConfig(module, _configData);
	}

	public static void UpdatePropertyValue(ModuleBase module, string propertyName, object newValue)
	{
		PropertyManager.UpdatePropertyValue(module, propertyName, newValue, _configData);
	}

	public static bool TryGetConfigProperties(ModuleBase module, out List<ConfigProperty> properties)
	{
		return PropertyManager.TryGetConfigProperties(module, out properties);
	}

	private static void ResetConfigAndBackup(string backupSuffix = "Backup")
	{
		_configData = new JsonObject();
		ConfigStorage.CreateBackup(backupSuffix);
	}
}