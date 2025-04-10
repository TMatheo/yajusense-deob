using UnityEngine;
using VRC.SDKBase;
using Yjsnpi.Core;

namespace Yjsnpi.Utilities;

public static class VRCUtility
{
    public static VRCPlayerApi GetLocalPlayer()
    {
        if (Networking.LocalPlayer == null)
        {
            YjPlugin.Log.LogDebug("Attempted to get local player, but it is not in world");
            return null;
        }
        
        return Networking.LocalPlayer;
    }

    public static bool IsInWorld()
    {
        return GetLocalPlayer() != null;
    }
}