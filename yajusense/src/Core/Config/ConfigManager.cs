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
            new UnityVector3JsonConverter()
        }
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
                _configData = new JsonObject();
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
                    _configData = new JsonObject();
                    TryCreateBackup("InvalidFormat");
                }
            }
        }
        catch (JsonException jsonEx)
        {
            Plugin.Log.LogError($"Config load error (JSON Parsing): {jsonEx.Message}. Resetting config.");
            _configData = new JsonObject();
            TryCreateBackup("CorruptedJson");
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Config load error (General): {ex}. Resetting config.");
            _configData = new JsonObject();
            TryCreateBackup("LoadError");
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
        Type type = module.GetType();
        List<PropertyInfo> properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ConfigAttribute>() != null)
            .ToList();

        var moduleConfigs = new List<ConfigProperty>();
        string moduleNodeName = module.Name;

        if (!_configData.TryGetPropertyValue(moduleNodeName, out JsonNode moduleNode) || !(moduleNode is JsonObject))
        {
            moduleNode = new JsonObject();
            _configData[moduleNodeName] = moduleNode;
        }

        var moduleSettings = (JsonObject)moduleNode;

        foreach (PropertyInfo prop in properties)
        {
            var configAttr = prop.GetCustomAttribute<ConfigAttribute>();
            string propName = prop.Name;
            object defaultValue = prop.GetValue(module);

            if (moduleSettings.TryGetPropertyValue(propName, out JsonNode savedValueNode) && savedValueNode != null)
            {
                try
                {
                    object typedValue = savedValueNode.Deserialize(prop.PropertyType, JsonOptions);
                    prop.SetValue(module, typedValue);
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogWarning(
                        $"Config value conversion failed for {moduleNodeName}.{propName}. Using default value. Error: {ex.Message}");
                    prop.SetValue(module, defaultValue);
                    try
                    {
                        moduleSettings[propName] = JsonSerializer.SerializeToNode(defaultValue, JsonOptions);
                    }
                    catch (Exception innerEx)
                    {
                        Plugin.Log.LogError(
                            $"Failed to serialize default value for {moduleNodeName}.{propName}: {innerEx.Message}");
                    }
                }
            }
            else
            {
                prop.SetValue(module, defaultValue);
                try
                {
                    moduleSettings[propName] = JsonSerializer.SerializeToNode(defaultValue, JsonOptions);
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError(
                        $"Failed to serialize default value for {moduleNodeName}.{propName}: {ex.Message}");
                }
            }

            moduleConfigs.Add(new ConfigProperty(prop, configAttr));
        }

        ModuleConfigProperties[module] = moduleConfigs;
    }

    public static void UpdatePropertyValue(ModuleBase module, string propertyName, object newValue)
    {
        string moduleNodeName = module.Name;
        if (_configData.TryGetPropertyValue(moduleNodeName, out JsonNode moduleNode) &&
            moduleNode is JsonObject moduleSettings)
            try
            {
                moduleSettings[propertyName] = JsonSerializer.SerializeToNode(newValue, JsonOptions);
                // SaveConfig(); // auto-save 
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError(
                    $"Failed to update property value for {moduleNodeName}.{propertyName}: {ex.Message}");
            }
    }

    public static bool TryGetConfigProperties(ModuleBase module, out List<ConfigProperty> properties)
    {
        return ModuleConfigProperties.TryGetValue(module, out properties);
    }

    private static void EnsureConfigFileExists()
    {
        try
        {
            if (!File.Exists(ConfigPath))
            {
                File.WriteAllText(ConfigPath, "{}");
                Plugin.Log.LogInfo($"Created new config file: {ConfigPath}");
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Config file/directory creation failed: {ex}");
            throw;
        }
    }

    private static void TryCreateBackup(string suffix = "Corrupted")
    {
        if (!File.Exists(ConfigPath)) return;

        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupPath = $"{ConfigPath}.{suffix}_{timestamp}.bak";
            File.Move(ConfigPath, backupPath);
            Plugin.Log.LogWarning($"Created backup of potentially corrupted config: {backupPath}");
        }
        catch (Exception backupEx)
        {
            Plugin.Log.LogError($"Backup creation failed: {backupEx}");
        }
    }

    private static void UpdateConfigDataFromModules()
    {
        foreach ((ModuleBase module, List<ConfigProperty> configProps) in ModuleConfigProperties)
        {
            string moduleNodeName = module.Name;
            if (!_configData.TryGetPropertyValue(moduleNodeName, out JsonNode moduleNode) || !(moduleNode is JsonObject))
            {
                moduleNode = new JsonObject();
                _configData[moduleNodeName] = moduleNode;
            }

            var moduleSettings = (JsonObject)moduleNode;

            foreach (ConfigProperty configProp in configProps)
                try
                {
                    object currentValue = configProp.Property.GetValue(module);
                    moduleSettings[configProp.Property.Name] =
                        JsonSerializer.SerializeToNode(currentValue, JsonOptions);
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError(
                        $"Failed to serialize property value for {moduleNodeName}.{configProp.Property.Name} during save: {ex.Message}");
                }
        }
    }
}