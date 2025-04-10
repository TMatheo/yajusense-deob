using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using UnityEngine;
using Yjsnpi.Core.Config.JsonConverters;
using Yjsnpi.Modules;
using Yjsnpi.Utilities;

namespace Yjsnpi.Core.Config;

public static class ConfigManager
{
    private static string ConfigDirectory => Path.Combine(Directory.GetCurrentDirectory(), "Yjsnpi");
    private static string ConfigPath => Path.Combine(ConfigDirectory, "ModuleConfig.cock");
    
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

    private static Dictionary<string, Dictionary<string, object>> _configData = new();
    private static readonly Dictionary<BaseModule, List<ConfigProperty>> ModuleConfigProperties = new();

    public static void Initialize()
    {
        EnsureConfigFileExists();
        LoadConfig();
    }
    
    public static void LoadConfig()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                
                if (string.IsNullOrWhiteSpace(json))
                {
                    _configData = new();
                    YjPlugin.Log.LogInfo("[Yjsnpi] Config file was empty, initialized with defaults");
                }
                else
                {
                    _configData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(json, JsonOptions);
                }
            }
        }
        catch (Exception ex)
        {
            YjPlugin.Log.LogError($"[Yjsnpi] Config load error: {ex}");
            _configData = new();
            
            TryCreateBackup();
        }
            
        _configData ??= new();
    }
    
    public static void SaveConfig()
    {
        UpdateConfigData();
            
        try
        {
            FileUtility.EnsureDirectoryExists(ConfigDirectory);
            var json = JsonSerializer.Serialize(_configData, JsonOptions);
            File.WriteAllText(ConfigPath, json);
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

        foreach (var prop in properties)
        {
            var configAttr = prop.GetCustomAttribute<ConfigAttribute>();
            var defaultValue = prop.GetValue(module);

            if (!_configData.TryGetValue(module.Name, out var moduleSettings))
            {
                moduleSettings = new();
                _configData[module.Name] = moduleSettings;
            }

            if (moduleSettings.TryGetValue(prop.Name, out var savedValue))
            {
                try
                {
                    var jsonValue = JsonSerializer.SerializeToElement(savedValue);
                    var typedValue = jsonValue.Deserialize(prop.PropertyType, JsonOptions);
                    prop.SetValue(module, typedValue);
                }
                catch (Exception ex)
                {
                    YjPlugin.Log.LogError($"Config value conversion failed for {module.Name}.{prop.Name}: {ex}");
                    prop.SetValue(module, defaultValue);
                }
            }
            else
            {
                moduleSettings[prop.Name] = defaultValue;
            }

            moduleConfigs.Add(new(prop, configAttr));
        }

        ModuleConfigProperties[module] = moduleConfigs;
        SaveConfig();
    }
    
    public static bool TryGetConfigProperties(BaseModule module, out List<ConfigProperty> properties)
    {
        return ModuleConfigProperties.TryGetValue(module, out properties);
    }
    
    private static void EnsureConfigFileExists()
    {
        try
        {
            FileUtility.EnsureDirectoryExists(ConfigDirectory);
                
            if (!File.Exists(ConfigPath))
            {
                var initialData = new Dictionary<string, Dictionary<string, object>>();
                File.WriteAllText(ConfigPath, JsonSerializer.Serialize(initialData, JsonOptions));
                YjPlugin.Log.LogInfo("[Yjsnpi] Created new config file: " + ConfigPath);
            }
        }
        catch (Exception ex)
        {
            YjPlugin.Log.LogError($"[Yjsnpi] Config file creation failed: {ex}");
        }
    }
    
    private static void TryCreateBackup()
    {
        try
        {
            string backupPath = ConfigPath + ".corrupted_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            if (File.Exists(ConfigPath))
            {
                File.Move(ConfigPath, backupPath);
                YjPlugin.Log.LogInfo($"[Yjsnpi] Created backup of corrupted config: {backupPath}");
            }
        }
        catch (Exception backupEx)
        {
            YjPlugin.Log.LogError($"[Yjsnpi] Backup creation failed: {backupEx}");
        }
    }

    private static void UpdateConfigData()
    {
        foreach (var (module, configProps) in ModuleConfigProperties)
        {
            var moduleSettings = new Dictionary<string, object>();
                
            foreach (var configProp in configProps)
            {
                var value = configProp.Property.GetValue(module);
                moduleSettings[configProp.Property.Name] = value;
            }

            _configData[module.Name] = moduleSettings;
        }
    }

    
}