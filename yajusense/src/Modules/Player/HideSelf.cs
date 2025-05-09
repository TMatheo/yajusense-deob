using UnityEngine;
using VRC.SDKBase;
using yajusense.VRC;

namespace yajusense.Modules.Player;

public class HideSelf : ModuleBase
{
	private GameObject _forwardDirection;

	public HideSelf() : base("HideSelf", "Hides your avatar (doesnt from menu)", ModuleCategory.Player) { }

	private bool TryInitialize()
	{
		if (!VRCHelper.IsInWorld())
			return false;

		VRCPlayerApi localPlayer = VRCHelper.GetLocalVRCPlayerApi();
		if (localPlayer == null)
			return false;

		_forwardDirection = localPlayer.gameObject.transform.Find("ForwardDirection")?.gameObject;

		return _forwardDirection != null;
	}

	private void SetAvatarElementsActive(bool active)
	{
		if (_forwardDirection != null)
			_forwardDirection.SetActive(active);
	}

	public override void OnEnable()
	{
		if (TryInitialize())
			SetAvatarElementsActive(false);
	}

	public override void OnDisable()
	{
		if (VRCHelper.IsInWorld())
			SetAvatarElementsActive(true);
	}
}