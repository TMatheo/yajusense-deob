using System;
using VRC.SDKBase;
using yajusense.Core;

namespace yajusense.Utils;

public static class VRCUtils
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

    public static bool IsInWorld() => GetLocalVRCPlayerApi() != null;
    
    public static void SafeExecuteInWorld(Action action)
    {
        if (!IsInWorld()) return;
        
        action();
    }
}