using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using yajusense.Modules;

namespace yajusense.Core.Config;

public class ConfigPropertyManager
{
	private readonly JsonSerializerOptions _jsonOptions;
	private readonly Dictionary<ModuleBase, List<ConfigProperty>> _moduleConfigProperties = new();

	public ConfigPropertyManager(JsonSerializerOptions jsonOptions)
	{
		_jsonOptions = jsonOptions;
	}

	public void RegisterModuleConfig(ModuleBase module, JsonObject configData)
	{
		Type moduleType = module.GetType();
		string moduleNodeName = module.Name;
		var moduleConfigs = new List<ConfigProperty>();

		if (!configData.TryGetPropertyValue(moduleNodeName, out JsonNode moduleNode) || moduleNode is not JsonObject)
		{
			moduleNode = new JsonObject();
			configData[moduleNodeName] = moduleNode;
		}

		var moduleSettings = (JsonObject)moduleNode;
		List<PropertyInfo> properties = GetConfigProperties(moduleType);

		foreach (PropertyInfo prop in properties)
		{
			ProcessProperty(module, prop, moduleSettings);
			moduleConfigs.Add(new ConfigProperty(prop, prop.GetCustomAttribute<ConfigAttribute>()!));
		}

		_moduleConfigProperties[module] = moduleConfigs;
	}

	public bool TryGetConfigProperties(ModuleBase module, out List<ConfigProperty> properties)
	{
		return _moduleConfigProperties.TryGetValue(module, out properties);
	}

	public void UpdatePropertyValue(ModuleBase module, string propertyName, object newValue, JsonObject configData)
	{
		string moduleNodeName = module.Name;
		if (configData.TryGetPropertyValue(moduleNodeName, out JsonNode moduleNode) && moduleNode is JsonObject moduleSettings)
			moduleSettings[propertyName] = JsonSerializer.SerializeToNode(newValue, _jsonOptions);
	}

	public void UpdateConfigDataFromModules(JsonObject configData)
	{
		foreach ((ModuleBase module, List<ConfigProperty> configProps) in _moduleConfigProperties)
		{
			string moduleNodeName = module.Name;

			if (!configData.TryGetPropertyValue(moduleNodeName, out JsonNode moduleNode) || moduleNode is not JsonObject)
			{
				moduleNode = new JsonObject();
				configData[moduleNodeName] = moduleNode;
			}

			var moduleSettings = (JsonObject)moduleNode;

			foreach (ConfigProperty configProp in configProps)
			{
				try
				{
					object currentValue = configProp.Property.GetValue(module);
					moduleSettings[configProp.Property.Name] = JsonSerializer.SerializeToNode(currentValue, _jsonOptions);
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError($"Failed to get/serialize property value for {moduleNodeName}.{configProp.Property.Name} during save: {ex.Message}");
				}
			}
		}
	}

	private static List<PropertyInfo> GetConfigProperties(Type moduleType)
	{
		return moduleType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttribute<ConfigAttribute>() != null).ToList();
	}

	private void ProcessProperty(ModuleBase module, PropertyInfo prop, JsonObject moduleSettings)
	{
		string propName = prop.Name;
		object defaultValue = prop.GetValue(module);

		try
		{
			if (moduleSettings.TryGetPropertyValue(propName, out JsonNode savedValueNode) && savedValueNode != null)
			{
				try
				{
					object typedValue = savedValueNode.Deserialize(prop.PropertyType, _jsonOptions)!;
					prop.SetValue(module, typedValue);
				}
				catch (Exception ex) when (ex is JsonException or ArgumentException or NotSupportedException)
				{
					Plugin.Log.LogWarning($"Config value deserialization/conversion failed for {module.Name}.{propName}. Using default. Error: {ex.Message}");
					SetPropertyToDefaultAndAddToJson(module, prop, defaultValue, moduleSettings);
				}
			}
			else
				SetPropertyToDefaultAndAddToJson(module, prop, defaultValue, moduleSettings);
		}
		catch (Exception ex)
		{
			Plugin.Log.LogError($"Unexpected error processing config for {module.Name}.{propName}: {ex.Message}");
			try
			{
				prop.SetValue(module, defaultValue);
			}
			catch
			{
				// ignored
			}
		}
	}

	private void SetPropertyToDefaultAndAddToJson(ModuleBase module, PropertyInfo prop, object defaultValue, JsonObject moduleSettings)
	{
		try
		{
			prop.SetValue(module, defaultValue);
			moduleSettings[prop.Name] = JsonSerializer.SerializeToNode(defaultValue, _jsonOptions);
		}
		catch (Exception ex)
		{
			Plugin.Log.LogError($"Failed to serialize or set default value for {module.Name}.{prop.Name}: {ex.Message}");
		}
	}
}