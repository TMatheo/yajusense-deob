using System.Linq;
using UnityEngine;
using VRC.SDKBase;
using yajusense.Extensions;
using yajusense.Utils.VRC;

namespace yajusense.Modules.Player;

public class HideSelf : ModuleBase
{
    private GameObject _forwardDirection;

    public HideSelf() : base("HideSelf", "Hides your avatar (doesnt from menu)", ModuleCategory.Player) { }

    private bool TryInitialize()
    {
        if (!PlayerUtils.IsInWorld())
            return false;

        VRCPlayerApi localPlayer = PlayerUtils.GetLocalVRCPlayerApi();
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
        if (PlayerUtils.IsInWorld())
            SetAvatarElementsActive(true);
    }
}