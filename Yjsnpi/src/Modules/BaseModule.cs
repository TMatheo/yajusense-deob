using UnityEngine;

namespace Yjsnpi.Modules;

public abstract class BaseModule
{
    public string Name { get; }
    public string Description { get; }
    public ModuleType Type { get; }
    public KeyCode ToggleKey { get; private set; }
    public bool Enabled { get; private set; }

    protected BaseModule(string name, string description, ModuleType type, KeyCode toggleKey = KeyCode.None)
    {
        Name = name;
        Description = description;
        Type = type;
        ToggleKey = toggleKey;
    }

    public virtual void OnUpdate() { }
    public virtual void OnRender() { }
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }

    public void Toggle()
    {
        Enabled = !Enabled;
        if (Enabled)
        {
            OnEnable();
        }
        else
        {
            OnDisable();
        }
    }

    public void SetToggleKey(KeyCode key) => ToggleKey = key;
}

public enum ModuleType
{
    Visual,
    Movement,
}