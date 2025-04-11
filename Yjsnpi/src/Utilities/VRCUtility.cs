using UnityEngine;
using VRC.SDKBase;
using Yjsnpi.Core;

namespace Yjsnpi.Utilities;

public static class VRCUtility
{
    public static VRCPlayerApi GetLocalVRCPlayerApi()
    {
        if (Networking.LocalPlayer == null)
        {
            YjPlugin.Log.LogDebug("Attempted to get local VRCPlayerApi, but it was null");
            return null;
        }
        
        return Networking.LocalPlayer;
    }

    public static bool IsInWorld()
    {
        return GetLocalVRCPlayerApi() != null;
    }
}