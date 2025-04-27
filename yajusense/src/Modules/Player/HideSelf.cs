using System.Linq;
using UnityEngine;
using yajusense.Extensions;
using yajusense.Utils;
using yajusense.Utils.VRC;

namespace yajusense.Modules.Player;

public class HideSelf : ModuleBase
{
    private GameObject _cachedAvatarPanel;
    private GameObject _forwardDirection;

    public HideSelf() : base("HideSelf", "Hides your avatar (only prevents from crashing when using one like mesh)", ModuleCategory.Player) { }
    
    private bool TryInitialize()
    {
        if (!PlayerUtils.IsInWorld()) return false;

        var localPlayer = PlayerUtils.GetLocalVRCPlayerApi();
        if (localPlayer == null) return false;
        _forwardDirection = localPlayer.gameObject.transform
            .Find("ForwardDirection")?.gameObject;

        var cachedAvatarPanels = Resources.FindObjectsOfTypeAll<GameObject>()
            .Where(obj => obj.name == "Panel_SelectedAvatar" && 
                          obj.transform.GetFullPath().Contains("Menu_Avatars"))
            .ToArray();
        _cachedAvatarPanel = cachedAvatarPanels[0];
        
        return _forwardDirection != null && _cachedAvatarPanel != null;
    }

    private void SetAvatarElementsActive(bool active)
    {
        if (_forwardDirection != null)
            _forwardDirection.SetActive(active);
        
        if (_cachedAvatarPanel != null)
            _cachedAvatarPanel.SetActive(active);
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