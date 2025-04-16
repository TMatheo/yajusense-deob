using UnityEngine;
using Yjsnpi.Core.Config;
using Yjsnpi.UI;

namespace Yjsnpi.Modules;

public abstract class BaseModule
{
    public string Name { get; }
    public string Description { get; }
    public ModuleType Type { get; }
    
    [Config("Enabled", "Is this module enabled?", true)]
    public bool Enabled { get; set; }
    
    [Config("Toggle Key", "Key to enable/disable this module")]
    public KeyCode ToggleKey { get; set; }

    protected BaseModule(string name, string description, ModuleType type, KeyCode toggleKey = KeyCode.None, bool enabled = false)
    {
        Name = name;
        Description = description;
        Type = type;
        ToggleKey = toggleKey;
        Enabled = enabled;
    }

    public virtual void OnUpdate() { }
    public virtual void OnGUI() { }
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }

    public void Toggle()
    {
        Enabled = !Enabled;
        
        ConfigManager.UpdatePropertyValue(this, nameof(Enabled), Enabled);
        
        if (Enabled)
        {
            OnEnable();
            NotificationManager.ShowNotification($"Enabled {Name}");
        }
        else
        {
            OnDisable();
            NotificationManager.ShowNotification($"Disabled {Name}");
        }
    }

    public void SetToggleKey(KeyCode key) => ToggleKey = key;
}

public enum ModuleType
{
    Visual,
    Movement,
}