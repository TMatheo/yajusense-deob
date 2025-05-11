using UnityEngine;
using VRC.SDKBase;
using yajusense.Core.Config;
using yajusense.Modules.Visual;
using yajusense.Platform.VRC;
using yajusense.Services;

namespace yajusense.Modules.Movement;

public class ClickTP : ModuleBase
{
	public ClickTP() : base("ClickTP", "Teleport to where you are looking at", ModuleCategory.Movement) { }

	[Config("Teleport Key", "Key to trigger teleport")]
	public KeyCode TeleportKey { get; set; } = KeyCode.Mouse0;

	public override void OnUpdate()
	{
		if (!VRCHelper.IsInWorld() || ModuleManager.GetModule<Menu>().Enabled)
			return;

		if (Input.GetKeyDown(TeleportKey))
			TryTeleport();
	}

	private void TryTeleport()
	{
		var thirdPerson = ModuleManager.GetModule<ThirdPerson>();
		Camera camera = thirdPerson.Enabled ? thirdPerson.GetCamera() : Camera.main;

		VRCPlayerApi localPlayer = VRCHelper.GetLocalVRCPlayerApi();
		Transform playerTransform = localPlayer.gameObject.transform;

		var ray = new Ray(camera.transform.position, camera.transform.forward);

		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			localPlayer.TeleportTo(hit.point, playerTransform.rotation);

			AudioService.PlayAudio(AudioService.AudioClipType.Teleport);
		}
	}
}