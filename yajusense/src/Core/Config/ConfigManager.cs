using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using yajusense.Core.Config.JsonConverters;
using yajusense.Modules;

namespace yajusense.Core.Config;

public static class ConfigManager
{
	private const string ConfigFileName = "ModuleConfig.cock";

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
	private static readonly Dictionary<ModuleBase, List<ConfigProperty>> ModuleConfigProperties = new();
	private static readonly string ConfigPath = Path.Combine(Plugin.ClientDirectory, ConfigFileName);

	public static void Initialize()
	{
		LoadConfig();
	}

	public static void LoadConfig()
	{
		try
		{
			EnsureConfigFileExists();
			string json = File.ReadAllText(ConfigPath);

			if (string.IsNullOrWhiteSpace(json))
			{
				ResetConfigAndBackup("EmptyFile");
				Plugin.Log.LogInfo("Config file was empty, starting with empty config.");
			}
			else
			{
				JsonNode parsedNode = JsonNode.Parse(json);
				if (parsedNode is JsonObject jsonObject)
				{
					_configData = jsonObject;
					Plugin.Log.LogInfo("Config loaded successfully.");
				}
				else
				{
					Plugin.Log.LogError("Config file root is not a JSON object. Resetting config.");
					ResetConfigAndBackup("InvalidFormat");
				}
			}
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
			UpdateConfigDataFromModules();
			string json = JsonSerializer.Serialize(_configData, JsonOptions);
			File.WriteAllText(ConfigPath, json);
			Plugin.Log.LogInfo("Config saved successfully.");
		}
		catch (Exception ex)
		{
			Plugin.Log.LogError($"Config save error: {ex}");
		}
	}

	public static void RegisterModuleConfig(ModuleBase module)
	{
		Type moduleType = module.GetType();
		string moduleNodeName = module.Name;
		var moduleConfigs = new List<ConfigProperty>();

		if (!_configData.TryGetPropertyValue(moduleNodeName, out JsonNode moduleNode) || moduleNode is not JsonObject)
		{
			moduleNode = new JsonObject();
			_configData[moduleNodeName] = moduleNode;
		}

		var moduleSettings = (JsonObject)moduleNode;

		List<PropertyInfo> properties = moduleType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttribute<ConfigAttribute>() != null).ToList();

		foreach (PropertyInfo prop in properties)
		{
			var configAttr = prop.GetCustomAttribute<ConfigAttribute>();
			string propName = prop.Name;
			object defaultValue = prop.GetValue(module);

			try
			{
				if (moduleSettings.TryGetPropertyValue(propName, out JsonNode savedValueNode) && savedValueNode != null)
				{
					try
					{
						object typedValue = savedValueNode.Deserialize(prop.PropertyType, JsonOptions);
						prop.SetValue(module, typedValue);
					}
					catch (Exception ex) when (ex is JsonException || ex is ArgumentException || ex is NotSupportedException)
					{
						Plugin.Log.LogWarning($"Config value deserialization/conversion failed for {moduleNodeName}.{propName}. Using default. Error: {ex.Message}");
						SetPropertyToDefaultAndAddToJson(module, prop, defaultValue, moduleSettings);
					}
				}
				else
					SetPropertyToDefaultAndAddToJson(module, prop, defaultValue, moduleSettings);
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError($"Unexpected error processing config for {moduleNodeName}.{propName}: {ex.Message}");
				try
				{
					prop.SetValue(module, defaultValue);
				}
				catch
				{
					// ignored
				}
			}

			moduleConfigs.Add(new ConfigProperty(prop, configAttr));
		}

		ModuleConfigProperties[module] = moduleConfigs;
	}

	public static void UpdatePropertyValue(ModuleBase module, string propertyName, object newValue)
	{
		string moduleNodeName = module.Name;
		if (_configData.TryGetPropertyValue(moduleNodeName, out JsonNode moduleNode) && moduleNode is JsonObject moduleSettings)
		{
			try
			{
				moduleSettings[propertyName] = JsonSerializer.SerializeToNode(newValue, JsonOptions);
				// SaveConfig(); // Consider uncommenting if auto-save on change is desired
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError($"Failed to update property value node for {moduleNodeName}.{propertyName}: {ex.Message}");
			}
		}
		else
			Plugin.Log.LogWarning($"Module node not found or not an object when updating property: {moduleNodeName}");
	}

	public static bool TryGetConfigProperties(ModuleBase module, out List<ConfigProperty> properties)
	{
		return ModuleConfigProperties.TryGetValue(module, out properties);
	}

	private static void SetPropertyToDefaultAndAddToJson(ModuleBase module, PropertyInfo prop, object defaultValue, JsonObject moduleSettings)
	{
		try
		{
			// Set default value in the module instance first
			prop.SetValue(module, defaultValue);
			// Then update the JSON node
			moduleSettings[prop.Name] = JsonSerializer.SerializeToNode(defaultValue, JsonOptions);
		}
		catch (Exception ex)
		{
			Plugin.Log.LogError($"Failed to serialize or set default value for {module.Name}.{prop.Name}: {ex.Message}");
		}
	}

	private static void EnsureConfigFileExists()
	{
		try
		{
			if (!File.Exists(ConfigPath))
			{
				string directory = Path.GetDirectoryName(ConfigPath);
				if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
					Directory.CreateDirectory(directory);

				File.WriteAllText(ConfigPath, "{}");
				Plugin.Log.LogInfo($"Created new config file: {ConfigPath}");
			}
		}
		catch (Exception ex)
		{
			Plugin.Log.LogError($"Config file/directory creation failed: {ex}");
			throw; // Rethrow to signal critical failure
		}
	}

	private static void ResetConfigAndBackup(string backupSuffix = "Backup")
	{
		_configData = new JsonObject(); // Reset in-memory config
		TryCreateBackup(backupSuffix);
		// Optionally immediately save the empty config: File.WriteAllText(ConfigPath, "{}");
	}

	private static void TryCreateBackup(string suffix = "Corrupted")
	{
		if (!File.Exists(ConfigPath))
			return;

		try
		{
			var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
			var backupPath = $"{ConfigPath}.{suffix}_{timestamp}.bak";
			File.Move(ConfigPath, backupPath); // Use Move for atomic operation if possible, fallback to Copy+Delete if needed
			Plugin.Log.LogWarning($"Created backup of potentially problematic config: {backupPath}");
		}
		catch (Exception backupEx)
		{
			Plugin.Log.LogError($"Backup creation failed: {backupEx}");
		}
	}

	private static void UpdateConfigDataFromModules()
	{
		foreach (KeyValuePair<ModuleBase, List<ConfigProperty>> kvp in ModuleConfigProperties)
		{
			ModuleBase module = kvp.Key;
			List<ConfigProperty> configProps = kvp.Value;
			string moduleNodeName = module.Name;

			if (!_configData.TryGetPropertyValue(moduleNodeName, out JsonNode moduleNode) || moduleNode is not JsonObject)
			{
				moduleNode = new JsonObject();
				_configData[moduleNodeName] = moduleNode;
			}

			var moduleSettings = (JsonObject)moduleNode;

			foreach (ConfigProperty configProp in configProps)
			{
				try
				{
					object currentValue = configProp.Property.GetValue(module);
					moduleSettings[configProp.Property.Name] = JsonSerializer.SerializeToNode(currentValue, JsonOptions);
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError($"Failed to get/serialize property value for {moduleNodeName}.{configProp.Property.Name} during save: {ex.Message}");
				}
			}
		}
	}
}