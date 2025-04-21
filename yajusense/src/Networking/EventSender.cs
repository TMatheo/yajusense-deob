using ExitGames.Client.Photon;
using UnityEngine;
using yajusense.Core;
using yajusense.Patches;
using yajusense.Utils;

namespace yajusense.Networking;

public static class EventSender
{
    private const int PositionDataIndex = 21;
    
    public static void SendEvent(byte code, object data, int sender)
    {
        if (NetworkManagerOnEventPatch.Instance == null)
        {
            YjPlugin.Log.LogError("NetworkManagerOnEventPatch.Instance is null at SendEvent");
            return;
        }

        EventData eventData = new()
        {
            Code = code,
            customData = Il2CppSerializationUtils.FromManagedToIL2CPP<Il2CppSystem.Object>(data),
            Sender = sender
        };
        
        NetworkManagerOnEventPatch.Instance.Method_Public_Virtual_Final_New_Void_EventData_0(eventData);
    }

    public static void SendMovementEvent(Vector3 position, Quaternion rotation)
    {
        if (OpRaiseEventPatch.LastData == null)
            return;

        var lastData = OpRaiseEventPatch.LastData;
    }
}