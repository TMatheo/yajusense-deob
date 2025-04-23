using System;
using ExitGames.Client.Photon;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using yajusense.Extensions;
using yajusense.Patches;
using yajusense.Utils;

namespace yajusense.Networking;

public static class EventSender
{
    private const int PositionDataIndex = 21;

    public static void SendMovementEvent(Vector3 position, Quaternion rotation)
    {
        if (OpRaiseEventPatch.LastData == null || !VRCUtils.IsInWorld())
            return;

        var lastData = OpRaiseEventPatch.LastData;
        byte[] sender =
            BitConverter.GetBytes(VRCUtils.GetLocalVRCPlayerApi().GetPlayer().GetPlayerNet().GetPhotonNumber());
        byte[] serverTime = BitConverter.GetBytes(VRC.SDKBase.Networking.GetServerTimeInMilliseconds());
        
        byte[] positionBytes = DataConvertionUtils.Vector3ToBytes(position);
        byte[] rotationBytes = QuaternionSerializer.Serialize(rotation);
        
        Buffer.BlockCopy(sender, 0, lastData, 0, sender.Length);
        Buffer.BlockCopy(serverTime, 0, lastData, sender.Length, serverTime.Length);
        
        Buffer.BlockCopy(positionBytes, 0, lastData, PositionDataIndex, positionBytes.Length);
        Buffer.BlockCopy(rotationBytes, 0, lastData, PositionDataIndex + positionBytes.Length, rotationBytes.Length);
        
        RaiseEvent(12, lastData);
    }

    private static void RaiseEvent(byte code, object content)
    {
        PhotonNetwork_Internal.Method_Public_Static_Boolean_Byte_Object_ObjectPublicObByObInByObObUnique_SendOptions_0(
            code,
            Il2CppSerializationUtils.FromManagedToIL2CPP<Il2CppSystem.Object>(content),
            null,
            SendOptions.SendUnreliable);
    }
}