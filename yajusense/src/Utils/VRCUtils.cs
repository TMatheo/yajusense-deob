using System.Linq;
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

    public static VRCPlayerApi GetVRCPlayerApiByID(int id)
    {
        return VRCPlayerApi.AllPlayers?.ToArray()
            .FirstOrDefault(player => player != null && player.playerId == id);
    }
}