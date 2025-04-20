using ExitGames.Client.Photon;
using HarmonyLib;
using Il2CppSystem;
using yajusense.Core;
using yajusense.Networking;
using yajusense.Utils;
using Array = System.Array;
using BitConverter = System.BitConverter;

namespace yajusense.Patches;

public static class OpRaiseEventPatch
{
    public static bool ShouldSendE12 { get; set; } = true;

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
            var serverTime = BitConverter.GetBytes(VRC.SDKBase.Networking.GetServerTimeInMilliseconds());
            var data = Il2CppSerializationUtils.FromIL2CPPToManaged<byte[]>(param_2);

            var serverTimeString = HexUtils.ToHexString(serverTime);
            var dataString = HexUtils.ToHexString(data);
            var posString = HexUtils.ToHexString(VRCUtils.GetLocalVRCPlayerApi().gameObject.transform.position);

            YjPlugin.Log.LogInfo($"Server time: {serverTimeString}");
            YjPlugin.Log.LogInfo($"Data: {dataString}");
            YjPlugin.Log.LogInfo($"Position: {posString}");

            var positionStartIndex = 21;
            var positionSize = 12;

            var quaternionData = new byte[5];
            Array.Copy(data, positionStartIndex + positionSize, quaternionData, 0, 5);
            var quat = QuaternionSerializer.Deserialize(quaternionData);

            YjPlugin.Log.LogInfo($"Rotation: {quat}");

            if (!ShouldSendE12)
                return false;
        }

        return true;
    }
}