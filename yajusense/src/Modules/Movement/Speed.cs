using UnityEngine;
using VRC.SDKBase;
using yajusense.Core.Config;
using yajusense.Platform.VRC;

namespace yajusense.Modules.Movement;

public class Speed : ModuleBase
{
	public Speed() : base("Speed", "god of speed!", ModuleCategory.Movement) { }

	[Config("Movement speed", "Movement speed", false, 1.0f, 50.0f)]
	public float MovementSpeed { get; set; } = 10.0f;

	public override void OnUpdate()
	{
		if (!VRCHelper.IsInWorld())
			return;

		VRCPlayerApi localPlayer = VRCHelper.GetLocalVRCPlayerApi();

		if (Input.GetKey(KeyCode.W))
			localPlayer.gameObject.transform.position += localPlayer.gameObject.transform.forward * (MovementSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.S))
			localPlayer.gameObject.transform.position -= localPlayer.gameObject.transform.forward * (MovementSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.A))
			localPlayer.gameObject.transform.position -= localPlayer.gameObject.transform.right * (MovementSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.D))
			localPlayer.gameObject.transform.position += localPlayer.gameObject.transform.right * (MovementSpeed * Time.deltaTime);
	}
}