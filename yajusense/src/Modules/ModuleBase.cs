using UnityEngine;
using VRC.SDKBase;
using yajusense.Core.Config;
using yajusense.Core.Services;
using yajusense.UI;

namespace yajusense.Modules;

public abstract class ModuleBase
{
    protected ModuleBase(string name, string description, ModuleCategory category, KeyCode toggleKey = KeyCode.None, bool enabled = false)
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

    public virtual void OnUpdate() { }
    public virtual void OnGUI() { }
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }
    public virtual void OnPlayerJoined(VRCPlayerApi player) { }
    public virtual void OnPlayerLeft(VRCPlayerApi player) { }

    public void Enable()
    {
        Enabled = true;

        OnEnable();

        ConfigManager.UpdatePropertyValue(this, nameof(Enabled), Enabled);
        NotificationManager.ShowNotification($"Enabled {Name}");
        AudioService.PlayAudio(AudioService.AudioClipType.ModuleEnable);
    }

    public void Disable()
    {
        Enabled = false;

        OnDisable();

        ConfigManager.UpdatePropertyValue(this, nameof(Enabled), Enabled);
        NotificationManager.ShowNotification($"Disabled {Name}");
        AudioService.PlayAudio(AudioService.AudioClipType.ModuleDisable);
    }

    public void Toggle()
    {
        if (Enabled)
            Disable();
        else
            Enable();
    }
}

public enum ModuleCategory
{
    Visual,
    Movement,
    Player,
    ClientSettings,
}