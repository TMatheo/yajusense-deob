using ExitGames.Client.Photon;
using HarmonyLib;
using Il2CppSystem;
using VRC.SDKBase;
using yajusense.Core;
using yajusense.Utils;

namespace yajusense.Patches;

public static class OpRaiseEventPatch
{
    public static bool ShouldSendE12 { get; set; }

    public static void ApplyPatch()
    {
        var originalMethod = AccessTools.Method(typeof(LoadBalancingClient_Internal),
            nameof(LoadBalancingClient_Internal
                .Method_Public_Virtual_New_Boolean_Byte_Object_ObjectPublicObByObInByObObUnique_SendOptions_0));

        HarmonyPatcher.ApplyPatch("OpRaiseEventPatch", originalMethod,
            new HarmonyMethod(typeof(OpRaiseEventPatch).GetMethod(nameof(Prefix))));
    }

    public static bool Prefix(byte param_1, Object param_2, ObjectPublicObByObInByObObUnique param_3,
        SendOptions param_4)
    {
        if (param_1 == 12)
        {
            var serverTime = System.BitConverter.GetBytes(Networking.GetServerTimeInMilliseconds());
            var data = SerializationUtils.FromIL2CPPToManaged<byte[]>(param_2);

            var serverTimeString = HexUtils.ToHexString(serverTime);
            var dataString = HexUtils.ToHexString(data);
            var posString = HexUtils.ToHexString(VRCUtils.GetLocalVRCPlayerApi().gameObject.transform.position);

            YjPlugin.Log.LogInfo($"Server time: {serverTimeString}");
            YjPlugin.Log.LogInfo($"Data: {dataString}");
            YjPlugin.Log.LogInfo($"Position: {posString}");

            if (!ShouldSendE12)
                return false;
        }

        return true;
    }
}