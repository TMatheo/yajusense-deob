using VRC.SDKBase;

namespace yajusense.Utils;

public static class VRCUtils
{
    public static VRCPlayerApi GetLocalVRCPlayerApi()
    {
        if (Networking.LocalPlayer == null) return null;

        return Networking.LocalPlayer;
    }

    public static bool IsInWorld()
    {
        return GetLocalVRCPlayerApi() != null;
    }
}