using yajusense.Core.Config;

namespace yajusense.Modules.Other;

public class ClientSettings : BaseModule
{
    [Config("Rainbow Color Step", "Controls the speed of the rainbow color effect used in various UI elements", min: 0.01f, max: 0.1f)]
    public float RainbowColorStep { get; set; } = 0.04f;

    public ClientSettings() : base("ClientSetting", "Global client settings", ModuleCategory.ClientSettings) { }
}