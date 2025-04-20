using ExitGames.Client.Photon;

namespace yajusense.Patches;

public static class NetworkManagerOnEventPatch
{
    public static NetworkManager_Internal Instance { get; private set; }

    public static void ApplyPatches()
    {
        
    }

    public static void Prefix(NetworkManager_Internal __instance, EventData param_1)
    {
        if (Instance == null)
            Instance = __instance;
    }
}