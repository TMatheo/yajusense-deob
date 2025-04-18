using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using yajusense.Core.Config.JsonConverters;
using yajusense.Modules;
using yajusense.Utils;

namespace yajusense.Core.Config;

public static class ConfigManager
{
    private const string ConfigDirectoryName = "yajusense";
    private const string ConfigFileName = "ModuleConfig.cock";
    private static string ConfigDirectory => Path.Combine(Directory.GetCurrentDirectory(), ConfigDirectoryName);
    private static string ConfigPath => Path.Combine(ConfigDirectory, ConfigFileName);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new ColorJsonConverter(),
            new KeyCodeJsonConverter(),
            new UnityVector2JsonConverter(),
            new UnityVector3JsonConverter()
        },
    };

    private static JsonObject _configData = new();
    private static readonly Dictionary<BaseModule, List<ConfigProperty>> ModuleConfigProperties = new();

    public static void Initialize()
    {
        LoadConfig();
    }

    public static void LoadConfig()
    {
        try
        {
            EnsureConfigFileExists();

            var json = File.ReadAllText(ConfigPath);

            if (string.IsNullOrWhiteSpace(json))
            {
                _configData = new JsonObject();
                YjPlugin.Log.LogInfo("Config file was empty, starting with empty config.");
            }
            else
            {
                var parsedNode = JsonNode.Parse(json);
                if (parsedNode is JsonObject jsonObject)
                {
                    _configData = jsonObject;
                    YjPlugin.Log.LogInfo("Config loaded successfully.");
                }
                else
                {
                    YjPlugin.Log.LogError("Config file root is not a JSON object. Resetting config.");
                    _configData = new JsonObject();
                    TryCreateBackup("InvalidFormat");
                }
            }
        }
        catch (JsonException jsonEx)
        {
            YjPlugin.Log.LogError($"Config load error (JSON Parsing): {jsonEx.Message}. Resetting config.");
            _configData = new JsonObject();
            TryCreateBackup("CorruptedJson");
        }
        catch (Exception ex)
        {
            YjPlugin.Log.LogError($"Config load error (General): {ex}. Resetting config.");
            _configData = new JsonObject();
            TryCreateBackup("LoadError");
        }
    }

    public static void SaveConfig()
    {
        try
        {
            UpdateConfigDataFromModules();

            FileUtils.EnsureDirectoryExists(ConfigDirectory);
            var json = JsonSerializer.Serialize(_configData, JsonOptions);
            File.WriteAllText(ConfigPath, json);
            YjPlugin.Log.LogInfo("Config saved successfully.");
        }
        catch (Exception ex)
        {
            YjPlugin.Log.LogError($"Config save error: {ex}");
        }
    }

    public static void RegisterModuleConfig(BaseModule module)
    {
        var type = module.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ConfigAttribute>() != null)
            .ToList();

        var moduleConfigs = new List<ConfigProperty>();
        var moduleNodeName = module.Name;

        if (!_configData.TryGetPropertyValue(moduleNodeName, out var moduleNode) || !(moduleNode is JsonObject))
        {
            moduleNode = new JsonObject();
            _configData[moduleNodeName] = moduleNode;
        }
        var moduleSettings = (JsonObject)moduleNode;

        foreach (var prop in properties)
        {
            var configAttr = prop.GetCustomAttribute<ConfigAttribute>();
            var propName = prop.Name;
            var defaultValue = prop.GetValue(module);

            if (moduleSettings.TryGetPropertyValue(propName, out var savedValueNode) && savedValueNode != null)
            {
                try
                {
                    var typedValue = savedValueNode.Deserialize(prop.PropertyType, JsonOptions);
                    prop.SetValue(module, typedValue);
                }
                catch (Exception ex)
                {
                    YjPlugin.Log.LogWarning($"Config value conversion failed for {moduleNodeName}.{propName}. Using default value. Error: {ex.Message}");
                    prop.SetValue(module, defaultValue);
                    try
                    {
                        moduleSettings[propName] = JsonSerializer.SerializeToNode(defaultValue, JsonOptions);
                    }
                    catch (Exception innerEx)
                    {
                        YjPlugin.Log.LogError($"Failed to serialize default value for {moduleNodeName}.{propName}: {innerEx.Message}");
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
                    YjPlugin.Log.LogError($"Failed to serialize default value for {moduleNodeName}.{propName}: {ex.Message}");
                }
            }

            moduleConfigs.Add(new ConfigProperty(prop, configAttr));
        }

        ModuleConfigProperties[module] = moduleConfigs;
    }

    public static void UpdatePropertyValue(BaseModule module, string propertyName, object newValue)
    {
        var moduleNodeName = module.Name;
        if (_configData.TryGetPropertyValue(moduleNodeName, out var moduleNode) && moduleNode is JsonObject moduleSettings)
        {
            try
            {
                moduleSettings[propertyName] = JsonSerializer.SerializeToNode(newValue, JsonOptions);
                // SaveConfig(); // auto-save 
            }
            catch (Exception ex)
            {
                YjPlugin.Log.LogError($"Failed to update property value for {moduleNodeName}.{propertyName}: {ex.Message}");
            }
        }
    }

    public static bool TryGetConfigProperties(BaseModule module, out List<ConfigProperty> properties)
    {
        return ModuleConfigProperties.TryGetValue(module, out properties);
    }

    private static void EnsureConfigFileExists()
    {
        try
        {
            FileUtils.EnsureDirectoryExists(ConfigDirectory);

            if (!File.Exists(ConfigPath))
            {
                File.WriteAllText(ConfigPath, "{}");
                YjPlugin.Log.LogInfo($"Created new config file: {ConfigPath}");
            }
        }
        catch (Exception ex)
        {
            YjPlugin.Log.LogError($"Config file/directory creation failed: {ex}");
            throw;
        }
    }

    private static void TryCreateBackup(string suffix = "Corrupted")
    {
        if (!File.Exists(ConfigPath)) return;

        try
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupPath = $"{ConfigPath}.{suffix}_{timestamp}.bak";
            File.Move(ConfigPath, backupPath);
            YjPlugin.Log.LogWarning($"Created backup of potentially corrupted config: {backupPath}");
        }
        catch (Exception backupEx)
        {
            YjPlugin.Log.LogError($"Backup creation failed: {backupEx}");
        }
    }

    private static void UpdateConfigDataFromModules()
    {
        foreach (var (module, configProps) in ModuleConfigProperties)
        {
            var moduleNodeName = module.Name;
            if (!_configData.TryGetPropertyValue(moduleNodeName, out var moduleNode) || !(moduleNode is JsonObject))
            {
                moduleNode = new JsonObject();
                _configData[moduleNodeName] = moduleNode;
            }
            var moduleSettings = (JsonObject)moduleNode;

            foreach (var configProp in configProps)
            {
                try
                {
                    var currentValue = configProp.Property.GetValue(module);
                    moduleSettings[configProp.Property.Name] = JsonSerializer.SerializeToNode(currentValue, JsonOptions);
                }
                catch (Exception ex)
                {
                    YjPlugin.Log.LogError($"Failed to serialize property value for {moduleNodeName}.{configProp.Property.Name} during save: {ex.Message}");
                }
            }
        }
    }
}