using UnityEngine;
using yajusense.Core.Config;

namespace yajusense.Modules.Visual.HUD;

public abstract class BaseHUD : BaseModule
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; protected set; }
    
    protected BaseHUD(string name, string description, Vector2 position, Vector2 size, bool enabled = false) 
        : base(name, description, ModuleType.Visual, KeyCode.None, enabled)
    {
        Position = position;
        Size = size;
    }
}