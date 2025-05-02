using VRC.SDKBase;
using yajusense.Core.Services;
using yajusense.UI;

namespace yajusense.Modules.Visual;

public class PlayerNotification : ModuleBase
{
	public PlayerNotification() : base("PlayerNotification", "Shows a notification when a player joins or leaves", ModuleCategory.Visual, enabled: true) { }

	public override void OnPlayerJoined(VRCPlayerApi player)
	{
		NotificationManager.ShowNotification($"{player.displayName} joined the world");
		AudioService.PlayAudio(AudioService.AudioClipType.PlayerNotification);
	}

	public override void OnPlayerLeft(VRCPlayerApi player)
	{
		NotificationManager.ShowNotification($"{player.displayName} left the world");
		AudioService.PlayAudio(AudioService.AudioClipType.PlayerNotification);
	}
}