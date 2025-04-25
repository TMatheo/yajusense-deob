using UnityEngine;
using yajusense.Core.Config;
using yajusense.UI;

namespace yajusense.Modules;

public abstract class BaseModule
{
    protected BaseModule(string name, string description, ModuleCategory category, KeyCode toggleKey = KeyCode.None, bool enabled = false)
    {
        Name = name;
        Description = description;
        Category = category;
        ToggleKey = toggleKey;
        Enabled = enabled;
    }

    public string Name { get; }
    public string Description { get; }
    public ModuleCategory Category { get; }

    [Config("Enabled", "Is this module enabled?", true)]
    public bool Enabled { get; set; }

    [Config("Toggle Key", "Key to enable/disable this module")]
    public KeyCode ToggleKey { get; set; }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnGUI()
    {
    }

    public virtual void OnEnable()
    {
    }

    public virtual void OnDisable()
    {
    }

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
}

public enum ModuleCategory
{
    Visual,
    Movement,
    ClientSettings
}