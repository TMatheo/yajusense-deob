using VRC.Core;
using VRC.SDKBase;

namespace yajusense.Utils.VRC;

public static class PlayerUtils
{
    public static VRCPlayerApi GetLocalVRCPlayerApi()
    {
        if (global::VRC.SDKBase.Networking.LocalPlayer == null) return null;

        return global::VRC.SDKBase.Networking.LocalPlayer;
    }

    public static bool IsInWorld()
    {
        return GetLocalVRCPlayerApi() != null;
    }

    public static void ChangeAvatar(string avatarID)
    {
        var apiAvatar = new ApiAvatar()
        {
            id = avatarID
        };
        UsedToChangeAvatar.Method_Public_Static_Void_ApiAvatar_String_0(apiAvatar);
    }
}