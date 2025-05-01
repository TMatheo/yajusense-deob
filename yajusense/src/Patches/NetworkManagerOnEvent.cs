using System.Reflection;
using ExitGames.Client.Photon;
using HarmonyLib;

namespace yajusense.Patches;

public class NetworkManagerOnEvent : PatchBase
{
    protected override void Initialize()
    {
        MethodInfo originalMethod = AccessTools.Method(typeof(NetworkManager_Internal), nameof(NetworkManager_Internal.Method_Public_Virtual_Final_New_Void_EventData_0));

        ConfigurePatch(originalMethod, nameof(Prefix));
    }

    public static void ApplyPatch()
    {
        ApplyPatch<NetworkManagerOnEvent>();
    }

    public static void Prefix(EventData param_1)
    {
        if (param_1.Code == 12) { }
    }
}