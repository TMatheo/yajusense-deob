using VRC.SDKBase;

namespace yajusense.Utils;

public static class VRCUtils
{
    public static VRCPlayerApi GetLocalVRCPlayerApi()
    {
        if (VRC.SDKBase.Networking.LocalPlayer == null) return null;

        return VRC.SDKBase.Networking.LocalPlayer;
    }

    public static bool IsInWorld()
    {
        return GetLocalVRCPlayerApi() != null;
    }
}