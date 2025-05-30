using yajusense.Core.Config;

namespace yajusense.Modules.Other;

public class ClientSettings : ModuleBase
{
	public ClientSettings() : base("ClientSetting", "Global client settings", ModuleCategory.ClientSettings) { }

	[Config("Color step", "Controls the speed of the color effect used in various UI elements", min: 0.01f, max: 0.1f)]
	public float ColorStep { get; set; } = 0.03f;

	[Config("Sound effects volume", "Controls the volume of the sound effects", min: 0.0f, max: 1.0f)]
	public float SoundEffectsVolume { get; set; } = 1f;
}