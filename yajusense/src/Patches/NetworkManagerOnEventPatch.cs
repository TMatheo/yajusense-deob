using ExitGames.Client.Photon;
using HarmonyLib;
using yajusense.Core;

namespace yajusense.Patches;

public class NetworkManagerOnEventPatch : BasePatch
{
    public static NetworkManager_Internal Instance { get; private set; }

    protected override void Initialize()
    {
        var originalMethod = AccessTools.Method(typeof(NetworkManager_Internal),
            nameof(NetworkManager_Internal.Method_Public_Virtual_Final_New_Void_EventData_0));
        
        ConfigurePatch(originalMethod, prefixName: nameof(Prefix));
    }
    
    public static void ApplyPatch()
    {
        ApplyPatch<NetworkManagerOnEventPatch>();
    }

    public static void Prefix(NetworkManager_Internal __instance, EventData param_1)
    {
        if (Instance == null)
            Instance = __instance;
        
        YjPlugin.Log.LogInfo($"Event received at NetworkManager: {param_1.Code}");
    }
}