using System;
using ExitGames.Client.Photon;
using HarmonyLib;
using VRC.SDKBase;
using yajusense.Core;
using yajusense.Extensions;
using yajusense.Utils;

// ReSharper disable InconsistentNaming

namespace yajusense.Patches;

public static class OpRaiseEventPatch
{
    public static void ApplyPatch()
    {
        var originalMethod = AccessTools.Method(typeof(LoadBalancingClient_Internal),
            nameof(LoadBalancingClient_Internal
                .Method_Public_Virtual_New_Boolean_Byte_Object_ObjectPublicObByObInByObObUnique_SendOptions_0));
        
        HarmonyPatcher.ApplyPatch("OpRaiseEventPatch", originalMethod, new HarmonyMethod(typeof(OpRaiseEventPatch).GetMethod(nameof(Prefix))));
    }

    public static void Prefix(byte param_1, Il2CppSystem.Object param_2, ObjectPublicObByObInByObObUnique param_3,
        SendOptions param_4)
    {
        if (param_1 == 12)
        {
            byte[] serverTime = BitConverter.GetBytes(Networking.GetServerTimeInMilliseconds());
            byte[] data = SerializationUtils.FromIL2CPPToManaged<byte[]>(param_2);

            string serverTimeString = serverTime.ToHexString();
            string dataString = data.ToHexString();
            
            YjPlugin.Log.LogInfo($"Server time: {serverTimeString}");
            YjPlugin.Log.LogInfo($"Data: {dataString}");
        }
    }
}